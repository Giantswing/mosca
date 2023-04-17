using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class TargetGroupControllerSystem : MonoBehaviour
{
    [Serializable]
    private class CustomCameraTarget
    {
        public Transform Transform;
        public float Weight;
        public float Radius;
    }


    [SerializeField] private List<CustomCameraTarget> cameraTargets = new();
    private CinemachineTargetGroup _targetGroup;
    public static TargetGroupControllerSystem Instance;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    private Camera mainCamera;
    [SerializeField] private Ease _easeOut;

    [FormerlySerializedAs("cameraData")] [SerializeField]
    private List<AttributeDataSO> playerList = new();

    private CinemachineTransposer transposer;
    private CinemachineComposer composer;
    private Vector3 startingPositionOffset;
    private Vector3 startingAimOffset;

    private Vector3 positionOffset;
    private Vector3 aimOffset;

    private Vector3 positionOffsetTo;
    private Vector3 aimOffsetTo;

    private Vector3 cameraZoneOffset;
    private Vector3 cameraZoneOffsetTo;
    private float cameraZoneZoom;
    private float cameraZoneZoomTo;
    private float cameraZoneSideAngleStrength;
    private float cameraZoneSideAngleStrengthTo;

    [SerializeField] private float playerZoomOffset = 0;
    [SerializeField] private float playerZoomOffsetTo = 0;
    [SerializeField] private float playerZoomThreshold = 0.1f;
    [SerializeField] private float playerZoomInThreshold = 0.3f;
    [SerializeField] private int numPlayersIn = 0;
    [SerializeField] private bool shouldZoomOut = false;
    [SerializeField] private bool shouldZoomIn = false;

    [SerializeField] private float posOffsetSpeed = 2f;
    [SerializeField] private float aimOffsetSpeed = 2f;
    [SerializeField] private float cameraZoneOffsetSpeed = 1f;
    [SerializeField] private float cameraZoneZoomSpeed = 1f;

    [SerializeField] private float flipStrength = 2f;
    [SerializeField] private GameObject playerPrefab;
    private bool canChangeTarget = true;


    private PlayerInputManager _playerInputManager;
    [SerializeField] private InputAction joinPlayerAction;

    private void Awake()
    {
        _targetGroup = GetComponent<CinemachineTargetGroup>();
        //_targets.Add(_targetGroup.m_Targets[0].target);
        Instance = this;

        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        composer = virtualCamera.GetCinemachineComponent<CinemachineComposer>();
        startingPositionOffset = transposer.m_FollowOffset;
        startingAimOffset = composer.m_TrackedObjectOffset;
        cameraZoneSideAngleStrengthTo = 1;
        mainCamera = Camera.main;

        _playerInputManager = GetComponent<PlayerInputManager>();
    }


    private void OnEnable()
    {
        joinPlayerAction.performed += JoinPlayer;
        joinPlayerAction.Enable();
    }

    private void OnDisable()
    {
        joinPlayerAction.performed -= JoinPlayer;
        joinPlayerAction.Disable();
    }


    public static void ModifyTarget(Transform target, float weight, float radius, float duration = 2f)
    {
        int index = Instance._targetGroup.FindMember(target);
        if (index == -1)
            return;

        Instance.cameraTargets[index].Weight = weight;
    }

    public static void AddTarget(Transform target, float weight, float radius, float duration = 2f,
        bool useDuration = true)
    {
        for (var i = 0; i < Instance._targetGroup.m_Targets.Length; i++)
            if (Instance._targetGroup.m_Targets[i].target == target)
                return;

        if (target.TryGetComponent(out Attributes attributes))
            if (attributes.hasInstantiatedData)
            {
                Instance.playerList.Add(attributes.attributeData);
                if (Instance.playerList.Count > 1)
                    attributes.transform.position = Instance.playerList[0].attributes.transform.position;
                //attributes.gameObject.GetComponent<InputReceiver>().ForceController();
                //PlayerInput.all[PlayerInput.all.Count - 1].SwitchCurrentControlScheme("Gamepad", Gamepad.current);
            }


        Instance._targetGroup.AddMember(target, 0, 0);
        Instance.cameraTargets.Add(new CustomCameraTarget
        {
            Transform = target,
            Weight = weight,
            Radius = radius
        });
    }

    public static void RemoveTarget(Transform target)
    {
        if (Instance._targetGroup.FindMember(target) == -1)
            return;

        Instance.cameraTargets.Find(x => x.Transform == target).Weight = 0;
    }

    public static Transform ClosestPlayer(Transform from)
    {
        if (Instance.playerList.Count == 0)
            return null;

        Transform closest = Instance.playerList[0].attributes.transform;
        float closestDistance = Vector3.Distance(from.position, closest.position);

        for (var i = 1; i < Instance.playerList.Count; i++)
        {
            float distance = Vector3.Distance(from.position, Instance.playerList[i].attributes.transform.position);
            if (distance < closestDistance)
            {
                closest = Instance.playerList[i].attributes.transform;
                closestDistance = distance;
            }
        }

        return closest;
    }

    private void LerpFields()
    {
        cameraZoneOffset = Vector3.Lerp(cameraZoneOffset, cameraZoneOffsetTo, Time.deltaTime * cameraZoneOffsetSpeed);
        cameraZoneZoom = Mathf.Lerp(cameraZoneZoom, cameraZoneZoomTo, Time.deltaTime * cameraZoneZoomSpeed);
        cameraZoneSideAngleStrength =
            Mathf.Lerp(cameraZoneSideAngleStrength, cameraZoneSideAngleStrengthTo,
                Time.deltaTime * cameraZoneOffsetSpeed);
        positionOffset = Vector3.Lerp(positionOffset, positionOffsetTo, Time.deltaTime * posOffsetSpeed);
        aimOffset = Vector3.Lerp(aimOffset, aimOffsetTo, Time.deltaTime * aimOffsetSpeed);
    }

    private void UpdateWeights()
    {
        for (var i = 0; i < cameraTargets.Count; i++)
        {
            int index = _targetGroup.FindMember(cameraTargets[i].Transform);
            if (index == -1)
                continue;

            if (_targetGroup.m_Targets[index].weight < cameraTargets[i].Weight)
                _targetGroup.m_Targets[index].weight = Mathf.Lerp(_targetGroup.m_Targets[index].weight,
                    cameraTargets[i].Weight, Time.deltaTime * 0.2f);
            else
                _targetGroup.m_Targets[index].weight = Mathf.Lerp(_targetGroup.m_Targets[index].weight,
                    cameraTargets[i].Weight, Time.deltaTime * 2f);

            if (_targetGroup.m_Targets[index].weight < 0.01f && cameraTargets[i].Weight < 0.01f)
                _targetGroup.m_Targets[index].weight = 0;

            _targetGroup.m_Targets[index].radius = Mathf.Lerp(_targetGroup.m_Targets[index].radius,
                cameraTargets[i].Radius, Time.deltaTime * 2f);
        }
    }

    private void CalculatePlayerOutOfView()
    {
        if (playerList.Count < 1) return;
        shouldZoomOut = false;
        shouldZoomIn = false;
        numPlayersIn = 0;

        for (var i = 0; i < playerList.Count; i++)
        {
            Vector3 playerCameraPos = mainCamera.WorldToViewportPoint(playerList[i].attributes.transform.position);

            if (playerCameraPos.x < playerZoomThreshold || playerCameraPos.x > 1 - playerZoomThreshold ||
                playerCameraPos.y < playerZoomThreshold || playerCameraPos.y > 1 - playerZoomThreshold)
                shouldZoomOut = true;


            if (playerCameraPos.x > playerZoomInThreshold && playerCameraPos.x < 1 - playerZoomInThreshold &&
                playerCameraPos.y > playerZoomInThreshold && playerCameraPos.y < 1 - playerZoomInThreshold)
                numPlayersIn++;
        }

        if (numPlayersIn == playerList.Count)
            shouldZoomIn = true;

        if (shouldZoomOut)
        {
            playerZoomOffsetTo += Time.deltaTime * 5f;
        }

        else if (shouldZoomIn)
        {
            playerZoomOffsetTo -= Time.deltaTime * 5f;
            if (playerZoomOffsetTo < 0f) playerZoomOffsetTo = 0;
        }

        playerZoomOffset = Mathf.Lerp(playerZoomOffset, playerZoomOffsetTo, Time.deltaTime * 1f);
    }

    private void FixedUpdate()
    {
        positionOffsetTo = Vector3.zero;
        aimOffsetTo = Vector3.zero;
        float finalFlipStrength = 0;
        float finalZoom = 0;

        if (playerList.Count > 0) finalFlipStrength = flipStrength * cameraZoneSideAngleStrength / playerList.Count;
        finalFlipStrength /= playerZoomOffset + 1f;

        for (var i = 0; i < playerList.Count; i++)
        {
            positionOffsetTo.x += finalFlipStrength * playerList[i].flipSystem.flipDirection;
            aimOffsetTo.x += finalFlipStrength * 2f * playerList[i].flipSystem.flipDirection;
        }

        UpdateWeights();
        LerpFields();
        CalculatePlayerOutOfView();


        transposer.m_FollowOffset = startingPositionOffset + positionOffset + cameraZoneOffset +
            Vector3.forward * cameraZoneZoom - Vector3.forward * playerZoomOffset;
        composer.m_TrackedObjectOffset = startingAimOffset + aimOffset + cameraZoneOffset;
    }

    public static void ChangePlayersEnabled(bool enabled = true)
    {
        for (var i = 0; i < Instance.playerList.Count; i++)
        {
            Instance.playerList[i].movementSystem.enabled = enabled;
            Instance.playerList[i].flipSystem.enabled = enabled;
            Instance.playerList[i].dashAbility.enabled = enabled;
            //Instance.playerList[i].doubleDashAbility.enabled = enabled;
            Instance.playerList[i].chargeShot.enabled = enabled;
        }
    }


    public void JoinPlayer(InputAction.CallbackContext context)
    {
        /*
        print("player joined");

        if (context.performed)
        {
            InputDevice device = context.control.device;
            print(device.name);

            GameObject playerGO = Instantiate(playerPrefab, playerList[0].attributes.transform.position,
                Quaternion.identity);
        }
        */
        if (context.performed)
        {
            print("test");
            //playerList[0].playerInput.neverAutoSwitchControlSchemes = true;
            _playerInputManager.EnableJoining();
        }
    }

    public void SetUpPlayer()
    {
        /*
        DOVirtual.DelayedCall(0.05f, () =>
        {
            Transform latestPlayer = playerList[playerList.Count - 1].attributes.transform;
            latestPlayer.position = playerList[0].attributes.transform.position;
        });
        */
    }


    public static void JoinPlayerStatic(InputDevice deviceToAssignToPlayer)
    {
        GameObject playerGO = Instantiate(Instance.playerPrefab, Instance.playerList[0].attributes.transform.position,
            Quaternion.identity);

        PlayerInput playerInput = playerGO.GetComponent<PlayerInput>();
        playerInput.SwitchCurrentControlScheme(deviceToAssignToPlayer.name, deviceToAssignToPlayer);
    }


    public static void SetCameraZoneOffset(Vector3 cameraZoneCameraOffset, float cameraZoneZoomOffset,
        float cameraZoneSideAngleStrengthOffset)
    {
        Instance.cameraZoneOffsetTo = cameraZoneCameraOffset;
        Instance.cameraZoneZoomTo = cameraZoneZoomOffset;
        Instance.cameraZoneSideAngleStrengthTo = cameraZoneSideAngleStrengthOffset;
    }
}
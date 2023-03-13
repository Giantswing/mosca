using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueTriggerEvent : MonoBehaviour
{
    public DialogueSO dialogueSO;
    [SerializeField] private Transform prompt;
    [SerializeField] private GameObject promptButton;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private SmartData.SmartInt.IntReader playerController; //1 keyboard 2 xbox 3 switch 4 ps4 5 ps5
    [SerializeField] private InputActionAsset inputActionAsset;

    private void Start()
    {
        prompt.gameObject.SetActive(false);
    }

    public void ShowPrompt()
    {
        prompt.gameObject.SetActive(true);
        prompt.localScale = Vector3.zero;
        prompt.DOScale(1, 0.6f).SetEase(Ease.OutBounce);

        /*
        if (playerController == 1)
        {
            promptButton.SetActive(false);
            promptText.gameObject.SetActive(true);
            var key = inputActionAsset.FindActionMap("Gameplay").FindAction("Map").bindings[0].effectivePath;

            //clean up key string, remove <Keyboard> and / from the string
            key = key.Replace("<Keyboard>", "");
            key = key.Replace("/", "");

            promptText.SetText(key);
        }
        else
        {
            promptButton.SetActive(true);
            promptText.gameObject.SetActive(false);
        }
        */
        //promptButtonRenderer.material.SetTexture(MainTex, triangleButton[playerController.value - 1].texture);
    }

    public void HidePrompt()
    {
        prompt.DOScale(0, 0.6f).SetEase(Ease.InElastic).OnComplete(() => prompt.gameObject.SetActive(false));
    }
}
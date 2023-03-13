using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ControllerSelectButtonUI : MonoBehaviour
{
    [SerializeField] private bool isAButton;
    [SerializeField] private TextMeshProUGUI actionText;
    [SerializeField] private string actionName;
    [SerializeField] private Sprite[] triangleButton;
    [SerializeField] private RawImage buttonImage;
    [SerializeField] private SmartData.SmartInt.IntReader playerController; //1 keyboard 2 xbox 3 switch 4 ps4 5 ps5
    [SerializeField] private InputActionAsset inputActionAsset;


    private void OnEnable()
    {
        if (isAButton)
        {
            if (playerController.value > 1)
            {
                buttonImage.texture = triangleButton[playerController.value - 2].texture;
                buttonImage.enabled = true;
            }
            else
            {
                buttonImage.enabled = false;
            }
        }
        else
        {
            if (playerController.value == 1)
            {
                actionText.enabled = true;
                //buttonImage.enabled = false;
                var key = inputActionAsset.FindActionMap("Gameplay").FindAction(actionName).bindings[0].effectivePath;
                //clean up key string, remove <Keyboard> and / from the string
                key = key.Replace("<Keyboard>", "");
                key = key.Replace("/", "");
                //make the text lowercase
                key = key.ToUpper();

                //make string substitution so that they work with keycaps font
                key = key.Replace("SPACE", "w");

                actionText.SetText(key);
            }
            else
            {
                actionText.enabled = false;
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class UIController : MonoBehaviour
{
    [SerializeField] private Button _testButton;

    // Start is called before the first frame update
    private void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        var buttons = root.Query<Button>().ToList();
        var buttonPrefab = buttons[0];

        //add new ui button


        foreach (var button in buttons) button.clickable.clicked += TestButtonPressed;
    }

    // Update is called once per frame
    private void TestButtonPressed()
    {
        print("test");
    }
}
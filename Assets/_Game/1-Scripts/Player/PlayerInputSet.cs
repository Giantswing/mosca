using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputSet : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;

    [SerializeField] private SmartData.SmartInt.IntWriter
        currentControlSchemeWriter; //1 = keyboard, 2 = xbox, 3 = switch, 4 = ps4, 5 = ps5

    public static PlayerInputSet Instance;

    private void Awake()
    {
        Instance = this;
    }

    public static int GetPlayerInput()
    {
        if (Instance != null)
            return Instance.currentControlSchemeWriter.value;
        else
            return 1;
    }


    public void TestMethod()
    {
        var currentControlScheme = playerInput.currentControlScheme;
        if (currentControlScheme == "Gamepad")
        {
            var gamepad = Gamepad.current;
            if (gamepad == null) return;
            //print(gamepad.name);
            if (gamepad.name.Contains("DualSense"))
                currentControlSchemeWriter.value = 5;
            else if (gamepad.name.Contains("DualShock"))
                currentControlSchemeWriter.value = 4;
            else if (gamepad.name.Contains("Switch"))
                currentControlSchemeWriter.value = 3;
            else
                currentControlSchemeWriter.value = 2;
        }
        else
        {
            //print(currentControlScheme);
            currentControlSchemeWriter.value = 1;
        }
    }
}
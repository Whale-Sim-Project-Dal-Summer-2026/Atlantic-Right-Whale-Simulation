/**
 * IdleUI.cs: Implements an idle mode.
 * 
 * @author Mars Semenova
 */

using UnityEngine;
using UnityEngine.InputSystem;

public class IdleUI : MonoBehaviour {
    // params
    // idle mode params
    [Header("Idle Mode Parameters")]
    [SerializeField] private GameObject idleUIVal;
    private static GameObject idleUI;
    [SerializeField] private float idleTimeVal = 300;
    private static float idleTime;
    
    // events
    public delegate void IdleEvent(bool on);
    public static event IdleEvent OnIdle;
    
    // vars
    private static float lastInput;
    private static double lastMouseInput;
    private static bool isIdle;
    
    void Awake() {
        // transfer editor input to vars
        idleUI = idleUIVal;
        idleTime = idleTimeVal;
        
        lastInput = Time.unscaledTime;
        lastMouseInput = Mouse.current.lastUpdateTime;
    }
        
    private void Update() {
        // check for input
        bool keyboardPress = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;
        bool controllerPress = Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame;
        bool mouseInput = Mouse.current != null && Mouse.current.lastUpdateTime != lastMouseInput;
        if (keyboardPress || controllerPress || mouseInput) { 
            lastInput = Time.unscaledTime;
            lastMouseInput = Mouse.current.lastUpdateTime;
        }
        
        // go to idle mode
        if (!isIdle && IsIdle()) {
            IdleMode(true);
            isIdle = true;
        }

        // leave idle mode
        if (isIdle && !IsIdle()) {
            IdleMode(false);
            isIdle = false;
        }
    }
        
    /**
     * Check whether idle mode is on.
     * @return Whether the idle mode is on.
     */
    private static bool IsIdle(){
        return (Time.unscaledTime - lastInput) > idleTime;
    }

    /**
     * Enter idle mode by displaying the UI and invoking an event.
     * @param on - Whether idle mode is on or off.
     */
    public static void IdleMode(bool on) {
        isIdle = on;
        idleUI.SetActive(on);
        OnIdle?.Invoke(on);
    }
}
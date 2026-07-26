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
    [SerializeField] private GameObject idleUI;
    [SerializeField] private float idleTime;
    // extra things to toggle
    [Header("Additional Parameters to be Hidden")]
    [SerializeField] private TogglesManager toggles;
    [SerializeField] private Scrubber scrubber;
    [SerializeField] private GameObject toggleUIBtn;
    
    // vars
    private float lastInput;
    private bool forceIdle = false;
    private bool isIdle = false;
    
    void Awake() {
        lastInput = Time.time;
    }
        
    private void Update() {
        // check for input
        bool keyboardPress = (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame);
        bool controllerPress = Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame;
        if (keyboardPress || controllerPress) { // TODO: mouse input
            lastInput = Time.time;
        }

        // force idle (largely for dev purposes)
        if (Keyboard.current.f10Key.wasPressedThisFrame) {
            forceIdle = !forceIdle;
        }
        
        // go to idle mode
        if (!isIdle && (IsIdle() || forceIdle)) {
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
    private bool IsIdle(){
        return (Time.time - lastInput > idleTime) || forceIdle;
    }

    /**
     * Set idle mode.
     * @param on - Whether idle mode is on or off.
     */
    private void IdleMode(bool on) {
        idleUI.SetActive(on);
        toggles.SetUIVisibility(!on);
        if (scrubber) {
            if (on) {
                scrubber.Play(); // TODO: loop
            }
            scrubber.gameObject.SetActive(!on);
        }
        toggleUIBtn.SetActive(!on);
    }
}
/**
 * TutorialDisplay.cs: Script which handles
 * the tutorial display. 
 *
 * @author Mars Semenova
 */

using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TutorialDisplay : MonoBehaviour {
    // params
    [Header("Dependencies")]
    [SerializeField] private Scrubber scrubber;
    [SerializeField] private GameObject toggleUIBtnObj; // TODO: could probably just have this be button in

    // vars
    private float lastInput;
    private Button toggleUIBtn;
    private double lastMouseInput;
    private bool firstInput = false;
    
    void Awake() {
        // get ref
        if (toggleUIBtnObj) {
            toggleUIBtn = toggleUIBtnObj.GetComponent<Button>();
        }
        
        lastInput = Time.unscaledTime;
        lastMouseInput = Mouse.current.lastUpdateTime;
    }

    void Start() {
        ShowTutorial(true);
    }

    private void Update() {
        bool keyboardPress = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame; // TODO: dupe code from idle mode, make a util func or smth
        bool controllerPress = Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame;
        bool mouseInput = Mouse.current != null && Mouse.current.lastUpdateTime != lastMouseInput;
        if (keyboardPress || controllerPress || mouseInput) {
            if (!firstInput) { // TODO: figure out why time jumps from 0 to like 10 (i.e. start is 10s not 0)
                firstInput = true;
            } else {
                ShowTutorial(false);
            }
            lastMouseInput = Mouse.current.lastUpdateTime;
        }
    }

    /**
     * Toggle tutorial display.
     * @param on - Whether the tutorial is displayed or not.
     */
    private void ShowTutorial(bool on) {
        if (toggleUIBtnObj) {
            toggleUIBtn.onClick.Invoke();
        }

        if (scrubber) {
            if (on) {
                scrubber.Pause(); 
            } else {
                scrubber.Play();
            }
            scrubber.gameObject.SetActive(!on);
        }
        else {
            if (on) {
                WhaleConnector.PauseWhale();
            } else {
                WhaleConnector.PlayWhale();
            }
        }
        
        toggleUIBtnObj.SetActive(!on);

        if (!on) {
           gameObject.SetActive(false);
        }
    }
}
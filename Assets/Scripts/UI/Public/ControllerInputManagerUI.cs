/**
 * ControllerInputManagerUI.cs: Script which integrates controller input
 * with the UI functionality.
 *
 * @author Mars Semenova, Dany Diab
 */

using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerInputManagerUI : MonoBehaviour {
    // params
    // options
    [Header("Options")]
    public float pressBuffer = 300; // ms
    // scripts
    [Header("Scripts")]
    [SerializeField] private Popup helpPopup;
    [SerializeField] private Popup viewerPopup;
    [SerializeField] private Scrubber scrubber;
    [SerializeField] private PublicUIManager publicUIManager;

    // actions
    private InputAction helpAction;
    private InputAction backAction;
    private InputAction pauseAction;
    private InputAction resetAction;
    
    // vars
    private double lastPressTime;

    void Awake() {
        // set up actions
        helpAction = InputSystem.actions.FindAction("OpenHelp");
        backAction = InputSystem.actions.FindAction("BackAction");
        pauseAction = InputSystem.actions.FindAction("Pause");
        resetAction = InputSystem.actions.FindAction("Reset");
        
        lastPressTime = Time.time;
    }

    void Update() {
        // please use seperate variables, it makes the code more readable
        bool interaction;
        
        interaction = (helpAction?.ReadValue<float>() ?? 0.0f) == 1.0f;
        if (interaction && PressAllowed())  {
            HelpBtnPressed();
        }
        
        interaction = (backAction?.ReadValue<float>() ?? 0.0f) == 1.0f;
        if (interaction && helpPopup.IsOpen() && PressAllowed())  {
            HelpCloseBtnPressed();
        }
        
        if (interaction && !helpPopup.IsOpen() && viewerPopup.IsOpen() && PressAllowed())  {
            ViewerCloseBtnPressed();
        }
        
        interaction = (pauseAction?.ReadValue<float>() ?? 0.0f) == 1.0f;
        if (interaction && PressAllowed())  {
            PausePlayBtnPressed();
        }

        interaction = (resetAction?.ReadValue<float>() ?? 0.0f) == 1.0f;
        if (interaction && PressAllowed()) {
            ResetBtnPressed();
        }
    }

    /**
     * Check if controller input is allowed. This is determined by a delay between inputs and
     * whether user input is allowed by the program. 
     * 
     * @return Whether input is allowed.
     */
    private bool PressAllowed(){
        if (publicUIManager.IsInputAllowed()) {
            double currTime = Time.unscaledTimeAsDouble * 1000;

            if (currTime - lastPressTime > pressBuffer){
                lastPressTime = currTime;
                return true;
            }
        }

        return false;
    }
    
    /**
     * Implements the help button press functionality.
     */
    private void HelpBtnPressed() {
        if (helpPopup) {
            helpPopup.PopupBtnPressed();
        }
    }

    /**
     * Implements back button functionality for the help popup.
     */
    private void HelpCloseBtnPressed() { // TODO: make sure this plays nice with multiple popups/other back actions
        if (helpPopup) {
            helpPopup.SetPopupVisibility(false);
        }
    }
    
    /**
     * Implements back button functionality for the viewer popup.
     */
    private void ViewerCloseBtnPressed() { // TODO: make sure this plays nice with multiple popups/other back actions
        if (viewerPopup) {
            viewerPopup.SetPopupVisibility(false);
        }
    }

    /**
     * Implements pause/play button functionality.
     */
    private void PausePlayBtnPressed() {
        if (scrubber) {
            scrubber.TogglePause();
        }
    }
    
    /**
     * Implements reset button functionality.
     */
    private void ResetBtnPressed() {
        if (scrubber) {
            scrubber.Restart();
        }
    }
}
/**
 * ControllerInputManagerUI.cs: Script which integrates controller input
 * with the UI functionality.
 *
 * @author Mars Semenova, Dany Diab
 */

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum InputState {
    ENABLED,
    DISABLED,
    POPUP,
    NOUI
}

public class ControllerInputManagerUI : MonoBehaviour {
    // params

    // scripts
    [Header("Scripts")]
    [SerializeField] private Popup helpPopup;
    [SerializeField] private Popup viewerPopup;
    [SerializeField] private Scrubber scrubber;
    [SerializeField] private CameraController camController;
    // btns
    [Header("Buttons")] 
    [SerializeField] private Button helpBtn;
    [SerializeField] private Button closeHelpBtn;
    [SerializeField] private Button scenariosBtn;
    [SerializeField] private Button closeViewerBtn;
    [SerializeField] private Button toggleUIBtn;

    // actions for controller input
    // mid btns
    private InputAction openHelpInput;
    private InputAction openMenuInput;
    // btns
    private InputAction pauseInput;
    private InputAction backInput;
    // cross
    private InputAction cam1Input;
    private InputAction cam2Input;
    private InputAction cam3Input;
    private InputAction toggleUIInput;
    // sticks
    private InputAction camLockInput;
    
    // vars
    private double lastPressTime;
    private InputState currState = InputState.ENABLED;
    private CameraState cam = CameraState.ORBIT;
    private bool whaleController;

    void Start() {
        // search for whale controller
        ManualWhaleController[] whaleControllers = FindObjectsByType<ManualWhaleController>();
        whaleController = whaleControllers.Length != 0;
        
        // set up actions
        // mid btns
        openHelpInput = InputSystem.actions.FindAction("OpenHelp");
        openMenuInput = InputSystem.actions.FindAction("OpenMenu"); 
        // btns
        backInput = InputSystem.actions.FindAction("BackAction");
        pauseInput = InputSystem.actions.FindAction("Pause");
        // cross
        cam1Input = InputSystem.actions.FindAction("Cam1");
        cam2Input = InputSystem.actions.FindAction("Cam2");
        cam3Input = InputSystem.actions.FindAction("Cam3");
        toggleUIInput = InputSystem.actions.FindAction("ToggleUI");
        // sticks
        camLockInput = InputSystem.actions.FindAction("CamLock");
        
        // sub
        CameraController.OnCamSwitch += UpdateCam;
        PopupManager.OnHelpPopup += SetStatePopup;
        TogglesManager.OnToggleUI += SetStateNoUI;
        
        lastPressTime = Time.time;
    }

    void Update() {
        // check if input allowed
        if (currState == InputState.DISABLED) {
            return;
        }
        
        // process input
        bool interaction;
        
        // mid btns
        if (ButtonPressUtil.Pressed(openHelpInput) && currState != InputState.NOUI)  {
            if (helpBtn) {
                helpBtn.onClick.Invoke();
            }
        }
        if (ButtonPressUtil.Pressed(openMenuInput) && currState != InputState.POPUP && currState != InputState.NOUI) {
            if (scenariosBtn) {
                scenariosBtn.onClick.Invoke();
            }
        }
        
        // btns
        // back btn
        if (ButtonPressUtil.Pressed(backInput))  {
            if (currState != InputState.POPUP && viewerPopup.IsOpen()) {
                if (closeViewerBtn) {
                    closeViewerBtn.onClick.Invoke();
                }
            }
            if (helpPopup.IsOpen()) {
                if (closeHelpBtn) {
                    closeHelpBtn.onClick.Invoke();
                }
            }
        }
        // top btn
        if (ButtonPressUtil.Pressed(pauseInput) && !whaleController && currState != InputState.POPUP)  { // paused disabled in free roam
            if (scrubber) {
                scrubber.TogglePause();
            }
        }
        
        // cross
        // cams
        if (ButtonPressUtil.Pressed(cam1Input) && currState != InputState.POPUP) {
            camController.changeToCam(1);
        }
        if (ButtonPressUtil.Pressed(cam2Input) && !whaleController && currState != InputState.POPUP) { // cam 2 disabled in free roam
            camController.changeToCam(2);
        }
        if (ButtonPressUtil.Pressed(cam3Input) && currState != InputState.POPUP) {
            camController.changeToCam(3);
        }
        // hide ui
        if (ButtonPressUtil.Pressed(toggleUIInput) && currState != InputState.POPUP) {
            if (toggleUIBtn) {
                toggleUIBtn.onClick.Invoke();
            }
        }
       
        // R stick
        if (ButtonPressUtil.Pressed(camLockInput) && !whaleController && (cam == CameraState.ORBIT || cam == CameraState.FREE) && currState != InputState.POPUP) { // rot lock in cam 1
            if (cam == CameraState.ORBIT) {
                camController.lockUnLockCamera();
            }

            if (cam == CameraState.FREE) {
                camController.resetFreeCam();
            }
        }
    }

    private void OnDestroy() {
        // unsub
        CameraController.OnCamSwitch -= UpdateCam;
        PopupManager.OnHelpPopup -= SetStatePopup;
        TogglesManager.OnToggleUI -= SetStateNoUI;
    }

    /**
     * Camera event subscriber which sets the corresponding
     * state based on the camera.
     * @param currCam - Current active camera.
     */
    private void UpdateCam(int currCam) {
        if (currCam == 1) {
            cam = CameraState.ORBIT;
        }
        if (currCam == 2) {
            cam = CameraState.FREE;
        }
        if (currCam == 3) {
            cam = CameraState.POV;
        }
    }

    /**
     * Sets the current state to InputState.POPUP.
     * @param on - Whether the state should be in the popup state or not.
     */
    private void SetStatePopup(bool on) {
        currState = on ? InputState.POPUP : InputState.ENABLED;
    }
    
    /**
     * Sets the current state to InputState.NOUI.
     * @param on - Whether the state should be in the no UI state or not.
     */
    private void SetStateNoUI(bool on) {
        currState = !on ? InputState.NOUI : InputState.ENABLED;
    }
}
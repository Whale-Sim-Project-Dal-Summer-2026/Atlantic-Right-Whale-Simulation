/**
 * InputManager.cs: Script which manages input.
 *
 * @author Mars Semenova, Dany Diab
 */

using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour {
    // params
    // actions 
    private static InputAction actionOpenHelp;
    private static InputAction actionOpenMenu;
    private static InputAction actionPause;
    private static InputAction actionBack;
    private static InputAction actionCam1;
    private static InputAction actionCam2;
    private static InputAction actionCam3;
    private static InputAction actionToggleUI;
    private static InputAction actionCamLock;
    private static InputAction actionReset;
    private static InputAction actionAccelerate; 
    private static InputAction actionMove;
    private static InputAction actionLook;
    private static InputAction actionUpDown;
    private static InputAction actionOpenMouth;
    private static InputAction actionSpeedUp;
    
    // input
    private static bool inputOpenHelp;
    private static bool inputOpenMenu;
    private static bool inputPause;
    private static bool inputBack;
    private static bool inputCam1;
    private static bool inputCam2;
    private static bool inputCam3;
    private static bool inputToggleUI;
    private static bool inputCamLock;
    private static bool inputReset;
    private static float inputAccelerate; 
    private static Vector2 inputMove;
    private static Vector2 inputLook;
    private static Vector2 inputUpDown;
    private static float inputOpenMouth;
    private static bool inputSpeedUp;
    
    // vars
    private static StateCamera cam;
    private static StateUI ui;
    private static StateIdle idle;
    private static StateMode mode;
    private static StateHelpPopup helpPopup;

    void Start() {
        // set up actions
        actionOpenHelp = InputSystem.actions.FindAction("OpenHelp");
        actionOpenMenu = InputSystem.actions.FindAction("OpenMenu"); 
        actionBack = InputSystem.actions.FindAction("BackAction");
        actionPause = InputSystem.actions.FindAction("Pause");
        actionCam1 = InputSystem.actions.FindAction("Cam1");
        actionCam2 = InputSystem.actions.FindAction("Cam2");
        actionCam3 = InputSystem.actions.FindAction("Cam3");
        actionToggleUI = InputSystem.actions.FindAction("ToggleUI");
        actionCamLock = InputSystem.actions.FindAction("CamLock");
        actionReset = InputSystem.actions.FindAction("Reset");
        actionMove = InputSystem.actions.FindAction("Move");
        actionLook = InputSystem.actions.FindAction("Look");
        actionUpDown = InputSystem.actions.FindAction("Up/Down");
        actionAccelerate = InputSystem.actions.FindAction("Accelerate"); // TODO: implement feedback
        actionOpenMouth = InputSystem.actions.FindAction("OpenMouth");
        actionSpeedUp = InputSystem.actions.FindAction("SimulationSpeedUp");

        mode = SceneManager.GetStateMode();
    }

    void Update() {
        // fetch states
        cam = CameraManager.GetStateCamera();
        ui = StateManager.GetStateUI();
        idle = StateManager.GetStateIdle();
        helpPopup = StateManager.GetStateHelpPopup();
        
        if (ButtonPressUtil.Pressed(actionOpenHelp))  {
            UIManager.OnHelp();
        }
        if (ButtonPressUtil.Pressed(actionOpenMenu)) {
            UIManager.OnScenarios();
        }
        
        if (ButtonPressUtil.Pressed(actionBack))  {
            UIManager.OnBack();
        }
        
        if (ButtonPressUtil.Pressed(actionPause) && mode != StateMode.FREESWIM && helpPopup == StateHelpPopup.HIDE)  { // paused disabled in free swim
            UIManager.OnTogglePause();
        }
        
        if (ButtonPressUtil.Pressed(actionCam1) && helpPopup == StateHelpPopup.HIDE) {
            UIManager.OnCameraStateChange(StateCamera.ORBIT);
        }
        if (ButtonPressUtil.Pressed(actionCam2) && mode != StateMode.FREESWIM && helpPopup == StateHelpPopup.HIDE) { // cam 2 disabled in free swim
            UIManager.OnCameraStateChange(StateCamera.FREE);
        }
        if (ButtonPressUtil.Pressed(actionCam3) && helpPopup == StateHelpPopup.HIDE) {
            UIManager.OnCameraStateChange(StateCamera.POV);
        }
        
        if (ButtonPressUtil.Pressed(actionToggleUI) && helpPopup == StateHelpPopup.HIDE) {
            UIManager.OnToggleUI(!(ui == StateUI.SHOW));
        }
       
        if (ButtonPressUtil.Pressed(actionCamLock) && mode != StateMode.FREESWIM && (cam == StateCamera.ORBIT || cam == StateCamera.FREE) && helpPopup == StateHelpPopup.HIDE) { // rot lock in cam 1
            UIManager.OnCamLock();
        }
        
        if (ButtonPressUtil.Pressed(actionReset) && helpPopup == StateHelpPopup.HIDE) {
            UIManager.OnReset();
        }
        
        inputMove = Vector2.zero;
        if (helpPopup == StateHelpPopup.HIDE) {
            inputMove = actionMove?.ReadValue<Vector2>() ?? Vector2.zero;
        }

        inputLook = Vector2.zero;
        if (helpPopup == StateHelpPopup.HIDE) {
            inputLook = actionLook?.ReadValue<Vector2>() ?? Vector2.zero;
        }

        inputUpDown = Vector2.zero;
        if (helpPopup == StateHelpPopup.HIDE) {
            inputUpDown = actionUpDown?.ReadValue<Vector2>() ?? Vector2.zero;
        }

        inputAccelerate = 0.0f;
        if (helpPopup == StateHelpPopup.HIDE) {
            inputAccelerate = actionAccelerate?.ReadValue<float>() ?? 0.0f;
        }

        inputOpenMouth = 0.0f;
        if (helpPopup == StateHelpPopup.HIDE) {
            inputOpenMouth = actionOpenMouth?.ReadValue<float>() ?? 0.0f;
        }

        inputSpeedUp = ButtonPressUtil.Pressed(actionSpeedUp); // TODO
    }
    
    // getters
    public static float GetInputAccelerate() {
        return inputAccelerate;
    }
    public static Vector2 GetInputMove() {
        return inputMove;
    }
    public static Vector2 GetInputLook() {
        return inputLook;
    }
    public static Vector2 GetInputUpDown() {
        return inputUpDown;
    }
    public static float GetInputOpenMouth() { 
        return inputOpenMouth;
    }
}
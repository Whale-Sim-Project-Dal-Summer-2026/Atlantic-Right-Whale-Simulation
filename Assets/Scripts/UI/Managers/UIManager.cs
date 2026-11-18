/**
 * UIManager.cs: Script which implements UI functionality.
 *
 * @author Mars Semenova
 */

using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    // scripts
    [Header("Scripts")]
    [SerializeField] private Popup helpPopupVal;
    private static Popup helpPopup;
    [SerializeField] private Popup viewerPopupVal;
    private static Popup viewerPopup;
    [SerializeField] private Scrubber scrubberVal;
    private static Scrubber scrubber;
    [SerializeField] private WhaleConnector whaleConnectorVal;
    private static WhaleConnector whaleConnector;
    [SerializeField] private TogglesManager togglesManagerVal;
    private static TogglesManager togglesManager;
    [SerializeField] private Toggles togglesVal;
    private static Toggles toggles;

    // btns
    [Header("Buttons")] 
    [SerializeField] private Button helpBtnVal;
    private static Button helpBtn;
    [SerializeField] private Button closeHelpBtnVal;
    private static Button closeHelpBtn;
    [SerializeField] private Button scenariosBtnVal;
    private static Button scenariosBtn;
    [SerializeField] private Button closeViewerBtnVal;
    private static Button closeViewerBtn;
    [SerializeField] private Button toggleUIBtnVal;
    private static Button toggleUIBtn;
    [SerializeField] private Button cam1BtnVal;
    private static Button cam1Btn;
    [SerializeField] private Button cam2BtnVal;
    private static Button cam2Btn;
    [SerializeField] private Button cam3BtnVal;
    private static Button cam3Btn;

    // events
    public delegate void ToggleInteractivityEvent(bool on);
    public static event ToggleInteractivityEvent OnToggleInteractivity;
    
    void Awake() {
        // transfer editor input to vars
        helpPopup = helpPopupVal;
        viewerPopup = viewerPopupVal;
        scrubber = scrubberVal;
        whaleConnector = whaleConnectorVal;
        togglesManager = togglesManagerVal;
        toggles = togglesVal;
        helpBtn = helpBtnVal;
        closeHelpBtn = closeHelpBtnVal;
        scenariosBtn = scenariosBtnVal;
        closeViewerBtn = closeViewerBtnVal;
        toggleUIBtn = toggleUIBtnVal;
        cam1Btn = cam1BtnVal;
        cam2Btn = cam2BtnVal;
        cam3Btn = cam3BtnVal;

        SetUpScenariosBtn();
    }

    void Start() {
        // sub
        IdleUI.OnIdle += IdleMode;
        Scrubber.OnCamSwitch += CameraManager.changeToCam;
    }

    private void OnDestroy() {
        // unsub
        IdleUI.OnIdle -= IdleMode;
        Scrubber.OnCamSwitch -= CameraManager.changeToCam;
    }

    /**
     * Idle mode functionality.
     * @param on - Whether idle mode is on or off.
     */
    private void IdleMode(bool on) {
        if (toggleUIBtn && togglesManager.IsUIVisible() == on) {
            toggleUIBtn.onClick.Invoke();
        }

        if (scrubber) {
            if (on) {
                scrubber.Play(); 
                scrubber.SetEndOfPlaybackBehaviour(EndOfPlaybackBehaviour.Restart);
            } else {
                scrubber.SetEndOfPlaybackBehaviour(EndOfPlaybackBehaviour.Pause);
            }
            scrubber.gameObject.SetActive(!on);
        }
        
        toggleUIBtn.gameObject.SetActive(!on);
    }
    
    /**
     * Disable all interactable UI elements.
     * @param on - Whether the elements should be disabled.
     */
    public static void SetUIInteractivity(bool on) { 
        OnToggleInteractivity?.Invoke(on);
        
        if (scrubber) {
            scrubber.SetScrubberInteractivity(on);
        }

        if (toggles) {
            toggles.SetTogglesInteractivity(on);
        }
    }
    
    private void SetUpScenariosBtn() {
        scenariosBtn.onClick.AddListener(() => { // TODO: make part of separate component
            if (SceneManager.Instance != null) {
                SceneManager.Instance.changeToScenarios();
            }
        });
    }
    
    // implementations
    public static void OnCameraStateChange(StateCamera stateCamera) {
        if (stateCamera == StateCamera.ORBIT) {
            if (cam1Btn) {
                cam1Btn.onClick.Invoke();
            }
            else {
                CameraManager.changeToCam(1);
            }
        }
        if (stateCamera == StateCamera.FREE) {
            if (cam2Btn) {
                cam2Btn.onClick.Invoke();
            }
            else {
                CameraManager.changeToCam(2);
            }
        }
        if (stateCamera == StateCamera.POV) {
            if (cam3Btn) {
                cam3Btn.onClick.Invoke();
            }
            else {
                CameraManager.changeToCam(3);
            }
        }
    }

    public static void OnToggleUI(bool on) {
        if (toggleUIBtn) {
            toggleUIBtn.onClick.Invoke();
        }
    }

    public static void OnTogglePause() {
        if (scrubber) {
            scrubber.TogglePause();
        }
    }

    public static void OnHelp() {
        if (helpBtn) {
            helpBtn.onClick.Invoke();
        }
    }

    
    public static void OnScenarios() {
        if (scenariosBtn) {
            scenariosBtn.onClick.Invoke();
        }
    }

    public static void OnBack() {
        if (StateManager.GetStateInput() != StateInput.POPUP && viewerPopup.IsOpen()) {
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

    public static void OnCamLock() {
        StateCamera cam = CameraManager.GetStateCamera();
        if (cam == StateCamera.ORBIT) {
            CameraManager.lockUnLockCamera();
        }

        if (cam == StateCamera.FREE) {
            CameraManager.resetFreeCam();
        }
    }

    public static void OnReset() {
        whaleConnector.Reset();
    }
}

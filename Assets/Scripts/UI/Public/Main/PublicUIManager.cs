/**
 * PublicUIManager.cs: Script which handles
 * the public UI. Built on top of the functionality
 * in SimulationUIManager.
 *
 * @author Mars Semenova 
 */

using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PublicUIManager : MonoBehaviour {
    // objs
    private Button helpBtn;
    private HelpPopup helpUI;
    private Button scenariosBtn;
    private GameObject controlHintsFreeCam;
    private GameObject controlHintsFreeRoam;
    private GameObject controlHintsFollowCam;
    private GameObject controlHintsPOVCam;
    private ViewerPopup viewerUI;
    private Button funFactsBtn;
    private TextMeshProUGUI funFactsTxt;
    private Button unstickBtn;
    private Button closeViewerBtn;
    private Button closeViewerBtnController;
    private Collider whaleCollider;
    // extern
    private SimulationUIManager simUIManager;
    private IdleMode idleUI;
    private Button toggleUIAltBtn;
    private GameObject scrubberUI;
    private SceneSwitcher sceneSwitcher;
    
    // vars
    private bool isIdle = false;
    private bool isHelpOpen = false;
    private bool isPathOn = false;

    public ResetManager resetManager;
    
    private WhaleTrail whaleTrail;
    private int currCam;
        
    
    
    void Awake() {
        CameraController.OnCamSwitch += GetCurrCam;
        
        // get refs
        helpBtn = GameObject.Find("HelpBtn").GetComponent<Button>();
        helpUI = GameObject.Find("HelpUI").GetComponent<HelpPopup>();
        scenariosBtn = GameObject.Find("ScenariosBtn").GetComponent<Button>();
        GameObject controlHintsUI = GameObject.Find("ControlHintsUI");
        Transform checkT = controlHintsUI.transform.Find("ControlHintsFreeCam");
        if (checkT) {
            controlHintsFreeCam = checkT.gameObject;
        }
        checkT = controlHintsUI.transform.Find("ControlHintsFreeRoam");
        if (checkT) {
            controlHintsFreeRoam = checkT.gameObject;
        }
        checkT = controlHintsUI.transform.Find("ControlHintsFollowCam");
        if (checkT) {
            controlHintsFollowCam = checkT.gameObject;
        }
        checkT = controlHintsUI.transform.Find("ControlHintsPOVCam");
        if (checkT) {
            controlHintsPOVCam = checkT.gameObject;
        }
        viewerUI = GameObject.Find("ViewerUI").GetComponent<ViewerPopup>();
        GameObject funFactsObj = GameObject.Find("CyclingFunFactsTxt");
        funFactsTxt = funFactsObj.GetComponent<TextMeshProUGUI>();
        funFactsBtn = funFactsObj.GetComponent<Button>();
        GameObject check = GameObject.Find("UnstickBtn");
        if (check) {
            unstickBtn = check.GetComponent<Button>();
            check = GameObject.Find("WhaleTrail");
            if (check) {
                whaleTrail = check.GetComponent<WhaleTrail>();
            }
            unstickBtn.onClick.AddListener(() => {
                resetManager.TriggerReset();
                if (whaleTrail) {
                    whaleTrail.ResetPath();
                }
            });
        }
        GameObject viewerBG = GameObject.Find("ViewerUI").transform.Find("ViewerBG").gameObject;
        closeViewerBtn = viewerBG.transform.Find("CloseViewerBtn").GetComponent<Button>();
        closeViewerBtnController = viewerBG.transform.Find("CloseViewerBtnController").GetComponent<Button>();
        whaleCollider = GameObject.Find("Right Whale SF Mouth Articulation 1").GetComponent<Collider>();
        // extern
        simUIManager = GetComponent<SimulationUIManager>();
        idleUI = GameObject.Find("IdleUI").GetComponent<IdleMode>();
        toggleUIAltBtn = GameObject.Find("ToggleBtns").transform.Find("ToggleUIAltBtn").GetComponent<Button>();
        scrubberUI = GameObject.Find("ScrubberUI");
        sceneSwitcher = GameObject.Find("Scene Switcher").GetComponent<SceneSwitcher>();
    }

// camera event subscriber
    void GetCurrCam(int camIndex) {
        
    }

    void Start() {
        scenariosBtn.onClick.AddListener(sceneSwitcher.changeToScenarios);
    }

    void Update() {
        if (helpUI) {
            simUIManager.SetCursorLock(!helpUI.IsOpen()) ;
        }
        if (idleUI) {
            if (!isIdle && idleUI.IsIdle()) {
                SetIdleMode(true);
            }

            if (isIdle && !idleUI.IsIdle()) {
                isIdle = false;
                SetIdleMode(false);
            }
        }

        bool camSwitched = false; // TODO

    }
    
    
    
    

    private void SetIdleMode(bool on) {
        isIdle = on;
        simUIManager.SetUIVisibility(!on);
        simUIManager.SetPathVisibility(!on);
        if (on) {
            if (helpUI) { 
                helpUI.SetHelpPopupVisibility(false);
            }
            if (viewerUI) {
                viewerUI.SetViewerPopupVisibility(false);
            }
            if (toggleUIAltBtn) {
                toggleUIAltBtn.gameObject.SetActive(false);
            }
        }
        if (scrubberUI) {
            scrubberUI.SetActive(!isIdle);
        }
    }
    
    /**
     * Toggle UI.
     */
    public void ToggleUI(bool on) {
        bool showUI = on;

        // hide UI
        helpBtn.gameObject.SetActive(showUI);
        scenariosBtn.gameObject.SetActive(showUI);
        funFactsTxt.gameObject.SetActive(showUI);
        if (controlHintsFreeCam && currCam == 2) { // TODO: only activate if curr scene matches
            controlHintsFreeCam.SetActive(showUI);
        }
        if (controlHintsFreeRoam) { // TODO: only activate if curr scene matches
            controlHintsFreeRoam.SetActive(showUI);
        }
        if (controlHintsFollowCam && currCam == 1) { // TODO: only activate if curr scene matches
            controlHintsFollowCam.SetActive(showUI);
        }
        if (controlHintsPOVCam && currCam == 3) { // TODO: only activate if curr scene matches
            controlHintsPOVCam.SetActive(showUI);
        }
        if (helpUI) {
            if (!on && helpUI.IsOpen()) {
                helpUI.SetHelpPopupVisibility(false);
            }
        }
        if (viewerUI) {
            if (!on && viewerUI.IsOpen()) {
                viewerUI.SetViewerPopupVisibility(false);
            }
        }
        whaleCollider.enabled = on;
    }
    
    public void SetUIInteractivity(bool on) {
        helpBtn.interactable = on;
        scenariosBtn.interactable = on;
        funFactsBtn.interactable = on;
        if (unstickBtn) {
            unstickBtn.interactable = on;
        }
        closeViewerBtn.interactable = on;
        closeViewerBtnController.interactable = on;
        whaleCollider.enabled = on;
    }
}

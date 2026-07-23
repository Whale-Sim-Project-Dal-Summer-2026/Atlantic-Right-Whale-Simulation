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
    private int currCam = 1;

    public delegate void MenuToggleEvent(bool toggle);

    public static event MenuToggleEvent OnMenuToggle;
    
    void Awake() {
        CameraController.OnCamSwitch += GetCurrCam;
        HelpPopup.OnSwapStates += swapCursorLock;
        
        // get refs
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

    void OnDisable()
    {
        CameraController.OnCamSwitch -= GetCurrCam;
        HelpPopup.OnSwapStates -= swapCursorLock;
    }

    // camera event subscriber
    void GetCurrCam(int camIndex) {
        currCam = camIndex; 
        if (currCam == 1) {
            if (controlHintsFreeCam) { 
                controlHintsFreeCam.SetActive(false);
            }
            if (controlHintsFollowCam) { 
                controlHintsFollowCam.SetActive(true);
            }
            if (controlHintsPOVCam) { 
                controlHintsPOVCam.SetActive(false);
            }
        } 
        if (currCam == 2) {
            if (controlHintsFreeCam) { 
                controlHintsFreeCam.SetActive(true);
            }
            if (controlHintsFollowCam) { 
                controlHintsFollowCam.SetActive(false);
            }
            if (controlHintsPOVCam) { 
                controlHintsPOVCam.SetActive(false);
            }
        }
        if (currCam == 3) {
            if (controlHintsFreeCam) { 
                controlHintsFreeCam.SetActive(false);
            }
            if (controlHintsFollowCam) { 
                controlHintsFollowCam.SetActive(false);
            }
            if (controlHintsPOVCam) { 
                controlHintsPOVCam.SetActive(true);
            }
        }
    }

    void Start() {
        scenariosBtn.onClick.AddListener(sceneSwitcher.changeToScenarios);
    }

    void swapCursorLock(){
        simUIManager.SetCursorLock(Cursor.visible);
    }

    void Update() {

        if (idleUI) {
            if (!isIdle && idleUI.IsIdle()) {
                SetIdleMode(true);
            }

            if (isIdle && !idleUI.IsIdle()) {
                isIdle = false;
                SetIdleMode(false);
            }
        }
    }
    
    // TODO make IDLE MODE an event that different components listen for to toggle, rather than keeping references

    private void SetIdleMode(bool on) {
        isIdle = on;
        simUIManager.SetUIVisibility(!on);
        simUIManager.SetPathVisibility(!on);
        if (on) {
            if (helpUI) { 
                // removed this line because publicUI manager no longer knows about the help popup. (It handles its own visibility)
                // helpUI.SetHelpPopupVisibility(false);
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

        
        // TODO make ToggleUI an event that different components listen for to toggle, rather than keeping references
        // components should Subscribe to this event to know when to toggle! Talk to Dany if Help if needed with setup :)
        OnMenuToggle?.Invoke(showUI);


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
        if (viewerUI) {
            if (!on && viewerUI.IsOpen()) {
                viewerUI.SetViewerPopupVisibility(false);
            }
        }
        whaleCollider.enabled = on;
    }
    
    public void SetUIInteractivity(bool on) {
        // helpBtn.interactable = on;
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

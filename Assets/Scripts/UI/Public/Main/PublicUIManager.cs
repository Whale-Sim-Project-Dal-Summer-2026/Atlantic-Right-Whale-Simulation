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
    private ViewerPopup viewerUI;
    private Button funFactsBtn;
    private TextMeshProUGUI funFactsTxt;
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
    
    void Awake() {
        // get refs
        helpBtn = GameObject.Find("HelpBtn").GetComponent<Button>();
        helpUI = GameObject.Find("HelpUI").GetComponent<HelpPopup>();
        scenariosBtn = GameObject.Find("ScenariosBtn").GetComponent<Button>();
        GameObject controlHintsUI = GameObject.Find("ControlHintsUI");
        Transform check = controlHintsUI.transform.Find("ControlHintsFreeCam");
        if (check) {
            controlHintsFreeCam = check.gameObject;
        }
        check = controlHintsUI.transform.Find("ControlHintsFreeRoam");
        if (check) {
            controlHintsFreeRoam = check.gameObject;
        }
        check = controlHintsUI.transform.Find("ControlHintsFollowCam");
        if (check) {
            controlHintsFollowCam = check.gameObject;
        }
        viewerUI = GameObject.Find("ViewerUI").GetComponent<ViewerPopup>();
        GameObject funFactsObj = GameObject.Find("CyclingFunFactsTxt");
        funFactsTxt = funFactsObj.GetComponent<TextMeshProUGUI>();
        funFactsBtn = funFactsObj.GetComponent<Button>();
        whaleCollider = GameObject.Find("Right Whale SF Mouth Articulation 1").GetComponent<Collider>();
        // extern
        simUIManager = GetComponent<SimulationUIManager>();
        idleUI = GameObject.Find("IdleUI").GetComponent<IdleMode>();
        toggleUIAltBtn = GameObject.Find("ToggleBtns").transform.Find("ToggleUIAltBtn").GetComponent<Button>();
        scrubberUI = GameObject.Find("ScrubberUI");
        sceneSwitcher = GameObject.Find("Scene Switcher").GetComponent<SceneSwitcher>();
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
        if (controlHintsFreeCam) { // TODO: only activate if curr scene matches
            controlHintsFreeCam.SetActive(showUI);
        }
        if (controlHintsFreeRoam) { // TODO: only activate if curr scene matches
            controlHintsFreeRoam.SetActive(showUI);
        }
        if (controlHintsFollowCam) { // TODO: only activate if curr scene matches
            controlHintsFollowCam.SetActive(showUI);
        }
        if (helpUI) {
            helpUI.SetHelpPopupVisibility(false);
        }
        if (viewerUI) {
            viewerUI.SetViewerPopupVisibility(false);
        }
    }
    
    public void SetUIInteractivity(bool on) {
        helpBtn.interactable = on;
        scenariosBtn.interactable = on;
        funFactsBtn.interactable = on;
        // TODO: disable whale collider
    }
}

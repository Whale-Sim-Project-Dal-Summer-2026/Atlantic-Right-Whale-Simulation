/**
 * SimulationUIManager.cs: Script which handles
 * the simulation UI.
 *
 * @author Mars Semenova 
 */

using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SimulationUIManager : MonoBehaviour {
    // objs
    private TextMeshProUGUI fpsText;
    private GameObject forcesListUI;
    private GameObject headingBallUI;
    private GameObject depthUI;
    private GameObject flukingUI;
    private MeshRenderer whaleTrailMesh;
    
    // toggles
    private Button toggleUIBtn;
    private Button toggleUIAltBtn;
    private Button toggleDragBtn;
    private Sprite toggleDragSpriteOff;
    private Sprite toggleDragSpriteOn;
    private Image toggleDragBtnImage;
    private Button toggleStressBtn;
    private Sprite toggleStressSpriteOff;
    private Sprite toggleStressSpriteOn;
    private Image toggleStressBtnImage;
    private Button togglePathBtn;
    private Sprite togglePathSpriteOff;
    private Sprite togglePathSpriteOn;
    private Image togglePathBtnImage;
    private Button settingsBtn;
    // graphs
    private GraphRenderer depthGraph;
    private GraphRenderer depthRollingGraph;
    private GraphRenderer flukingGraph;
    private GraphRenderer flukingRollingGraph;
    // extern
    private PublicUIManager pubUIManager;
    private Scrubber scrubber;
    
    //vars
    CursorLockMode currCursorState = CursorLockMode.Locked;

    void Awake() {
        // get refs
        GameObject check = GameObject.Find("FPS");
        if (check) {
            fpsText = check.GetComponent<TextMeshProUGUI>();
        }
        forcesListUI = GameObject.Find("ForcesListUI");
        headingBallUI = GameObject.Find("HeadingBallUI");
        depthUI = GameObject.Find("DepthUI");
        flukingUI = GameObject.Find("FlukingUI");
        whaleTrailMesh = GameObject.Find("WhaleTrail").GetComponent<MeshRenderer>();
        // toggles
        toggleUIBtn = GameObject.Find("ToggleUIBtn").GetComponent<Button>();
        toggleUIAltBtn = GameObject.Find("ToggleBtns").transform.Find("ToggleUIAltBtn").GetComponent<Button>();
        GameObject toggleDragBtnObj = GameObject.Find("ToggleDragBtn");
        if (toggleDragBtnObj) {
            toggleDragBtn = toggleDragBtnObj.GetComponent<Button>();
            toggleDragSpriteOff = toggleDragBtnObj.GetComponent<Image>().sprite;
            toggleDragSpriteOn = Resources.Load<Sprite>("UI/Toggles/yesdrag");
            toggleDragBtnImage = toggleDragBtnObj.GetComponent<Image>();
        }
        GameObject toggleStressBtnObj = GameObject.Find("ToggleStressBtn");
        if (toggleStressBtnObj) {
            toggleStressBtn = toggleStressBtnObj.GetComponent<Button>();
            toggleStressSpriteOff = toggleStressBtnObj.GetComponent<Image>().sprite;
            toggleStressSpriteOn = Resources.Load<Sprite>("UI/Toggles/yesstress");
            toggleStressBtnImage = toggleStressBtnObj.GetComponent<Image>();
        }
        GameObject togglePathBtnObj = GameObject.Find("TogglePathBtn");
        if (togglePathBtnObj) {
            togglePathBtn = togglePathBtnObj.GetComponent<Button>();
            togglePathSpriteOff = togglePathBtnObj.GetComponent<Image>().sprite;
            togglePathSpriteOn = Resources.Load<Sprite>("UI/Toggles/yespath");
            togglePathBtnImage = togglePathBtnObj.GetComponent<Image>();
        }

        check = GameObject.Find("SettingsBtn");
        if (check) {
            settingsBtn = check.GetComponent<Button>();
        }
        // graphs
        depthGraph = GameObject.Find("DepthGraph").GetComponent<GraphRenderer>();
        depthRollingGraph = GameObject.Find("DepthRollingGraph").GetComponent<GraphRenderer>();
        check = GameObject.Find("FlukingGraph");
        if (check) {
            flukingGraph = check.GetComponent<GraphRenderer>();
        }
        check = GameObject.Find("FlukingRollingGraph");
        if (check) {
            flukingRollingGraph = check.GetComponent<GraphRenderer>();
        }
        // extern
        pubUIManager = GetComponent<PublicUIManager>();
        scrubber = GameObject.Find("ScrubberUI").GetComponent<Scrubber>();
    }
    
    void Start() {
        // setup
        SetGraphData();
        InvokeRepeating(nameof(ShowFPS), 0.01f, 0.5f);
        TogglesSetup();
    }
    
    void Update() {
        // toggle cursor (TODO: disable cam rotate)
        Cursor.lockState = Keyboard.current.altKey.isPressed ? CursorLockMode.None : currCursorState;
        
        // shortcuts (TODO)
        if (Keyboard.current.escapeKey.wasPressedThisFrame) {
            // TODO: open settings or close curr popup
        }
        if (Keyboard.current.uKey.wasPressedThisFrame) {
            SetUIVisibility(!toggleUIBtn.IsActive());
        }
    }
    
    /**
     * Set simulation data to be used for graphs.
     */
    private void SetGraphData() {
        TextAsset data = Resources.Load<TextAsset>("WhaleMovement/RW230714P48processed"); // TODO
        if (depthGraph) {
            depthGraph.SetData(data);
        }
        if (depthRollingGraph) {
            depthRollingGraph.SetData(data);
        }
        if (flukingGraph) {
            flukingGraph.SetData(data);
        }
        if (flukingRollingGraph) {
            flukingRollingGraph.SetData(data);
        }
    }

    /**
     * Update and display FPS.
     */
    private void ShowFPS() {
        if (fpsText) {
            fpsText.text = (Mathf.RoundToInt(Time.frameCount / Time.time)).ToString(); // TODO: not sure how correct this is
        }
    }

    /**
     * Set up toggle functionality. 
     */
    private void TogglesSetup() {
        // UI toggle
        if (toggleUIBtn) {
            toggleUIBtn.onClick.AddListener(() => SetUIVisibility(false));
        }
        if (toggleUIAltBtn) {
            toggleUIAltBtn.onClick.AddListener(() => SetUIVisibility(true));
        }

        // drag toggle
        if (toggleDragBtn) {
            toggleDragBtn.onClick.AddListener(() => {
                toggleDragBtnImage.sprite = toggleDragBtnImage.sprite == toggleDragSpriteOff ? toggleDragSpriteOn : toggleDragSpriteOff;
                // TODO
            });
        }

        // stress toggle
        if (toggleStressBtn) {
            toggleStressBtn.onClick.AddListener(() => {
                toggleStressBtnImage.sprite = toggleStressBtnImage.sprite == toggleStressSpriteOff ? toggleStressSpriteOn : toggleStressSpriteOff;
                // TODO
            });
        }

        // path UI
        if (togglePathBtn) {
            togglePathBtn.onClick.AddListener(() => SetPathVisibility(!whaleTrailMesh.enabled));
        }
    }
    
    /**
     * Togge UI visibility.
     */
    public void SetUIVisibility(bool on) {
        if (toggleUIBtn && toggleUIAltBtn) {
            // toggle buttons
            toggleUIBtn.gameObject.SetActive(on);
            toggleUIAltBtn.gameObject.SetActive(!on);

            // hide UI
            if (fpsText) {
                fpsText.gameObject.SetActive(on);
            }
            if (forcesListUI) {
                forcesListUI.SetActive(on);
            }
            if (headingBallUI) {
                headingBallUI.SetActive(on);
            }
            if (depthUI) {
                depthUI.SetActive(on);
            }
            if (flukingUI) {
                flukingUI.SetActive(on);
            }
            if (toggleDragBtn) {
                toggleDragBtn.gameObject.SetActive(on);
            }
            if (toggleStressBtn) {
                toggleStressBtn.gameObject.SetActive(on);
            }
            if (togglePathBtn) {
                togglePathBtn.gameObject.SetActive(on);
            }
            if (settingsBtn) {
                settingsBtn.gameObject.SetActive(on);
            }
            
            // extern
            if (pubUIManager) {
                pubUIManager.ToggleUI(on);
            }
        }
    }

    public void SetPathVisibility(bool on) {
        whaleTrailMesh.enabled = on;
        togglePathBtnImage.sprite = togglePathBtnImage.sprite == togglePathSpriteOff ? togglePathSpriteOn : togglePathSpriteOff;
    }
    
    /**
     * Disable all interactable UI elements.
     */
    public void SetUIInteractivity(bool on) { // TODO: make sure can't click gear/whale
        if (toggleUIBtn) {
            toggleUIBtn.interactable = on;
        }
        if (toggleUIAltBtn) {
            toggleUIAltBtn.interactable = on;
        }
        if (toggleDragBtn) {
            toggleDragBtn.interactable = on;
        }
        if (toggleStressBtn) {
            toggleStressBtn.interactable = on;
        }
        if (togglePathBtn) {
            togglePathBtn.interactable = on;
        }
        if (settingsBtn) {
            settingsBtn.interactable = on;
        }
        
        // extern
        if (pubUIManager) {
            pubUIManager.SetUIInteractivity(on);
        }

        if (scrubber) {
            scrubber.ToggleScrubberInteractivity(on);
        }
    }

    public void SetCursorLock(bool on) {
        currCursorState = on ? CursorLockMode.Locked : CursorLockMode.None;
    }
}

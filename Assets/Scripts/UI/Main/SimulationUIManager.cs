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
        depthGraph =  GameObject.Find("DepthGraph").GetComponent<GraphRenderer>(); // TODO: rem
        return;
        GameObject check = GameObject.Find("FPS");
        if (check) {
            fpsText = check.GetComponent<TextMeshProUGUI>();
        }
        forcesListUI = GameObject.Find("ForcesListUI");
        headingBallUI = GameObject.Find("HeadingBallUI");
        //depthUI = GameObject.Find("DepthUI"); (TODO)
        flukingUI = GameObject.Find("FlukingUI");
        check = GameObject.Find("WhaleTrail");
        if (check) {
            whaleTrailMesh = check.GetComponent<MeshRenderer>();
        }
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
        check = GameObject.Find("DepthGraph");
        if (check) {
            depthGraph =  check.GetComponent<GraphRenderer>();
        }
        check = GameObject.Find("DepthRollingGraph");
        if (check) {
            depthRollingGraph = check.GetComponent<GraphRenderer>();
        }
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
        check = GameObject.Find("ScrubberUI");
        if (check) {
            scrubber =  check.GetComponent<Scrubber>();
        }
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
        int[] cols = { 0, 1, 8, 9, 10, 12, 14, 23 };
        string[] lines = data.text.Split(new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        int count = 0;
        for (int x = 1; x < lines.Length; x++) { // TODO: y 1?
            string[] values = lines[x].Split(',');
            if (values.Length >= 1) {
                count++;
            }
        }
        float[] depth = new float[count];
        float[] time = new float[count];
        count = 0;
        for (int x = 1; x < lines.Length; x++) {
            string[] values = lines[x].Split(',');
            if (values.Length >= 1) {
                depth[count] = float.Parse(values[cols[1]]);
                time[count] = float.Parse(values[cols[0]]);
                count++;
            }
        }

        if (depthGraph) {
            depthGraph.SetData(time, depth);
        }
        if (depthRollingGraph) {
            depthRollingGraph.SetData(time, depth);
        }
        if (flukingGraph) {
            flukingGraph.SetData(time, depth);
        }
        if (flukingRollingGraph) {
            flukingRollingGraph.SetData(time, depth);
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
        if (whaleTrailMesh) {
            whaleTrailMesh.enabled = on;
        }
        if (togglePathBtn) {
            togglePathBtnImage.sprite = togglePathBtnImage.sprite == togglePathSpriteOff ? togglePathSpriteOn : togglePathSpriteOff;
        }
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

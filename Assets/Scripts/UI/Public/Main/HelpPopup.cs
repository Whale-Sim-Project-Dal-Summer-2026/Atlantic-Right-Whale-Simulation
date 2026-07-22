using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HelpPopup : MonoBehaviour {
    // objs
    public Button helpBtn;
    public GameObject helpPopup;
    public Button[] closeBtns;
    
    // extern
    private SimulationUIManager simUIManager;
    private Scrubber scrubber;

    // vars
    private bool pauseOn = false;
    private bool open = false;
    
    void Awake() {
        // get refs
        simUIManager = GameObject.Find("UI").GetComponent<SimulationUIManager>();
        GameObject check = GameObject.Find("ScrubberUI");
        if (check) {
            scrubber =  check.GetComponent<Scrubber>();
        }
    }

    void Start() {
        // set up btns
        helpBtn.onClick.AddListener(() => SetHelpPopupVisibility(true));
        for (int x = 0; x < closeBtns.Length; x++) {
            closeBtns[x].onClick.AddListener(() => SetHelpPopupVisibility(false));
        }
    }
    
    public void SetHelpPopupVisibility(bool on) {
        if (scrubber && on) {
            pauseOn = scrubber.IsPaused();
        }
        open = on;
        helpPopup.SetActive(on);
        simUIManager.SetUIInteractivity(!on);
        if (scrubber && on && !pauseOn) {
            scrubber.SetPause(true);
        }
        if (scrubber && !on && !pauseOn) {
            scrubber.SetPause(false);
        }
        if (scrubber && !on) {
            pauseOn = false;
        }
    }

    public bool IsOpen() {
        return open;
    }
}
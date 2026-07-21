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
    private bool pauseOn;
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
    
    public void SetHelpPopupVisibility(bool isOpen) {
        if (scrubber && isOpen) {
            pauseOn = scrubber.IsPaused();
        }
        open = isOpen;
        helpPopup.SetActive(isOpen);
        simUIManager.SetUIInteractivity(!isOpen);
        if (scrubber && isOpen && !pauseOn) {
            scrubber.SetPause(true);
        }
        if (scrubber && !isOpen && !pauseOn) {
            scrubber.SetPause(false);
            pauseOn = true; // so that if called without opening again it wont set it to pause
        }
    }

    public bool IsOpen() {
        return open;
    }
}
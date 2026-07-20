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
        scrubber = GameObject.Find("ScrubberUI").GetComponent<Scrubber>();
    }

    void Start() {
        // set up btns
        helpBtn.onClick.AddListener(() => SetHelpPopupVisibility(true));
        for (int x = 0; x < closeBtns.Length; x++) {
            closeBtns[x].onClick.AddListener(() => SetHelpPopupVisibility(false));
        }
    }
    
    public void SetHelpPopupVisibility(bool isOpen) {
        if (isOpen) {
            pauseOn = scrubber.IsPaused();
        }
        open = isOpen;
        helpPopup.SetActive(isOpen);
        simUIManager.SetUIInteractivity(!isOpen);
        if (scrubber && !pauseOn) {
            scrubber.TogglePause();
        }
    }

    public bool IsOpen() {
        return open;
    }
}
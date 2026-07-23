using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

enum PopupState
{
    OPEN,
    CLOSED
}

public class HelpPopup : MonoBehaviour {
    // objs
    [SerializeField] Button helpBtn;
    [SerializeField] GameObject helpPopup;
    [SerializeField] Button[] closeBtns;
    // extern
    [SerializeField] SimulationUIManager simUIManager;
    private Scrubber scrubber;

    PopupState currState;

    public delegate void PopupSwapStateEvent();

    public static event PopupSwapStateEvent OnSwapStates;
    
    void Awake() {
        currState = PopupState.CLOSED;
    }

    void OnEnable(){
        PublicUIManager.OnMenuToggle += setButtonVisibility;
    }

    void OnDisable(){
        PublicUIManager.OnMenuToggle -= setButtonVisibility;
    }

    void setButtonVisibility(bool visible){
        helpBtn.gameObject.SetActive(visible);
    }



    void helpPress(){
        bool currVisibility = currState == PopupState.OPEN;
        SetHelpPopupVisibility(!currVisibility);
    }

    void Start() {
        // set up btns
        helpBtn.onClick.AddListener(helpPress);
        for (int x = 0; x < closeBtns.Length; x++) {
            closeBtns[x].onClick.AddListener(() => SetHelpPopupVisibility(false));
        }
    }
    
    void swapState(bool on){
        currState = on ? PopupState.OPEN : PopupState.CLOSED;
        OnSwapStates?.Invoke();
    }

    void togglePopUp(){
        switch (currState)
        {
            case (PopupState.OPEN):{
                helpPopup.SetActive(true);
                simUIManager.SetUIInteractivity(false);
                break;
            }
            
            case (PopupState.CLOSED):{
                helpPopup.SetActive(false);
                simUIManager.SetUIInteractivity(true);
                break;
            }
        }
    }

    void SetHelpPopupVisibility(bool on) {
        swapState(on);
        togglePopUp();
    }
}
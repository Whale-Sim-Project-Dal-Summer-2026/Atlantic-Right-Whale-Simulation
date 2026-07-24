using System;
using System.Collections.Generic;
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
    [SerializeField] private Button helpBtn;
    [SerializeField] private GameObject helpPopup;
    [SerializeField] private Button[] closeBtns;
    // extern
    [SerializeField] SimulationUIManager simUIManager; // TODO: not good dependency
    PopupState currState;

    InputAction helpAction;
    InputAction backAction;
    double lastPressTime;
// MS between presses
    float pressBuffer = 300;
    public delegate void PopupSwapStateEvent();

    public static event PopupSwapStateEvent OnSwapStates;

    List<InputAction> actionsToListenFor;
    

    void Start() {
        // set up btns
        helpBtn.onClick.AddListener(helpPress);
        for (int x = 0; x < closeBtns.Length; x++) {
            closeBtns[x].onClick.AddListener(() => SetHelpPopupVisibility(false));
        }

        lastPressTime = Time.time;
        currState = PopupState.CLOSED;
        helpAction = InputSystem.actions.FindAction("OpenHelp");
        backAction = InputSystem.actions.FindAction("BackAction");

        actionsToListenFor = new List<InputAction>();
        actionsToListenFor.Add(helpAction);
    }

    bool checkActions(){
        bool interaction = false;

        foreach(InputAction action in actionsToListenFor){
            interaction = (action?.ReadValue<float>() ?? 0.0f) == 1.0f;
            if(interaction) break;
        }

        return interaction;

    }

    void registerPress(){
        double currTime = Time.unscaledTimeAsDouble * 1000;

        if (currTime - lastPressTime > pressBuffer){
            helpPress();
            lastPressTime = currTime;
        } 
    }

    void Update()
    {
        bool interaction = checkActions();

        if(!interaction) return;

        registerPress();

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
                actionsToListenFor.Add(backAction);
                break;
            }
            
            case (PopupState.CLOSED):{
                helpPopup.SetActive(false);
                simUIManager.SetUIInteractivity(true);
                actionsToListenFor.Remove(backAction);
                break;
            }
        }
    }

    void SetHelpPopupVisibility(bool on) {
        swapState(on);
        togglePopUp();
    }
}
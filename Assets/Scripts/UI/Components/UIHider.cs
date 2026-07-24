using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public enum UiHideState
{
    SHOWING,
    HIDEN
}

public class UIHider : MonoBehaviour{
    InputAction hideAction;

    float pressBuffer = 300;

    double lastPressTime;

    UiHideState currState;

    [SerializeField] PublicUIManager publicUIManager;

    void Start(){
        currState = UiHideState.SHOWING;
        hideAction = InputSystem.actions.FindAction("HideUI");
    }


    void toggleBasedOnState(){
        switch (currState)
        {
            case (UiHideState.SHOWING):{
                publicUIManager.ToggleUI(true);
                break;
            }
            case (UiHideState.HIDEN):{
                publicUIManager.ToggleUI(false);
                break;       
            }
        }
    }

    bool validPress(){
        double currTime = Time.unscaledTimeAsDouble * 1000;

        if (currTime - lastPressTime > pressBuffer){
            lastPressTime = currTime;
            return true;
        }
        return false;
    }

    void cycleState(){
        currState = currState == UiHideState.SHOWING ? UiHideState.HIDEN : UiHideState.SHOWING;
    }

    void Update(){
        bool interacted = (hideAction?.ReadValue<float>() ?? 0.0f) == 1.0f;

        if(!interacted) return;

        if (validPress()){
            cycleState();
            toggleBasedOnState();
        }
    }
}
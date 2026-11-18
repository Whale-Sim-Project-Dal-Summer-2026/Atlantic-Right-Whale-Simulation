/**
 * StateManager.cs: Script which keeps track of states.
 *
 * @author Mars Semenova
 */

using UnityEngine;
using UnityEngine.Serialization;

public enum StateInput { // TODO: nuke
    ENABLED, // all input valid
    POPUP, // input disabled except back btn + popup btn
    NOUI, // input disabled except toggle UI btn
    IDLE // some input disabled
}

public enum StateUI {
    SHOW,
    HIDE
}

public enum StateHelpPopup {
    SHOW,
    HIDE
}

public enum StateIdle {
    ON,
    OFF
}

public enum StateDebug {
    ON,
    OFF
}

public class StateManager : MonoBehaviour {
    // states
    private static StateInput stateInput = StateInput.ENABLED;
    private static StateUI stateUI = StateUI.SHOW;
    private static StateHelpPopup stateHelpPopup = StateHelpPopup.HIDE;
    private static StateIdle stateIdle = StateIdle.OFF;
    [SerializeField] private StateDebug stateDebugVal = StateDebug.OFF;
    private static StateDebug stateDebug;
    
    void Start() {
        // transfer editor input to vars
        stateDebug = stateDebugVal;
        
        // sub
        PopupManager.OnHelpPopup += SetStatePopup; // TODO: nuke
        PopupManager.OnHelpPopup += SetStateHelpPopup;
        TogglesManager.OnToggleUI += SetStateNoUI; // TODO: nuke
        TogglesManager.OnToggleUI += SetStateUI;
        IdleUI.OnIdle += SetStateIdle;
    }
    
    private void OnDestroy() {
        // unsub
        PopupManager.OnHelpPopup -= SetStatePopup;
        TogglesManager.OnToggleUI -= SetStateNoUI;
        TogglesManager.OnToggleUI -= SetStateUI;
    }
    
    /**
     * Sets the current state to InputState.NOUI.
     * @param on - Whether the state should be in the no UI state or not.
     */
    private void SetStateNoUI(bool on) { // TODO: nuke
        stateInput = !on ? StateInput.NOUI : StateInput.ENABLED;
    }
    
    /**
     * Sets the current state to InputState.POPUP.
     * @param on - Whether the state should be in the popup state or not.
     */
    private void SetStatePopup(bool on) { // TODO: nuke
        stateInput = on ? StateInput.POPUP : StateInput.ENABLED;
    }
    
    // setters
    public static void SetStateUI(bool on) {
        stateUI = on ? StateUI.SHOW : StateUI.HIDE;
        if (stateDebug == StateDebug.ON) {
            print("stateUI set to " + (stateUI == StateUI.SHOW ? "SHOW" : "HIDE"));
        }
    } 
    public void SetStateIdle(bool on) { 
        stateInput = !on ? StateInput.NOUI : StateInput.ENABLED;
        if (stateDebug == StateDebug.ON) {
            print("stateIdle set to " + (stateIdle == StateIdle.ON ? "ON" : "OFF"));
        }
    }
    public static void SetStateHelpPopup(bool on) {
        stateHelpPopup = on ? StateHelpPopup.SHOW : StateHelpPopup.HIDE;
        if (stateDebug == StateDebug.ON) {
            print("stateHelpPopup set to " + (stateHelpPopup == StateHelpPopup.SHOW ? "SHOW" : "HIDE"));
        }
    } 
    
    // getters
    public static StateInput GetStateInput() { // TODO: nuke
        return stateInput;
    }
    public static StateUI GetStateUI() {
        return stateUI;
    }
    public static StateIdle GetStateIdle() {
        return stateIdle;
    }
    public static StateDebug GetStateDebug() {
        return stateDebug;
    }
}

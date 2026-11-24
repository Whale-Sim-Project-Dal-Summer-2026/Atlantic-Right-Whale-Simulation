/**
 * StateManager.cs: Script which keeps track of states.
 *
 * @author Mars Semenova
 */

using UnityEngine;
using UnityEngine.Serialization;

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

public enum StateTutorial {
    YES,
    NO
}

public class StateManager : MonoBehaviour {
    // states
    private static StateUI stateUI = StateUI.SHOW;
    private static StateHelpPopup stateHelpPopup = StateHelpPopup.HIDE;
    private static StateIdle stateIdle = StateIdle.OFF;
    [SerializeField] private StateDebug stateDebugVal = StateDebug.OFF;
    private static StateDebug stateDebug;
    private static StateTutorial stateTutorial = StateTutorial.NO;
    
    void Start() {
        // transfer editor input to vars
        stateDebug = stateDebugVal;
        
        // sub
        PopupManager.OnHelpPopup += SetStateHelpPopup;
        TogglesManager.OnToggleUI += SetStateUI;
        TutorialDisplay.OnTutorial += SetStateTutorial;
    }
    
    private void OnDestroy() {
        // unsub
        PopupManager.OnHelpPopup -= SetStateHelpPopup;
        TogglesManager.OnToggleUI -= SetStateUI;
        TutorialDisplay.OnTutorial -= SetStateTutorial;
    }
    
    // setters
    public static void SetStateUI(bool on) {
        stateUI = on ? StateUI.SHOW : StateUI.HIDE;
        if (stateDebug == StateDebug.ON) {
            print("stateUI set to " + (stateUI == StateUI.SHOW ? "SHOW" : "HIDE"));
        }
    } 
    public static void SetStateIdle(bool on) { 
        stateIdle = on ? StateIdle.ON : StateIdle.OFF;
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
    
    public static void SetStateTutorial(bool on) {
        stateTutorial = on ? StateTutorial.YES : StateTutorial.NO;
        if (stateDebug == StateDebug.ON) {
            print("stateTutorial set to " + (stateTutorial == StateTutorial.YES ? "YES" : "NO"));
        }
    } 
    
    // getters
    public static StateUI GetStateUI() {
        return stateUI;
    }
    public static StateIdle GetStateIdle() {
        return stateIdle;
    }
    public static StateHelpPopup GetStateHelpPopup() {
        return stateHelpPopup;
    }
    public static StateDebug GetStateDebug() {
        return stateDebug;
    }
    public static StateTutorial GetStateTutorial() {
        return stateTutorial;
    }
}

/**
 * SimulationUIManager.cs: Script which handles
 * the simulation UI.
 *
 * @author Mars Semenova 
 */

using UnityEngine;

public class SimulationUIManager : MonoBehaviour {
    // params
    // scripts
    [Header("Scripts")]
    [SerializeField] private Scrubber scrubber;
    [SerializeField] private TogglesManager toggles;
    
    // events

    // use only 1 event for these 2, pass bool into it to say wheter it should be on or off
    public delegate void DisableInteractivityEvent();
    public static event DisableInteractivityEvent OnInteractivityDisabled;
    public delegate void EnableInteractivityEvent();
    public static event EnableInteractivityEvent OnInteractivityEnabled;
    
    void Awake() {
        // toggle UI interactivity when help popup is toggled
        PopupManager.OnHelpPopupOn += SetUIInteractivityOff;
        PopupManager.OnHelpPopupOff += SetUIInteractivityOn;
    }

    /**
     * Disable all interactable UI elements.
     * @param on - Whether the elements should be disabled.
     */
    private void SetUIInteractivity(bool on) { 
        if (on) {
            OnInteractivityEnabled?.Invoke();
        } else {
            OnInteractivityDisabled?.Invoke();
        }
        if (scrubber) {
            scrubber.SetScrubberInteractivity(on);
        }

        if (toggles) {
            toggles.SetTogglesInteractivity(on);
        }
    }
    public void SetUIInteractivityOn() { // TODO
        SetUIInteractivity(true);
    }
    public void SetUIInteractivityOff() { // TODO
        SetUIInteractivity(false);
    }
}

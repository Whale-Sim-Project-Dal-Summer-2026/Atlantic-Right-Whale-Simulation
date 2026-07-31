/**
 * SimulationUIManager.cs: Script which handles
 * the simulation UI.
 *
 * @author Mars Semenova 
 */

using System;
using UnityEngine;

public class SimulationUIManager : MonoBehaviour {
    // params
    // scripts
    [Header("Scripts")]
    [SerializeField] private Scrubber scrubber;
    [SerializeField] private TogglesManager toggles;
    
    // events

    // use only 1 event for these 2, pass bool into it to say wheter it should be on or off
    public delegate void ToggleInteractivityEvent(bool on);
    public static event ToggleInteractivityEvent OnToggleInteractivity;
    
    void Awake() {
        // toggle UI interactivity when help popup is toggled
        PopupManager.OnHelpPopup += SetUIInteractivityOnHelpPopup;
    }

    private void OnDestroy() {
        // unsub
        PopupManager.OnHelpPopup -= SetUIInteractivityOnHelpPopup;
    }

    /**
     * Disable all interactable UI elements.
     * @param on - Whether the elements should be disabled.
     */
    public void SetUIInteractivity(bool on) { 
        OnToggleInteractivity?.Invoke(on);
        if (scrubber) {
            scrubber.SetScrubberInteractivity(on);
        }

        if (toggles) {
            toggles.SetTogglesInteractivity(on);
        }
    }
    
    /**
     * A method which inverts the bool passed by the OnHelpPopup event
     * before calling the set UI interactivity function.
     * @param on - Whether the popup is on or off.
     */
    private void SetUIInteractivityOnHelpPopup(bool on) { 
        SetUIInteractivity(!on);
    }
}

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
    [SerializeField] private Toggles toggles;
    // viewer colliders
    [Header("Viewer Colliders to Disable when Hiding UI")]
    [SerializeField] private Collider[] viewerColliders;
    
    // events
    public delegate void DisableInteractivityEvent();
    public static event DisableInteractivityEvent OnInteractivityDisabled;
    public delegate void EnableInteractivityEvent();
    public static event EnableInteractivityEvent OnInteractivityEnabled;
    
    void Awake() {
        // toggle UI interactivity when help popup is toggled
        PopupEventManager.OnHelpPopupOn += SetUIInteractivityOff;
        PopupEventManager.OnHelpPopupOff += SetUIInteractivityOn;
        // toggle viewer collider interactivity on UI toggle
        Toggles.OnToggleUIOn += SetViewerCollidersInteractivityOn;
        Toggles.OnToggleUIOff += SetViewerCollidersInteractivityOff;
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

    /**
     * Set viewer colliders interactivity. Used when UI is disabled.
     * @param on - Whether to disable the colliders.
     */
    private void SetViewerCollidersInteractivity(bool on) {
        for (int x = 0; x < viewerColliders.Length; x++) {
            viewerColliders[x].enabled = on;
        }
    }
    private void SetViewerCollidersInteractivityOn() { // TODO
        SetViewerCollidersInteractivity(true);
    }
    private void SetViewerCollidersInteractivityOff() { // TODO
        SetViewerCollidersInteractivity(false);
    }
}

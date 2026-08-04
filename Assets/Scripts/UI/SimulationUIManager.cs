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
    [SerializeField] private CameraController camController;
    
    // events
    public delegate void ToggleInteractivityEvent(bool on);
    public static event ToggleInteractivityEvent OnToggleInteractivity;

    void Start() {
        // sub
        Scrubber.OnCamSwitch += camController.changeToCam;
    }

    private void OnDestroy() {
        // unsub
        Scrubber.OnCamSwitch -= camController.changeToCam;
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
}

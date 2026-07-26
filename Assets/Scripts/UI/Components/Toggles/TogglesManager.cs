/**
 * Manager.cs: Script which handles the toggle buttons.
 *
 * @author Mars Semenova 
 */

using System;
using UnityEngine;
using UnityEngine.UI;

public class TogglesManager : MonoBehaviour {
    // params
    // toggles
    [Header("Scripts")] 
    [SerializeField] private Toggles toggles;
    [Header("Toggles")]
    [SerializeField] private Button toggleUIBtn;
    [SerializeField] private Button toggleDragBtn;
    [SerializeField] private Button toggleStressBtn;
    [SerializeField] private Button togglePathBtn;
    // path params
    [Header("Path Parameters")]
    [SerializeField] private WhaleTrail whaleTrail;
    
    // events
    public delegate void ToggleUIOnEvent();
    public static event ToggleUIOnEvent OnToggleUIOn;
    public delegate void ToggleUIOffEvent();
    public static event ToggleUIOffEvent OnToggleUIOff;
    public delegate void TogglePathOnEvent();
    public static event TogglePathOnEvent OnTogglePathOn;
    public delegate void TogglePathOffEvent();
    public static event TogglePathOffEvent OnTogglePathOff;

    void Awake() {
        Toggles.OnToggleOn += ToggleOnEvent;
        Toggles.OnToggleOff += ToggleOffEvent;
    }

    /**
     * Invoke events for when a toggle is clicked.
     * @param label - Name of a toggle's GameObject passed through the generic toggle's events.
     * @param on - Whether the toggle was toggled on or off.
     */
    private void ToggleEvent(String label, bool on) {
        if (toggleUIBtn && label == toggleUIBtn.gameObject.name) {
            SetUIVisibility(on);
        }
        if (toggleDragBtn && label == toggleDragBtn.gameObject.name) {
            // TODO
        }
        if (toggleStressBtn && label == toggleStressBtn.gameObject.name) {
            // TODO
        }
        if (togglePathBtn && label == togglePathBtn.gameObject.name) {
            SetPathVisibility(on);
        }
    }
    private void ToggleOnEvent(String label) { // TODO
        ToggleEvent(label, true);
    }
    private void ToggleOffEvent(String label) { // TODO
        ToggleEvent(label, false);
    }

    /**
     * Toggle UI visibility.
     * @param on - Whether the UI should be visible or not.
     */
    public void SetUIVisibility(bool on) {
        if (on) {
            OnToggleUIOn?.Invoke();
        } else {
            OnToggleUIOff?.Invoke();
        }
    }

    /**
     * Toggle path visibility,
     * @param on - Whether the path should be visible or not.
     */
    public void SetPathVisibility(bool on) {
        if (on) {
            OnTogglePathOn?.Invoke();
        } else {
            OnTogglePathOff?.Invoke();
        }
    }

    /**
     * Set the interactivity of buttons. Connection to generic toggles functionality.
     * @param on - Whether the buttons should be interactive or not.
     */
    public void SetTogglesInteractivity(bool on) {
        toggles.SetTogglesInteractivity(on);
    }
}

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

    // dont need 2 events, have 1 event, and pass either true or false into them
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

    //  should likely just pass a gameobject into this
    private void ToggleEvent(GameObject obj, bool on) {
        if (toggleUIBtn && obj == toggleUIBtn.gameObject) {
            SetUIVisibility(on);
        }
        if (toggleDragBtn && obj == toggleDragBtn.gameObject) {
            // TODO
        }
        if (toggleStressBtn && obj == toggleStressBtn.gameObject) {
            // TODO
        }
        if (togglePathBtn && obj == togglePathBtn.gameObject) {
            SetPathVisibility(on);
        }
    }
    private void ToggleOnEvent(GameObject obj) { // TODO
        ToggleEvent(obj, true);
    }
    private void ToggleOffEvent(GameObject obj) { // TODO
        ToggleEvent(obj, false);
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
        if (toggles) {
            toggles.SetTogglesInteractivity(on);
        }
    }
}

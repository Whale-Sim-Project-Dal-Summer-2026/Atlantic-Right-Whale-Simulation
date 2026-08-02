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
    public delegate void ToggleUIEvent(bool on);
    public static event ToggleUIEvent OnToggleUI;
    public delegate void TogglePathEvent(bool on);
    public static event TogglePathEvent OnTogglePath;
    
    // vars
    private RectTransform toggleUIRect;
    private Vector3 ogToggleUIPos;
    private Vector3 newToggleUIPos;

    void Awake() {
        // get init pos
        toggleUIRect = toggleUIBtn.gameObject.GetComponent<RectTransform>();
        ogToggleUIPos = toggleUIRect.transform.localPosition;
        newToggleUIPos =  new Vector3(393.3f, ogToggleUIPos.y, ogToggleUIPos.z);
        
        // add to events
        Toggles.OnToggle += ToggleEvent;
    }

    /**
     * Invoke events for when a toggle is clicked.
     * @param label - Name of a toggle's GameObject passed through the generic toggle's events.
     * @param on - Whether the toggle was toggled on or off.
     */

    //  should likely just pass a gameobject into this
    private void ToggleEvent(bool on, GameObject obj) {
        if (toggleUIBtn && obj == toggleUIBtn.gameObject) {
            toggleUIRect.transform.localPosition = on ? ogToggleUIPos : newToggleUIPos;
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

    /**
     * Toggle UI visibility.
     * @param on - Whether the UI should be visible or not.
     */
    public void SetUIVisibility(bool on) {
        OnToggleUI?.Invoke(on);
    }

    /**
     * Toggle path visibility,
     * @param on - Whether the path should be visible or not.
     */
    public void SetPathVisibility(bool on) {
        OnTogglePath?.Invoke(on);
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

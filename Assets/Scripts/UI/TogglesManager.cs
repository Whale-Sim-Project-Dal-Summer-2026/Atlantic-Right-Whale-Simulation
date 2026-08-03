/**
 * Manager.cs: Script which handles the toggle buttons.
 *
 * @author Mars Semenova 
 */

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
    private bool isUIOn = true;

    void Awake() {
        // get init pos
        toggleUIRect = toggleUIBtn.gameObject.GetComponent<RectTransform>();
        ogToggleUIPos = toggleUIRect.transform.localPosition;
        newToggleUIPos =  new Vector3(393.3f, ogToggleUIPos.y, ogToggleUIPos.z);
        
        // add to events
        Toggles.OnToggle += ToggleEvent;
    }

    private void OnDestroy() {
        // unsub
        Toggles.OnToggle -= ToggleEvent;
    }

    /**
     * Invoke events for when a toggle is clicked.
     * @param label - Name of a toggle's GameObject passed through the generic toggle's events.
     * @param on - Whether the toggle was toggled on or off.
     */

    //  should likely just pass a gameobject into this
    private void ToggleEvent(bool on, GameObject obj) {
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

    /**
     * Toggle UI visibility.
     * @param on - Whether the UI should be visible or not.
     */
    private void SetUIVisibility(bool on) {
        isUIOn = on;
        toggleUIRect.transform.localPosition = on ? ogToggleUIPos : newToggleUIPos;
        OnToggleUI?.Invoke(on);
    }

    /**
     * Check whether the UI is visible
     * @return Whether the UI is visible.
     */
    public bool IsUIVisible() {
        return isUIOn;
    }
    
    /**
     * Toggle path visibility,
     * @param on - Whether the path should be visible or not.
     */
    private void SetPathVisibility(bool on) {
        OnTogglePath?.Invoke(on);
    }
}

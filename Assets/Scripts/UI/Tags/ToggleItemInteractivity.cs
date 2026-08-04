/**
 * ToggleItemInteractivity.cs: Used to toggle the interactivity
 * of listed buttons and/or colliders when the interactivity is toggled via events.
 *
 * @author Mars Semenova
 */

using UnityEngine;
using UnityEngine.UI;

public class ToggleItemInteractivity : MonoBehaviour {
    private Button btn;
    private Collider collider;
    
    void Awake() {
        // get refs
        btn = GetComponent<Button>();
        collider = GetComponent<Collider>();
        
        // add func to events
        SimulationUIManager.OnToggleInteractivity += SetInteractivity;
        TogglesManager.OnToggleUI += SetColliderInteractivity;
    }

    private void OnDestroy() {
        // unsub
        SimulationUIManager.OnToggleInteractivity -= SetInteractivity;
        TogglesManager.OnToggleUI -= SetColliderInteractivity;
    }

    /**
     * Toggle the interactivity of the buttons and/or colliders.
     * @param on - Whether the colliders should be interactable or not.
     */
    private void SetInteractivity(bool on) {
        if (btn) {
            btn.interactable = on;
        }
        SetColliderInteractivity(on);
    }
    
    /**
     * Toggle interactivity of colliders.
     * @param on - Whether the colliders should be interactable or not.
     */
    private void SetColliderInteractivity(bool on) {
        if (collider) {
            collider.enabled = on;
        }
    }
}
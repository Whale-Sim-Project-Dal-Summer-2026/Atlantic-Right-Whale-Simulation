/**
 * ToggleItemVisibility.cs: Used to toggle the visibility
 * of the object this script is attached to when the UI visibility is toggled
 * via events.
 *
 * @author Mars Semenova
 */

using UnityEngine;

public class ToggleItemVisibility : MonoBehaviour {
    void Awake() {
        TogglesManager.OnToggleUI += SetVisibility;
    }

    private void OnDestroy() {
        // unsub
        TogglesManager.OnToggleUI -= SetVisibility;
    }

    /**
     * Toggle visibility.
     * @param on - Whether the object should be visible or not.
     */
    private void SetVisibility(bool on) {
        gameObject.SetActive(on);
    }
}
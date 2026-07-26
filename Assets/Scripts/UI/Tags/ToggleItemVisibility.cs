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
        TogglesManager.OnToggleUIOn += SetVisibilityOn;
        TogglesManager.OnToggleUIOff += SetVisibilityOff;
    }

    /**
     * Turn visibility on.
     */
    private void SetVisibilityOn() {
        gameObject.SetActive(true);
    }
    
    /**
     * Turn visibility off.
     */
    private void SetVisibilityOff() {
        gameObject.SetActive(false);
    }
}
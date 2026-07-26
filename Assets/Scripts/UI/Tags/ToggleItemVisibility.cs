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
        TogglesManager.OnToggleUIOn += ToggleVisibilityOn;
        TogglesManager.OnToggleUIOff += ToggleVisibilityOff;
    }

    /**
     * Turn visibility on.
     */
    private void ToggleVisibilityOn() {
        gameObject.SetActive(true);
    }
    
    /**
     * Turn visibility off.
     */
    private void ToggleVisibilityOff() {
        gameObject.SetActive(false);
    }
}
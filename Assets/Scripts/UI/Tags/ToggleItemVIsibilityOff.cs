/**
 * ToggleItemVisibilityOff.cs: Used to force the visibility
 * of the object this script is attached to off when the UI visibility is toggled
 * via events.
 *
 * @author Mars Semenova
 */

using UnityEngine;

public class ToggleItemVisibilityOff : MonoBehaviour {
    void Awake() {
        TogglesManager.OnToggleUIOn += ToggleVisibilityOff;
        TogglesManager.OnToggleUIOff += ToggleVisibilityOff;
    }

    /**
     * Turn visibility off.
     */
    private void ToggleVisibilityOff() {
        gameObject.SetActive(false);
    }
}
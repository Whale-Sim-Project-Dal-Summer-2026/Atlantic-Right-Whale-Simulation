/**
 * ToggleItemVisibilityOff.cs: Used to force the visibility
 * of the object this script is attached to off when the UI visibility is toggled
 * via events.
 *
 * @author Mars Semenova
 */

using System;
using UnityEngine;

public class ToggleItemVisibilityOff : MonoBehaviour {
    void Awake() {
        TogglesManager.OnToggleUI += SetVisibilityOff;
    }

    private void OnDestroy() {
        // unsub
        TogglesManager.OnToggleUI -= SetVisibilityOff;
    }

    /**
     * Turn visibility off.
     * @param on - Boolean passed by OnToggleUI event. Not used.
     */
    private void SetVisibilityOff(bool on) {
        gameObject.SetActive(false);
    }
}
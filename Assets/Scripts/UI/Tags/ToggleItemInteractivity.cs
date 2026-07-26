/**
 * ToggleItemInteractivity.cs: Used to toggle the interactivity
 * of listed buttons and/or colliders when the interactivity is toggled via events.
 *
 * @author Mars Semenova
 */

using UnityEngine;
using UnityEngine.UI;

public class ToggleItemInteractivity : MonoBehaviour {
    [Header("Buttons")]
    [SerializeField] private Button[] btns;
    [Header("Colliders")]
    [SerializeField] private Collider[] colliders;
    
    void Awake() {
        SimulationUIManager.OnInteractivityEnabled += ToggleInteractivityOn;
        SimulationUIManager.OnInteractivityDisabled += ToggleInteractivityOff;
    }

    /**
     * Make buttons and/or colliders interactable.
     */
    private void ToggleInteractivityOn() {
        for (int x = 0; x < btns.Length; x++) {
            btns[x].interactable = true;
        }
        for (int x = 0; x < colliders.Length; x++) {
            colliders[x].enabled = true;
        }
    }
    
    /**
     * Make buttons and/or colliders not interactable.
     */
    private void ToggleInteractivityOff() {
        for (int x = 0; x < btns.Length; x++) {
            btns[x].interactable = false;
        }
        for (int x = 0; x < colliders.Length; x++) {
            colliders[x].enabled = false;
        }
    }
}
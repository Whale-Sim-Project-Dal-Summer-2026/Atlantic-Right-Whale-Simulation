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
        SimulationUIManager.OnInteractivityEnabled += SetInteractivityOn;
        SimulationUIManager.OnInteractivityDisabled += SetInteractivityOff;
        TogglesManager.OnToggleUIOn += SetColliderInteractivityOn;
        TogglesManager.OnToggleUIOff += SetColliderInteractivityOff;
    }

    /**
     * Make buttons and/or colliders interactable.
     */
    private void SetInteractivityOn() {
        for (int x = 0; x < btns.Length; x++) {
            btns[x].interactable = true;
        }
        SetColliderInteractivityOn();
    }
    
    /**
     * Make buttons and/or colliders not interactable.
     */
    private void SetInteractivityOff() {
        for (int x = 0; x < btns.Length; x++) {
            btns[x].interactable = false;
        }
        SetColliderInteractivityOff();
    }
    
    /**
     * Make colliders interactable.
     */
    private void SetColliderInteractivityOn() {
        for (int x = 0; x < colliders.Length; x++) {
            colliders[x].enabled = true;
        }
    }
    
    /**
     * Make colliders not interactable.
     */
    private void SetColliderInteractivityOff() {
        for (int x = 0; x < colliders.Length; x++) {
            colliders[x].enabled = false;
        }
    }
}
/**
 * PopupManager.cs: Implements custom behaviour for the popups in the scene.
 *
 * @author Mars Semenova
 */

using System;
using UnityEngine;

public class PopupManager : MonoBehaviour {
    // params
    // help popup params
    [Header("Help Popup Parameters")]
    [SerializeField] private GameObject helpPopup;
    [SerializeField] private Scrubber scrubber;
    [SerializeField] private SimulationUIManager simUIManager;
    
    // events
    public delegate void HelpPopupOnEvent(); 
    public static event HelpPopupOnEvent OnHelpPopupOn;
    public delegate void HelpPopupOffEvent(); 
    public static event HelpPopupOffEvent OnHelpPopupOff;
    
    void Awake() {
        // attach methods to generic popup events
        Popup.OnPopupOn += PopupOnEvent;
        Popup.OnPopupOff += PopupOffEvent;
        
        // help events setup
        if (scrubber) {
            OnHelpPopupOn += scrubber.Pause;
        }
        OnHelpPopupOn += simUIManager.SetUIInteractivityOff;
        OnHelpPopupOff += simUIManager.SetUIInteractivityOn;
    }

    /**
     * Invoke events for when a popup is opened.
     * @param label - Name of a popup's GameObject passed through the generic popup's events.
     */
    private void PopupOnEvent(String label) {
        if (label == helpPopup.name) {
            OnHelpPopupOn?.Invoke();
        }
    }
    
    /**
     * Invoke events for when a popup is closed.
     * @param label - Name of a popup's GameObject passed through the generic popup's events.
     */
    private void PopupOffEvent(String label) {
        if (label == helpPopup.name) {
            OnHelpPopupOff?.Invoke();
        }
    }
}
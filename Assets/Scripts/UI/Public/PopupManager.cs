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
    // make this 1 event
    public delegate void HelpPopupEvent(bool on); 
    public static event HelpPopupEvent OnHelpPopup;
    
    void Awake() {
        // attach methods to generic popup events
        Popup.OnPopup += PopupEvent;
    }

    private void OnDestroy() {
        // unsub
        Popup.OnPopup -= PopupEvent;
    }

    /**
     * Invoke events for when a popup is opened or closed.
     * @param on - Whether the popup is on or off.
     * @param label - Name of a popup's GameObject passed through the generic popup's events.
     */
    private void PopupEvent(bool on, GameObject obj) {
        if (obj == helpPopup) {
            OnHelpPopup?.Invoke(on);
            if (on && scrubber) {
                scrubber.Pause();
            }
            simUIManager.SetUIInteractivity(!on);
        }
    }
}
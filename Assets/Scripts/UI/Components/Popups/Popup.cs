/**
 * Popup.cs: A generic implementation for a popup.
 * 
 * @author Mars Semenova
 */

using System;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour {
    // params
    [SerializeField] private Button[] popupBtns;
    [SerializeField] private GameObject popupObj;
    [SerializeField] private Button[] closeBtns;
    
    // events
    public delegate void PopupOnEvent(String label);
    public static event PopupOnEvent OnPopupOn;
    public delegate void PopupOffEvent(String label);
    public static event PopupOffEvent OnPopupOff;

    // vars
    private bool open = false;
    private String label;
    private bool invoked = false;

    void Awake() {
        // get label
        label = popupObj.name;
        
        // set up btns
        for (int x = 0; x < popupBtns.Length; x++) {
            popupBtns[x].onClick.AddListener(PopupBtnPressed);
        }
        for (int x = 0; x < closeBtns.Length; x++) {
            closeBtns[x].onClick.AddListener(() => SetPopupVisibility(false));
        }
    }

    void Update() {
        if (!popupObj.activeSelf && !invoked) { // if popup obj ever deactivated manually should set popup state to closed
            open = false;
            OnPopupOff?.Invoke(label);
            invoked = true;
        }

        if (popupObj.activeSelf) { // to prevent constant invoking
            invoked = false;
        }
    }

    /**
     * Implements popup functionality on popup btn press =.
     */
    public void PopupBtnPressed(){
        SetPopupVisibility(!open);
    }

    /**
     * Implements the toggling of the popup.
     * @param on - Whether the popup should be open or not.
     */
    public void SetPopupVisibility(bool on) {
        open = on;
        popupObj.SetActive(on);
        if (on) {
            OnPopupOn?.Invoke(label);
        } else {
            OnPopupOff?.Invoke(label);
        }
    }

    /**
     * Check whether the popup is open or not.
     * @return Whether the popup is open.
     */
    public bool IsOpen() {
        return open;
    }
}
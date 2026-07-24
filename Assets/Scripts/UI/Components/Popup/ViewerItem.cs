using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ViewerItem : MonoBehaviour {
    // refs
    public Collider collider;
    private ViewerPopup popup;
    
    //vars
    public Texture image;
    public String text;

    private void Awake() {
        popup = GameObject.Find("ViewerUI").GetComponent<ViewerPopup>();
    }

    void Update() {
        if (collider.bounds.IntersectRay(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue())) && Mouse.current.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked) {
            popup.SetViewerPopupVisibility(true, image, text);
        }
    }
}

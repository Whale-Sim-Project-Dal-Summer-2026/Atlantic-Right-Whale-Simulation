/**
 * ViewerItem.cs: Sets up parameters to be displayed in the viewer
 * popup on click on the set collider.
 *
 * @author Mars Semenova
 */

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ViewerItem : MonoBehaviour {
    // params
    // content
    [Header("Content")]
    [SerializeField] private Texture image;
    [TextArea]
    [SerializeField] private String text;
    // refs
    [Header("References")]
    [SerializeField] private Collider collider;
    
    // events
    public delegate void ShowViewerEvent(Texture img, String txt);
    public static event ShowViewerEvent OnShowViewer;

    void Update() {
        if (collider.bounds.IntersectRay(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue())) && Mouse.current.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked && !EventSystem.current.IsPointerOverGameObject()) {
            OnShowViewer?.Invoke(image, text);
        }
    }
}

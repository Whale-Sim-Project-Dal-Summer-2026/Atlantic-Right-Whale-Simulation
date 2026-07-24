using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ViewerPopup : MonoBehaviour {
    // objs
    public GameObject viewerPopup;
    public RawImage viewerImg;
    public TextMeshProUGUI viewerTxt;
    public Button[] closeBtns;
    
    // vars
    private bool pauseOn;
    private bool open = false;
    
    void Start() {
        // set up btns
        for (int x = 0; x < closeBtns.Length; x++) {
            closeBtns[x].onClick.AddListener(() => SetViewerPopupVisibility(false));
        }
    }
    
    public void SetViewerPopupVisibility(bool isOpen) {
        open = isOpen;
        viewerPopup.SetActive(isOpen);
    }
    
    public void SetViewerPopupVisibility(bool isOpen, Texture img, String txt) {
        viewerImg.texture = img;
        viewerTxt.text = txt;
        SetViewerPopupVisibility(isOpen);
    }

    public bool IsOpen() {
        return open;
    }
}
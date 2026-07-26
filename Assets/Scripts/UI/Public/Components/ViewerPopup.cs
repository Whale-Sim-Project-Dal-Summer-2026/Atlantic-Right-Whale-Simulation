/**
 * ViewerPopup.cs: Implements the viewer popup functionality.
 * 
 * @author Mars Semenova
 */

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ViewerPopup : MonoBehaviour {
    // params
    // scripts
    [Header("Scripts")]
    [SerializeField] private Popup viewerPopup;
    // objs for content
    [Header("Content")]
    [SerializeField] private RawImage viewerImg;
    [SerializeField] private TextMeshProUGUI viewerTxt;
    
    /**
     * Show viewer with passed params.
     * @param img - Image to set.
     * @param txt - Text to set.
     */
    public void ShowViewer(Texture img, String txt) {
        viewerImg.texture = img;
        viewerTxt.text = txt;
        viewerPopup.SetPopupVisibility(true);
    }
}
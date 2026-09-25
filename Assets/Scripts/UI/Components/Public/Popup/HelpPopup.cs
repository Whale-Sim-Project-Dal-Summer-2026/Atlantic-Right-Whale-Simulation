/**
 * HelpPopup.cs: Implements the help popup functionality.
 *
 * @author Mars Semenova
 */

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HelpPopup : MonoBehaviour {
    // params
    // refs
    [Header("References")] 
    [SerializeField] private Image controlsHelp;
    [SerializeField] private Sprite freeCamControls;
    [SerializeField] private Sprite whalePOVControls;
    [SerializeField] private TextMeshProUGUI helpText;
    [Header("Help Text")] 
    [TextArea]
    [SerializeField] private String freeCamHelp;
    [TextArea]
    [SerializeField] private String whalePOVHelp;
    
    // vars
    private String followCamHelp;
    // control sprites
    private Sprite followCamControls;
    
    void Awake() {
        // load 
        followCamControls = controlsHelp.sprite;
        followCamHelp = helpText.text;
        
        // add to event
        CameraController.OnCamSwitch += UpdateControlsHelp;
    }

    private void OnDestroy() {
        // unsub
        CameraController.OnCamSwitch -= UpdateControlsHelp;
    }

    /**
     * Camera event subscriber which sets the corresponding
     * controls help UI based on the camera.
     * @param currCam - Current active camera.
     */
    private void UpdateControlsHelp(int currCam) {
        if (currCam == 1) { 
            controlsHelp.sprite = followCamControls;
            helpText.text = followCamHelp;
        }
        if (currCam == 2) {
            controlsHelp.sprite = freeCamControls;
            helpText.text = freeCamHelp;
        }
        if (currCam == 3) {
            controlsHelp.sprite = whalePOVControls;
            helpText.text = whalePOVHelp;
        }
    }
}
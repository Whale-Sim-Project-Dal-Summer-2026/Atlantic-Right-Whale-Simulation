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
    [SerializeField] private RawImage controlsHelp;
    [SerializeField] private TextMeshProUGUI helpText;
    [Header("Help Text")] 
    [TextArea]
    [SerializeField] private String freeCamHelp;
    [TextArea]
    [SerializeField] private String whalePOVHelp;
    
    // vars
    private String followCamHelp;
    // control sprites
    private Texture followCamControls;
    private Texture freeCamControls;
    private Texture whalePOVControls;
    
    void Awake() {
        // load 
        followCamControls = controlsHelp.texture;
        followCamHelp = helpText.text;
        freeCamControls = Resources.Load<Texture>("UI/Help/freecamhelp");
        whalePOVControls = Resources.Load<Texture>("UI/Help/whalepovhelp");
        
        // add to event
        CameraController.OnCamSwitch += UpdateControlsHelp;
    }

    private void OnDestroy() {
        // unsub
        CameraController.OnCamSwitch -= UpdateControlsHelp;
    }

    /**
     * Camera event subscriber which sets the corresponding
     * control help UI based on camera.
     * @param currCam - Current active camera.
     */

    //  TODO: I would use a state machine using an enum
    //  It would work like the following: 
    //  set all hints to false
    // switch (currcam)
    // case: 1
    //  controlHintsFollowCam.SetActive(true);
    //  break;

    // this would allow you to have ode that is easier to read and is less redundant
    private void UpdateControlsHelp(int currCam) {
        if (currCam == 1) {
            controlsHelp.texture = followCamControls;
            helpText.text = followCamHelp;
        }
        if (currCam == 2) {
            controlsHelp.texture = freeCamControls;
            helpText.text = freeCamHelp;
        }
        if (currCam == 3) {
            controlsHelp.texture = whalePOVControls;
            helpText.text = whalePOVHelp;
        }
    }
}
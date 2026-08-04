/**
 * ControlHints.cs: Script which handles
 * the control hints.
 *
 * @author Mars Semenova 
 */

using UnityEngine;
using UnityEngine.UI;

public class ControlHints : MonoBehaviour {
    // params
    // hint objs
    [Header("Hints")]
    [SerializeField] private GameObject controlHintsFreeCam;
    [SerializeField] private GameObject controlHintsFollowCam;
    [SerializeField] private GameObject controlHintsPOVCam;
    // etc
    [Header("Additional Functionality")]
    [SerializeField] private Button unstickBtn;

    // event
    public delegate void UnstickEvent();
    public static event UnstickEvent OnUnstick;
    
    void Awake() {
        // add to event
        CameraController.OnCamSwitch += UpdateControlHints;
        
        // set up unstick btn
        if (unstickBtn) {
            unstickBtn.onClick.AddListener(() => OnUnstick?.Invoke());
        }
    }

    private void OnDestroy() {
        // unsub
        CameraController.OnCamSwitch -= UpdateControlHints;
    }

    /**
     * Camera event subscriber which sets the corresponding
     * hints UI based on camera.
     * @param currCam - Current active camera.
     */
    private void UpdateControlHints(int currCam) {
        if (controlHintsFollowCam) {
            controlHintsFollowCam.SetActive(false);
            if (currCam == 1) {
                controlHintsFollowCam.SetActive(true);
            }
        }
        if (controlHintsFreeCam) {
            controlHintsFreeCam.SetActive(false);
            if (currCam == 2) {
                controlHintsFreeCam.SetActive(true);
            }
        }
        if (controlHintsPOVCam) {
            controlHintsPOVCam.SetActive(false);
            if (currCam == 3) {
                controlHintsPOVCam.SetActive(true);
            }
        }
    }
}

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
        CameraController.OnCamSwitch += SetCurrCam;
        if (unstickBtn) {
            unstickBtn.onClick.AddListener(() => OnUnstick?.Invoke());
        }
    }

    // camera event subscriber
    /**
     * Camera event subscriber which sets the corresponding
     * hints UI based on camera.
     * @param currCam - Current active camera.
     */
    private void SetCurrCam(int currCam) {
        if (currCam == 1) {
            if (controlHintsFreeCam) {
                controlHintsFreeCam.SetActive(false);
            }
            if (controlHintsFollowCam) {
                controlHintsFollowCam.SetActive(true);
            }
            if (controlHintsPOVCam) {
                controlHintsPOVCam.SetActive(false);
            }
        }

        if (currCam == 2) {
            if (controlHintsFreeCam) {
                controlHintsFreeCam.SetActive(true);
            }
            if (controlHintsFollowCam) {
                controlHintsFollowCam.SetActive(false);
            }
            if (controlHintsPOVCam) {
                controlHintsPOVCam.SetActive(false);
            }
        }

        if (currCam == 3) {
            if (controlHintsFreeCam) {
                controlHintsFreeCam.SetActive(false);
            }
            if (controlHintsFollowCam) {
                controlHintsFollowCam.SetActive(false);
            }
            if (controlHintsPOVCam) {
                controlHintsPOVCam.SetActive(true);
            }
        }
    }
}

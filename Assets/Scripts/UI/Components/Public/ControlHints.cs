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

    //  I would use a state machine using an enum
    //  It would work like the following: 
    //  set all hints to false
    // switch (currcam)
    // case: 1
    //  controlHintsFollowCam.SetActive(true);
    //  break;

    // this would allow you to have ode that is easier to read and is less redundant
    private void SetCurrCam(int currCam) {
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

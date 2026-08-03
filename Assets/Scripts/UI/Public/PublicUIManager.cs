/**
 * PublicUIManager.cs: Script which handles
 * the public UI. Built on top of the functionality
 * in SimulationUIManager.
 *
 * @author Mars Semenova 
 */

using agxSDK;
using UnityEngine;
using UnityEngine.UI;

public class PublicUIManager : MonoBehaviour {
    // params
    // scenarios btn
    [Header("Scenarios Button")]
    [SerializeField] private Button scenariosBtn;
    [Header("Idle Mode Dependencies")]
    [SerializeField] private TogglesManager toggles;
    [SerializeField] private Scrubber scrubber;
    [SerializeField] private GameObject toggleUIBtn;

    void Awake() {
        // scenario btn functionality
        scenariosBtn.onClick.AddListener(() => {
            if (SceneSwitcher.Instance != null) {
                SceneSwitcher.Instance.changeToScenarios();
            }
        });
        
        // set up idle listener
        IdleUI.OnIdle += IdleMode;
    }

    /**
     * Idle mode functionality.
     * @param on - Whether idle mode is on or off.
     */
    private void IdleMode(bool on) {
        if (toggles) {
            toggles.SetUIVisibility(!on);
        }

        if (scrubber) {
            if (on) {
                scrubber.Play(); 
                scrubber.SetEndOfPlaybackBehaviour(EndOfPlaybackBehaviour.Restart);
            } else {
                scrubber.SetEndOfPlaybackBehaviour(EndOfPlaybackBehaviour.Pause);
            }
            scrubber.gameObject.SetActive(!on);
        }
        toggleUIBtn.SetActive(!on);
    }
}

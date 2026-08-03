/**
 * PublicUIManager.cs: Script which handles
 * the public UI. Built on top of the functionality
 * in SimulationUIManager.
 *
 * @author Mars Semenova 
 */

using UnityEngine;
using UnityEngine.UI;

public class PublicUIManager : MonoBehaviour {
    // params
    // scenarios btn
    [Header("Scenarios Button")]
    [SerializeField] private Button scenariosBtn;
    [Header("Idle Mode Dependencies")]
    [SerializeField] private Scrubber scrubber;
    [SerializeField] private GameObject toggleUIBtnObj;
    [SerializeField] private TogglesManager togglesManager;

    // vars
    private Button toggleUIBtn;
    
    void Awake() {
        // get ref
        if (toggleUIBtnObj) {
            toggleUIBtn = toggleUIBtnObj.GetComponent<Button>();
        }
        
        // scenario btn functionality
        scenariosBtn.onClick.AddListener(() => {
            if (SceneSwitcher.Instance != null) {
                SceneSwitcher.Instance.changeToScenarios();
            }
        });
        
        // set up idle listener
        IdleUI.OnIdle += IdleMode;
    }

    void OnDestroy() {
        // unsub
        IdleUI.OnIdle -= IdleMode;
    }

    /**
     * Idle mode functionality.
     * @param on - Whether idle mode is on or off.
     */
    private void IdleMode(bool on) {
        if (toggleUIBtnObj && togglesManager.IsUIVisible() == on) {
            toggleUIBtn.onClick.Invoke();
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
        
        toggleUIBtnObj.SetActive(!on);
    }
}

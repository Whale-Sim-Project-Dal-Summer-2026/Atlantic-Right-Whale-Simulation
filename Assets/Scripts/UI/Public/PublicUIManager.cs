/**
 * PublicUIManager.cs: Script which handles
 * the public UI. Built on top of the functionality
 * in SimulationUIManager.
 *
 * @author Mars Semenova 
 */

using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PublicUIManager : MonoBehaviour {
    // params
    // scripts
    [Header("Scripts")]
    [SerializeField] private SceneSwitcher sceneSwitcher;
    // scenarios btn
    [Header("Scenarios Button")]
    [SerializeField] private Button scenariosBtn;
    
    // vars
    private bool allowInput = true; // TODO

    void Awake() {
        scenariosBtn.onClick.AddListener(sceneSwitcher.changeToScenarios);
    }

    /**
     * Function to determine whether input is allowed.
     * @return Whether input is allowed.
     */
    public bool IsInputAllowed() { // TODO
        return allowInput;
    }
}

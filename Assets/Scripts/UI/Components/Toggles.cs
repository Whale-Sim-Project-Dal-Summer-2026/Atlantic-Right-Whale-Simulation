/**
 * Toggles.cs: Script which handles
 * the toggle buttons.
 *
 * @author Mars Semenova 
 */

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Toggles : MonoBehaviour {
    // params
    // toggles
    [Header("Toggles")]
    [SerializeField] private Button toggleUIBtn;
    [SerializeField] private Button toggleDragBtn;
    [SerializeField] private Button toggleStressBtn;
    [SerializeField] private Button togglePathBtn;
    // path params
    [Header("Path Parameters")]
    [SerializeField] private WhaleTrail whaleTrail;
    
    // events
    public delegate void ToggleUIOnEvent();
    public static event ToggleUIOnEvent OnToggleUIOn;
    public delegate void ToggleUIOffEvent();
    public static event ToggleUIOffEvent OnToggleUIOff;
    public delegate void TogglePathOnEvent();
    public static event TogglePathOnEvent OnTogglePathOn;
    public delegate void TogglePathOffEvent();
    public static event TogglePathOffEvent OnTogglePathOff;

    // vars
    private Sprite toggleUISpriteOff;
    private Sprite toggleUISpriteOn;
    private Image toggleUIBtnImage;
    private Sprite toggleDragSpriteOff;
    private Sprite toggleDragSpriteOn;
    private Image toggleDragBtnImage;
    private Sprite toggleStressSpriteOff;
    private Sprite toggleStressSpriteOn;
    private Image toggleStressBtnImage;
    private Sprite togglePathSpriteOff;
    private Sprite togglePathSpriteOn;
    private Image togglePathBtnImage;
    
    void Awake() {
        // get refs
        if (toggleUIBtn) {
            toggleUIBtnImage = toggleUIBtn.GetComponent<Image>();
            toggleUISpriteOff = toggleUIBtnImage.sprite;
            toggleUISpriteOn = Resources.Load<Sprite>("UI/Toggles/yesui");
        }
        if (toggleDragBtn) {
            toggleDragBtnImage = toggleDragBtn.GetComponent<Image>();
            toggleDragSpriteOff = toggleDragBtnImage.sprite;
            toggleDragSpriteOn = Resources.Load<Sprite>("UI/Toggles/yesdrag");
        }
        if (toggleStressBtn) {
            toggleStressBtnImage = toggleStressBtn.GetComponent<Image>();
            toggleStressSpriteOff = toggleStressBtnImage.sprite;
            toggleStressSpriteOn = Resources.Load<Sprite>("UI/Toggles/yesstress");
        }
        if (togglePathBtn) {
            togglePathBtnImage = togglePathBtn.GetComponent<Image>();
            togglePathSpriteOff = togglePathBtnImage.sprite;
            togglePathSpriteOn = Resources.Load<Sprite>("UI/Toggles/yespath");
        }
    }
    
    void Start() {
        // UI toggle
        if (toggleUIBtn) {
            toggleUIBtn.onClick.AddListener(() => SetUIVisibility(toggleUIBtnImage.sprite == toggleUISpriteOn));
        }

        // drag toggle
        if (toggleDragBtn) {
            toggleDragBtn.onClick.AddListener(() => {
                toggleDragBtnImage.sprite = toggleDragBtnImage.sprite == toggleDragSpriteOff ? toggleDragSpriteOn : toggleDragSpriteOff;
                // TODO
            });
        }

        // stress toggle
        if (toggleStressBtn) {
            toggleStressBtn.onClick.AddListener(() => {
                toggleStressBtnImage.sprite = toggleStressBtnImage.sprite == toggleStressSpriteOff ? toggleStressSpriteOn : toggleStressSpriteOff;
                // TODO
            });
        }

        // path UI
        if (togglePathBtn) {
            togglePathBtn.onClick.AddListener(() => SetPathVisibility(!whaleTrail.IsVisible()));
        }
    }

    /**
     * Togge UI visibility.
     * @param on - Whether the UI should be visible or not.
     */
    public void SetUIVisibility(bool on) {
        if (on) {
            OnToggleUIOn?.Invoke();
        } else {
            OnToggleUIOff?.Invoke();
        }
        
        if (toggleUIBtn) {
            toggleUIBtnImage.sprite = toggleUIBtnImage.sprite == toggleUISpriteOff ? toggleUISpriteOn : toggleUISpriteOff;
        }
    }

    /**
     * Toggle path visibility,
     * @param on - Whether the path should be visible or not.
     */
    public void SetPathVisibility(bool on) {
        if (on) {
            OnTogglePathOn?.Invoke();
        } else {
            OnTogglePathOff?.Invoke();
        }
        
        if (togglePathBtn) {
            togglePathBtnImage.sprite = togglePathBtnImage.sprite == togglePathSpriteOff ? togglePathSpriteOn : togglePathSpriteOff;
        }
    }

    /**
     * Toggle the interactivity of the toggle btns.
     * @param on - Whether the btns should be enabled or not.
     */
    public void SetTogglesInteractivity(bool on) {
        if (toggleUIBtn) {
            toggleUIBtn.interactable = on;
        }
        if (toggleDragBtn) {
            toggleDragBtn.interactable = on;
        }
        if (toggleStressBtn) {
            toggleStressBtn.interactable = on;
        }
        if (togglePathBtn) {
            togglePathBtn.interactable = on;
        }
    }
}

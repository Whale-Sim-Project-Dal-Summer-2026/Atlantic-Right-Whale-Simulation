/**
 * Toggles.cs: Script which implements a generic button toggles component.
 *
 * @author Mars Semenova 
 */

using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
struct Toggle {
    public Button btn;
    public  String spriteRsrcPath;
    public bool on;
    private Sprite spriteOff;

    // I personally advise against the use of getters/setters, it often means that one class is dependant on another, which makes the code less modular
    public void SetSpriteOff (Sprite s) {
        spriteOff = s; 
    }
    public Sprite GetSpriteOff() {
        return spriteOff;
    }
    private Sprite spriteOn;
    public void SetSpriteOn (Sprite s) {
        spriteOn = s; 
    }
    public Sprite GetSpriteOn() {
        return spriteOn;
    }
    private Image btnImage;
    public void SetBtnImage (Image i) {
        btnImage = i; 
    }
    public Image GetBtnImage() {
        return btnImage;
    }
}

public class Toggles : MonoBehaviour {
    // params
    // toggles
    [Header("Toggles")]
    [SerializeField] private Toggle[] toggles;
    
    // events
    public delegate void ToggleEvent(bool on, GameObject obj);
    public static event ToggleEvent OnToggle;
    
    void Awake() {
        // get refs
        for (int x = 0; x < toggles.Length; x++) {
            toggles[x].SetBtnImage(toggles[x].btn.GetComponent<Image>());
            if (toggles[x].spriteRsrcPath != "") {
                if (toggles[x].on) {
                    toggles[x].SetSpriteOff(Resources.Load<Sprite>(toggles[x].spriteRsrcPath));
                    toggles[x].SetSpriteOn(toggles[x].GetBtnImage().sprite);
                } else {
                    toggles[x].SetSpriteOn(Resources.Load<Sprite>(toggles[x].spriteRsrcPath));
                    toggles[x].SetSpriteOff(toggles[x].GetBtnImage().sprite);
                }
            } else {
                toggles[x].SetSpriteOn(toggles[x].GetBtnImage().sprite);
                toggles[x].SetSpriteOff(toggles[x].GetBtnImage().sprite);
            }
        }
    }
    
    void Start() {
        // add event listeners
        for (int x = 0; x < toggles.Length; x++) {
            int param = x;
            toggles[x].btn.onClick.AddListener(() => Toggle(param));
        }
    }

    /**
     * Toggle.
     * @param x - Index of toggle to toggle.
     */
    private void Toggle(int x) {
        toggles[x].on = !toggles[x].on;
        OnToggle?.Invoke(toggles[x].on, toggles[x].btn.gameObject);
        
        toggles[x].GetBtnImage().sprite = toggles[x].on ? toggles[x].GetSpriteOn() : toggles[x].GetSpriteOff();
    }

    /**
     * Set the interactivity of buttons
     * @param on - Whether the buttons should be interactive or not.
     */
    public void SetTogglesInteractivity(bool on) {
        for (int x = 0; x < toggles.Length; x++) {
            toggles[x].btn.interactable = on;
        }
    }
}

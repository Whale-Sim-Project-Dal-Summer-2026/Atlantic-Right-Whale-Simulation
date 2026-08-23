/**
 * Scrubber.cs: Script which implements
 * the scrubber functionality.
 *
 * @author Mars Semenova 
 */

using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public enum EndOfPlaybackBehaviour {
    Pause,
    Restart
}

public class Scrubber : MonoBehaviour {
    // params
    // options
    [Header("Options")]
    [SerializeField] private EndOfPlaybackBehaviour endBehaviour;
    // btns
    [Header("Buttons")]
    [SerializeField] private Button pinBtn;
    // cam btns
    [SerializeField] private Button cam1Btn;
    [SerializeField] private Button cam2Btn;
    [SerializeField] private Button cam3Btn;
    // playback btns
    [SerializeField] private Button restartBtn;
    [SerializeField] private Button bwdBtn;
    [SerializeField] private Button pausePlayBtn;
    [SerializeField] private Button fwdBtn;
    // speed btns
    [SerializeField] private Button slowerBtn;
    [SerializeField] private Button fasterBtn;
    // labels
    [Header("Labels")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI speedText;
    // etc
    [Header("Other")]
    [SerializeField] private Animator animator;
    [SerializeField] private Slider timeline;
    
    // vars
    private String[] speedsLabel = {"0.25", "0.5", "0.75", "1", "1.5", "2", "3", "4", "5"}; 
    private Color pinBtnColor;
    private Image pausePlayBtnImage;
    private Sprite pauseSprite;
    private Sprite playSprite;
    
    // states
    private bool paused;
    private bool pinned  = true;
    private float[] speeds = {0.25f, 0.5f, 0.75f, 1.0f, 1.5f, 2.0f, 3.0f, 4.0f, 5.0f};
    private int speedsInd = 3;

    // events
    public delegate void PauseEvent();
    public static event PauseEvent OnPause;
    public delegate void PlayEvent();
    public static event PlayEvent OnPlay;
    public delegate void CamSwitchEvent(int index);
    public static event CamSwitchEvent OnCamSwitch;
    public delegate void RestartEvent();
    public static event RestartEvent OnRestart;
    
    void Awake() {
        // pin btn colour setup
        if (pinBtn) {
            pinBtnColor = pinBtn.colors.normalColor;
            ColorBlock newColors = pinBtn.colors;
            newColors.normalColor = pinned ? Color.white : pinBtnColor;
            pinBtn.colors = newColors;
        }
        
        // pause/play btn setup
        if (pausePlayBtn) {
            pausePlayBtnImage = pausePlayBtn.GetComponent<Image>();
            pauseSprite = Resources.Load<Sprite>("UI/Scrubber/play");
            playSprite = Resources.Load<Sprite>("UI/Scrubber/pause");
        }
    }
    

    void Start() {
        // set up btns
        if (pinBtn) {
            pinBtn.onClick.AddListener(() => {
                pinned = !pinned;
                ColorBlock newColors = pinBtn.colors;
                newColors.normalColor = pinned ? Color.white : pinBtnColor;
                pinBtn.colors = newColors;
            });
        }

        // cams btns
        if (cam1Btn) {
            cam1Btn.onClick.AddListener(() => {
                OnCamSwitch?.Invoke(1);
            });
        }
        if (cam2Btn) {
            cam2Btn.onClick.AddListener(() => {
                OnCamSwitch?.Invoke(2);
            });
        }
        if (cam3Btn) {
            cam3Btn.onClick.AddListener(() => {
                OnCamSwitch?.Invoke(3);
            });
        }
        
        // playback btns
        if (pausePlayBtn) {
            pausePlayBtn.onClick.AddListener(() => SetPause(!paused));
        }
        if (restartBtn) {
            restartBtn.onClick.AddListener(Restart);
        }
        
        // spd btns
        if (slowerBtn) {
            slowerBtn.onClick.AddListener(() => SetSpeed(speedsInd - 1));
        }
        if (fasterBtn) {
            fasterBtn.onClick.AddListener(() => SetSpeed(speedsInd + 1));
        }
    }

    void Update() {
        // update animator controller state (hover) based on vertical mouse pos
        Vector2 pos = Camera.main.ScreenToViewportPoint(Mouse.current.position.ReadValue());
        if (animator && !pinned) {
            if (pos.y > 0.1) {
                animator.SetBool("hover", false);
            } else {
                if (pos.y <= 0.1 && pos.y != 0) {
                    animator.SetBool("hover", true);
                }
            }
        }
    }
    void OnDestroy() {
        SetPause(false); 
    }
    
    /**
     * Set pause.
     * @param on - Whether pause is on or off.
     */
    private void SetPause(bool on) {
        paused = on;
        if (pausePlayBtnImage) {
            pausePlayBtnImage.sprite = paused ? pauseSprite : playSprite;

        }
        if (on) {
            OnPause?.Invoke();
        } else {
            OnPlay?.Invoke();
        }
    }
    public void Pause() { 
        SetPause(true);
    }
    public void Play() { 
        SetPause(false);
    }
    public void TogglePause() { 
        SetPause(!paused);
    }

    /**
     * Set speed of playback and update UI.
     * @param speedInd - Index of speed in speeds array.
     */
    private void SetSpeed(int speedInd) {
        speedsInd = speedInd;
        
        if (speedsInd <= 0) {
            speedsInd = 0;
            if (slowerBtn) {
                slowerBtn.interactable = false;
            }
        } else {
            if (slowerBtn) {
                slowerBtn.interactable = true;
            }
        }
        if (speedsInd >= speeds.Length-1) {
            speedsInd = speeds.Length-1;
            if (fasterBtn) {
                fasterBtn.interactable = false;
            }
        } else {
            if (fasterBtn) {
                fasterBtn.interactable = true;
            }
        }

        if (speedText) {
            speedText.text = speedsLabel[speedsInd] + "x";
        }
    }

    /**
     * Set the interactivity of the scrubber btns.
     * @param on - Whether the btns should be enabled or not.
     */
    public void SetScrubberInteractivity(bool on) {
        if (pinBtn) {
            pinBtn.interactable = on;
        }
        if (cam1Btn) {
            cam1Btn.interactable = on;
        }
        if (cam2Btn) {
            cam2Btn.interactable = on;
        }
        if (cam3Btn) {
            cam3Btn.interactable = on;
        }
        if (restartBtn) {
            restartBtn.interactable = on;
        }
        if (bwdBtn) {
            bwdBtn.interactable = on;
        }
        if (pausePlayBtn) {
            pausePlayBtn.interactable = on;
        }
        if (fwdBtn) {
            fwdBtn.interactable = on;
        }
        if (slowerBtn) {
            slowerBtn.interactable = on;
        }
        if (fasterBtn) {
            fasterBtn.interactable = on;
        }
    }
    
    /**
     * Update the time display.
     * @param mins - Minutes to set.
     * @param secs - Seconds to set.
     */
    public void UpdateTime(int mins, int secs) { 
        timeText.text = $"{mins:00}:{secs:00}";
    }

    /**
     * Set the percentage of the timeline.
     * @param percent - Percent (0-100) of the timeline.
     */
    public void UpdateTimelineProgress(float percent) {
        timeline.value = percent;
        
        // behaviour on reset
        if (percent >= 100.0) {
            if (endBehaviour == EndOfPlaybackBehaviour.Restart) {
                Restart();
            }
            if (endBehaviour == EndOfPlaybackBehaviour.Pause) {
                Pause();
                pausePlayBtn.interactable = false;
            }
        }
    }

    /**
     * Invoke the restart event.
     */
    private void Restart() {
        pausePlayBtn.interactable = true;
        Play();
        OnRestart?.Invoke();
    }

    /**
     * Set end of playback behaviour.
     * @param behaviour - Behaviour.
     */
    public void SetEndOfPlaybackBehaviour(EndOfPlaybackBehaviour behaviour) {
        endBehaviour = behaviour;
    }
}

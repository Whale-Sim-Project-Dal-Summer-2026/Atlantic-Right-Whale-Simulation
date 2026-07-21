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

public class Scrubber : MonoBehaviour {
    // obj
    private Animator animator;
    private Button pinBtn;
    private Color pinBtnColor;
    private Slider timeline;
    // labels
    private TextMeshProUGUI timeText;
    private TextMeshProUGUI speedText;
    private String[] speedsLabel = {"0.25", "0.5", "0.75", "1", "1.5", "2", "3", "4", "5"}; // TODO: may be able to do this programmatically
    // cam btns
    private Button cam1Btn;
    private Button cam2Btn;
    private Button cam3Btn;
    // playback btns
    private Button restartBtn;
    private Button bwdBtn;
    private Button pausePlayBtn;
    private Image pausePlayBtnImage;
    private Sprite pauseSprite;
    private Sprite playSprite;
    private Button fwdBtn;
    // speed btns
    private Button slowerBtn;
    private Button fasterBtn;

    // states
    private bool paused = false;
    private bool pinned  = true;
    private float[] speeds = {0.25f, 0.5f, 0.75f, 1.0f, 1.5f, 2.0f, 3.0f, 4.0f, 5.0f};
    private int speedsInd = 3;


    // used for the scrubber percentages and time
    public WhaleDriver whaleDriver;
    public float currentTimeStepDelta; 

    //used for the play/pause
    public PauseManager pauseManager;

    public ResetManager resetManager;

    public static event PauseEvent OnPause;

    public delegate void PauseEvent();
    
    
    public static event CamSwitchEvent OnCamSwitch;

    public delegate void CamSwitchEvent(int index);
    
    void Awake() {
        // get refs
        
        animator = GetComponent<Animator>();
        GameObject check = GameObject.Find("PinBtn");
        if (check) {
            pinBtn = check.GetComponent<Button>();
            pinBtnColor = pinBtn.GetComponent<Button>().colors.normalColor;
            ColorBlock newColors = pinBtn.GetComponent<Button>().colors;
            newColors.normalColor = pinned ? Color.white : pinBtnColor;
            pinBtn.GetComponent<Button>().colors = newColors;
        }
        timeline = GameObject.Find("Timeline").GetComponent<Slider>();
        // labels
        timeText = GameObject.Find("Time").GetComponent<TextMeshProUGUI>();
        check = GameObject.Find("Speed");
        if (check) {
            speedText = check.GetComponent<TextMeshProUGUI>();
        }
        // cam btns
        cam1Btn = GameObject.Find("Cam1Btn").GetComponent<Button>();
        cam2Btn = GameObject.Find("Cam2Btn").GetComponent<Button>();
        cam3Btn = GameObject.Find("Cam3Btn").GetComponent<Button>();
        // playback btns
        restartBtn = GameObject.Find("RestartBtn").GetComponent<Button>();
        check = GameObject.Find("BwdBtn");
        if (check) {
            bwdBtn = check.GetComponent<Button>();
        }
        pausePlayBtn = GameObject.Find("PausePlayBtn").GetComponent<Button>(); 
        pausePlayBtnImage = pausePlayBtn.GetComponent<Image>();
        pauseSprite = Resources.Load<Sprite>("UI/Scrubber/play");
        playSprite = Resources.Load<Sprite>("UI/Scrubber/pause");
        check = GameObject.Find("FwdBtn");
        if (check) {
            fwdBtn = check.GetComponent<Button>();
        }
        // speed btns 
        check = GameObject.Find("SlowerBtn");
        if (check) {
            slowerBtn = check.GetComponent<Button>();
        }
        check = GameObject.Find("FasterBtn");
        if (check) {
            fasterBtn = check.GetComponent<Button>();
        }
    }

    void addButtonListeners() {
        if (pinBtn) {
            pinBtn.onClick.AddListener(() => {
                pinned = !pinned;
                ColorBlock newColors = pinBtn.GetComponent<Button>().colors;
                newColors.normalColor = pinned ? Color.white : pinBtnColor;
                pinBtn.GetComponent<Button>().colors = newColors;
            });
        }

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
    }

    void Start() {
        // set up btns
        addButtonListeners();
        
        if (pausePlayBtn) {
            pausePlayBtn.onClick.AddListener(() => SetPause(!paused));
        }
        if (slowerBtn) {
            slowerBtn.onClick.AddListener(() => SetSpeed(speedsInd - 1));
        }
        if (fasterBtn) {
            fasterBtn.onClick.AddListener(() => SetSpeed(speedsInd + 1));
        }
        if (restartBtn) {
            restartBtn.onClick.AddListener(() => resetManager.TriggerReset());
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
        
        updateTime();
        // set scrubber percentage
        float percent = ((float)whaleDriver.currentTimestep / whaleDriver.CSV_ResetTimeStep) * 100; 
        //Debug.Log("Current Prercent: " + percent);

        // this will get the current timne 
        timeline.value = percent;
        
        // shortcuts (TODO)
        if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.mediaPlayPause.wasPressedThisFrame) {
            SetPause(!paused);
        }
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame) {
            // TODO: time+=1
        }
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame) {
            // TODO: time-=1
        }
        if (Keyboard.current.ctrlKey.isPressed) {
            // cams
            if (Keyboard.current.numpad1Key.wasPressedThisFrame || Keyboard.current.digit1Key.wasPressedThisFrame) {
                // TODO: cam 1
            }
            if (Keyboard.current.numpad2Key.wasPressedThisFrame || Keyboard.current.digit2Key.wasPressedThisFrame) {
                // TODO: cam 2
            }
            if (Keyboard.current.numpad3Key.wasPressedThisFrame || Keyboard.current.digit3Key.wasPressedThisFrame) {
                // TODO: cam 3
            }
            
            // speed
            if (Keyboard.current.equalsKey.wasPressedThisFrame) {
                SetSpeed(speedsInd + 1);
            }
            if (Keyboard.current.minusKey.wasPressedThisFrame) {
                SetSpeed(speedsInd - 1);
            }
            
            // scrub back/ahead
            if (Keyboard.current.rightArrowKey.wasPressedThisFrame) {
                // TODO: time+=10
            }
            if (Keyboard.current.leftArrowKey.wasPressedThisFrame) {
                // TODO: time-=10
            }
        }
    }

    /**
     * Toggle pause.
     */
    public void SetPause(bool on) {
        paused = on;
        if (pausePlayBtnImage) {
            pausePlayBtnImage.sprite = paused ? pauseSprite : playSprite;

        }
        OnPause?.Invoke();
    }
    
    /**
     * Set speed of playback and update UI.
     * @param speedInt - Index of speed in speeds array.
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

    public bool IsPaused() {
        return paused;
    }

    public void ToggleScrubberInteractivity(bool on) {
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
    void updateTime() {
        
        float currentTimeStep = whaleDriver.currentTimestep; 
        float secondsConvert = currentTimeStep * currentTimeStepDelta;

        float mins = secondsConvert / 60;
        
        float secs = secondsConvert % 60; 
        int minsInt = Mathf.CeilToInt(mins);
        minsInt--;
        int secsInt = Mathf.CeilToInt(secs); 
        secsInt--;

        timeText.SetText(minsInt + ":" + secsInt);

    }
}

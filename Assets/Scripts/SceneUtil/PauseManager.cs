using System;
using UnityEngine;
using UnityEngine.InputSystem;

enum PauseState{
    PLAYING,
    PAUSED
}

public class PauseManager : MonoBehaviour{
    

    InputAction pauseAction;

    PauseState state;
    
    bool updateTime;

// minimum MS between presses
    double pressBuffer;

    double lastPressTime;

    Scrubber Scrubber;

    [SerializeField] bool allowUserInput;

    void Start(){
        pauseAction = InputSystem.actions.FindAction("Pause");
        Time.timeScale = 1.0f;
        state = PauseState.PLAYING;

        pressBuffer = 500.0f;
        lastPressTime = Time.realtimeSinceStartupAsDouble * 1000;


        HelpPopup.OnSwapStates += swapStates;
        Scrubber.OnPause += swapStates;
    }


    void swapStates(){
        // swap states

        Debug.Log("Swap Pause");
        switch (state){
            case (PauseState.PLAYING):{
                Time.timeScale = 0.0f;
                state = PauseState.PAUSED;       
                break;
            }

            case (PauseState.PAUSED):{
                Time.timeScale = 1.0f;
                state = PauseState.PLAYING;       
                break;
            }
        }
    }


    bool readInAction(){
        if(!allowUserInput) return false;
        
        return (pauseAction?.ReadValue<float>() ?? 0.0f) == 1.0f;
    }


    void Update(){
        bool pausePressed = readInAction();

        if(!pausePressed) return;

        double currTime = Time.realtimeSinceStartupAsDouble * 1000;

        if(currTime - lastPressTime < pressBuffer) return;

        lastPressTime = currTime;

        swapStates();


    }
}
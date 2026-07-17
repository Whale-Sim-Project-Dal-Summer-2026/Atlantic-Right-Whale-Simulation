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


    void Start(){
        pauseAction = InputSystem.actions.FindAction("Pause");
        state = PauseState.PLAYING;

        pressBuffer = 500.0f;
        lastPressTime = Time.realtimeSinceStartupAsDouble * 1000;
    }


    bool readInAction(){
        return (pauseAction?.ReadValue<float>() ?? 0.0f) == 1.0f;
    }


    void Update(){
        bool pausePressed = readInAction();

        if(!pausePressed) return;

        double currTime = Time.realtimeSinceStartupAsDouble * 1000;

        if(currTime - lastPressTime < pressBuffer) return;

        lastPressTime = currTime;

// swap states
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
}
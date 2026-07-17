using UnityEngine;
using UnityEngine.InputSystem;

public class ResetManager : MonoBehaviour{

    public delegate void ResetEvent();

    public static event ResetEvent OnReset;


// how long does the user need to hold down the button to reset?
    [SerializeField] float holdTimeForReset;

    float currTimeHeld;


    InputAction resetButton;


    void Start(){
        currTimeHeld = 0.0f;
        resetButton = InputSystem.actions.FindAction("Reset");
    }

    bool resetBeingPressed(){
        bool press = (resetButton?.ReadValue<float>() ?? 0.0f) == 1.0f;
        return press;
    }

    void Update(){
        if (resetBeingPressed()){
            currTimeHeld += Time.deltaTime;
        }
        else
        {
            currTimeHeld = 0.0f;
        }

        if(currTimeHeld >= holdTimeForReset){
            OnReset?.Invoke();
            currTimeHeld = 0.0f;
        }
    }



}
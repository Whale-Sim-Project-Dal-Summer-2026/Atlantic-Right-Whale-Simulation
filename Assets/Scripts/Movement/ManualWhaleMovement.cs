using System;
using AGXUnity.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore;



public class ManualWhaleController : MonoBehaviour{
    

    [SerializeField] UserInputManager whaleInput;

    [Header("Manually Set Values")]
    [SerializeField] float yaw;
    [SerializeField] float pitch;
    [SerializeField] float speed;
    [SerializeField] float openMouth;

    [Header("Constants")]

    [SerializeField] float maxAcceleration = .5f;
    [SerializeField] float maxSpeed = 5f;
    [SerializeField] float turningSpeed = .5f;

    [Header("References")]
    [SerializeField] AGXUnity.RigidBody rb;
    [SerializeField] GameObject WhaleEnd;
    [SerializeField] ProcessingSettings settings;
    InputAction moveInput;
    InputAction accelerateInput;
    InputAction deccelerateInput;
    InputAction openMouthInput;

    InputAction speedUpAction;



    Vector2 lookDir;
    float accelerateForce;
    float deccelerateForce;

    bool controllerConnected;

    bool speedUpPressed;

    float currSpeedUp;

    float speedAcceleration;
    agx.Vec3 whaleStartPos;
    float timeOutOfWater = 0.0f;
    float pitchNoseDownForceMult = 1.0f;
    float pitchNoiseDownCurrForce = 0.0f;
    float seaLevel;

    private bool controlsEnabled = true;

    void Awake(){
        moveInput = InputSystem.actions.FindAction("Move");
        accelerateInput = InputSystem.actions.FindAction("Accelerate");
        deccelerateInput = InputSystem.actions.FindAction("Deccelerate");
        openMouthInput = InputSystem.actions.FindAction("OpenMouth");
        speedUpAction = InputSystem.actions.FindAction("SimulationSpeedUp");

        controllerConnected = Gamepad.current != null;

        whaleInput.rb = rb.GetInitialized<AGXUnity.RigidBody>();
        whaleStartPos = rb.Native.getPosition();
        speedAcceleration = 1f;
    }

    void Start() {
       seaLevel = settings.SeaLevel; 
    }

    public void resetWhalePosition(){
        rb.Native.setPosition(whaleStartPos);
        // maybe reset yaw pitch etc... here
    }


    void updateMoveInfo(){
        whaleInput.yaw = yaw;
        whaleInput.pitch = pitch;
        // clamp the values to the constraints for the animation
        whaleInput.speed = speed;
        whaleInput.mouthOpen = openMouth == 1;
    }


    void readInputs(){
        if(!controllerConnected) return;

        lookDir = moveInput?.ReadValue<Vector2>() ?? Vector2.zero;
        accelerateForce = accelerateInput?.ReadValue<float>() ?? 0f;
        deccelerateForce = deccelerateInput?.ReadValue<float>() ?? 0f;

        openMouth = openMouthInput?.ReadValue<float>() ?? 0.0f;

        speedUpPressed = (speedUpAction?.ReadValue<float>() ?? 0.0f) == 1.0f; 
    }

    float calculateSpeedUp(){
        if (!speedUpPressed){
            return 0.0f;
        }
        
        return currSpeedUp + (speedAcceleration * Time.deltaTime);
    }


    void updateSpeed(){
        float currAcceleartion = Mathf.Lerp(0,maxAcceleration,accelerateForce);
        float currDecelleration = Mathf.Lerp(0,-maxAcceleration, deccelerateForce);

        float deltaAcceleration = currAcceleartion + currDecelleration;

        speed += deltaAcceleration * Time.deltaTime;

        speed = Mathf.Clamp(speed, 0, maxSpeed);

        currSpeedUp = calculateSpeedUp();
        
        speed += currSpeedUp;
    }

    void updateYawPitch(){
        // pitch X
        // Yaw Z

// remap the inputs from [-1,1] -> [0,1]
        float x = Mathf.Lerp(0.0f, 1.0f, Mathf.InverseLerp(-1.0f,1.0f,lookDir.x));
        float y = Mathf.Lerp(0.0f, 1.0f, Mathf.InverseLerp(-1.0f,1.0f,lookDir.y));

        float yawDelta = Mathf.Lerp(-turningSpeed, turningSpeed, x);
        float pitchDelta = Mathf.Lerp(turningSpeed, -turningSpeed, y);

        yaw += yawDelta;
        pitch += pitchDelta;


        pitchNoseDownOutOfWater();
        yaw = Mathf.Repeat(yaw, 360f);
        pitch = Mathf.Clamp(pitch, -90, 90);


    }

    void pitchNoseDownOutOfWater() {
        if(WhaleEnd.transform.position.y < seaLevel) {
            timeOutOfWater = 0.0f;
            pitchNoiseDownCurrForce = 0.0f;
            return;
        }

        timeOutOfWater += Time.deltaTime;

        pitchNoiseDownCurrForce = pitchNoseDownForceMult * timeOutOfWater;

        pitch += pitchNoiseDownCurrForce;

    }

    public void SetControlsStatus(bool on) {
        controlsEnabled = on;
    }
    
    void Update()
    {
        if (controlsEnabled) {
            updateMoveInfo();
            
            readInputs();

            updateSpeed();

            updateYawPitch();
        }

    }
}


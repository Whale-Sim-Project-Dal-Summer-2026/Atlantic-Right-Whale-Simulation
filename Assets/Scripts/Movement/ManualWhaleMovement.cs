using UnityEngine;
using UnityEngine.InputSystem;



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
    InputAction moveInput;
    InputAction accelerateInput;
    InputAction deccelerateInput;
    InputAction openMouthInput;



    Vector2 lookDir;
    float accelerateForce;
    float deccelerateForce;

    bool controllerConnected;


    void Awake(){
        moveInput = InputSystem.actions.FindAction("Move");
        accelerateInput = InputSystem.actions.FindAction("Accelerate");
        deccelerateInput = InputSystem.actions.FindAction("Deccelerate");
        openMouthInput = InputSystem.actions.FindAction("OpenMouth");
        controllerConnected = Gamepad.current != null;

        whaleInput.rb = rb.GetInitialized<AGXUnity.RigidBody>();

        if (!controllerConnected){
            Debug.LogWarning("No Controller Detected!");
        }

    }

    void updateMoveInfo(){
        whaleInput.yaw = yaw;
        whaleInput.pitch = pitch;
        whaleInput.speed = speed;
        whaleInput.mouthOpen = openMouth == 1;
    }


    void readInputs(){
        if(!controllerConnected) return;

        lookDir = moveInput?.ReadValue<Vector2>() ?? Vector2.zero;
        accelerateForce = accelerateInput?.ReadValue<float>() ?? 0f;
        deccelerateForce = deccelerateInput?.ReadValue<float>() ?? 0f;

        openMouth = openMouthInput?.ReadValue<float>() ?? 0.0f;
    }

    void updateSpeed(){
        float currAcceleartion = Mathf.Lerp(0,maxAcceleration,accelerateForce);
        float currDecelleration = Mathf.Lerp(0,-maxAcceleration, deccelerateForce);

        float deltaAcceleration = currAcceleartion + currDecelleration;

        speed += deltaAcceleration * Time.deltaTime;

        speed = Mathf.Clamp(speed, 0, maxSpeed);
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

        pitch = Mathf.Clamp(pitch, -90, 90);
    }

    void Update()
    {
        updateMoveInfo();
        
        readInputs();

        updateSpeed();

        updateYawPitch();


        // float accelerationDelta = 

    }
}


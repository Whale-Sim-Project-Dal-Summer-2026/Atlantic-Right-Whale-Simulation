using UnityEngine;
using UnityEngine.InputSystem;
public class CameraController : MonoBehaviour{
    [Header("Free Cam")]
    public float moveSpeed = 10f;
    public float fastSpeed = 25f;
    public float sensitivity = 2f;

    [Header("Orbit")]
    public Transform orbitTarget;
    public float orbitDistance = 20f;
    public float orbitSensitivity = 2f;

    CameraControls controls;
    Vector2 moveInput;
    Vector2 lookInput;
    Vector2 upDownInput;
    bool sprinting;
    bool isFreeCam = false;
    float yaw;
    float pitch;
    [SerializeField] bool locked;
    
    CameraState state;
    
    InputAction Cam1;
    InputAction Cam2;
    InputAction Cam3;

    enum CameraState {
        ORBIT,
        FREE,
        LOCKED
    }

    void Awake(){
        Cam1 = InputSystem.actions.FindAction("Cam1");
        Cam2 = InputSystem.actions.FindAction("Cam2");
        Cam3 = InputSystem.actions.FindAction("Cam3");
    
        
        
        locked = false;
        controls = new CameraControls();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        controls.Player.UpDown.performed += ctx => upDownInput = ctx.ReadValue<Vector2>();
        controls.Player.UpDown.canceled += ctx => upDownInput = Vector2.zero;

        controls.Player.Sprint.performed += ctx => sprinting = true;
        controls.Player.Sprint.canceled += ctx => sprinting = false;
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Start(){

        // starting pitch and yaw based on the current state of camera
        yaw   = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;

        //start in orbit mode 
        if (orbitTarget != null)
            ApplyOrbit();
    }

    void changeCams() {
        bool toCam1 = (Cam1?.ReadValue<float>() ?? 0f) == 1.0f;
        bool toCam2 = (Cam2?.ReadValue<float>() ?? 0f) == 1.0f;
        bool toCam3 = (Cam3?.ReadValue<float>() ?? 0f) == 1.0f;

        if (toCam1) state = CameraState.ORBIT;
        if (toCam2) state = CameraState.LOCKED;
        if (toCam3) state = CameraState.FREE;
    }

    void Update() {
        changeCams();
        
        switch (state) {
            case CameraState.ORBIT: {
                UpdateOrbit();
                break;
            }
            case CameraState.FREE: {
                UpdateFreeCam();
                break;
            }
            case CameraState.LOCKED: {
                /*cam on top of whale*/                
                break;
            }
        }

        // Main update loop

        // Toggle between free cam and orbit mode with F key
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            locked = !locked;
        }
        if (Keyboard.current.fKey.wasPressedThisFrame)
            ToggleMode();
        if (isFreeCam)
            UpdateFreeCam();
        else
            UpdateOrbit();

    }

    void ToggleMode(){   
        // Toggle the camera mode
        isFreeCam = !isFreeCam;

        // When switching to orbit mode change to orbit camera right away
        if (!isFreeCam && orbitTarget != null)
        {
            ApplyOrbit();
        }
    }

    void UpdateFreeCam(){

        // rotate with look input 
        yaw += lookInput.x * sensitivity;
        pitch -= lookInput.y * sensitivity;
        pitch = Mathf.Clamp(pitch, -89f, 89f);
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        // move with move input and up/down input (for vertical movement)
        float speed = sprinting ? fastSpeed : moveSpeed;
        Vector3 move =  transform.forward * moveInput.y +
                        transform.right * moveInput.x +
                        transform.up * upDownInput.y;
        transform.position += move * speed * Time.deltaTime;
    }
    void UpdateOrbit(){


        // If no target, do nothing
        if (orbitTarget == null) return;

        // rotate with look input around the target
        yaw += lookInput.x * orbitSensitivity;
        pitch -= lookInput.y * orbitSensitivity;
        pitch = Mathf.Clamp(pitch, -80, 80);
        
        // calculate zoom based on up/down input (basically how close to the target))
        float zoom   = upDownInput.y * moveSpeed * Time.deltaTime;
        
    
        // apply zoom to base orbit distance
        orbitDistance -= zoom;
        // take the max of orbit to prevent from going through the target
        orbitDistance  = Mathf.Max(5f, orbitDistance);
        if(locked) return;
       
        ApplyOrbit();
    }

    void ApplyOrbit(){

        // calcuate rotation based on the pitch and yaw
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        // set position based on the targets position 
        transform.position = orbitTarget.position - rot * Vector3.forward * orbitDistance;

        transform.rotation = rot;
    }
}
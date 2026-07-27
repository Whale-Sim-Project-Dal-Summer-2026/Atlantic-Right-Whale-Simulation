using openplx.Physics3D.Interactions;
using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

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
    float yaw;
    float pitch;
    [SerializeField] bool rotationLocked;
    [SerializeField] GameObject POVTarget;
    
    CameraState state;
    
    InputAction Cam1;
    InputAction Cam2;
    InputAction Cam3;
    InputAction CamLock;

    float inputBuffer = 200;
    double lastTimePressed;
    
    public delegate void CamSwitchEvent(int index);

    public static event CamSwitchEvent OnCamSwitch;

    bool forceLock;

    enum CameraState {
        ORBIT,
        FREE,
        POV,
    }

    void Awake() {


        lastTimePressed = Time.realtimeSinceStartupAsDouble * 1000;

        
        Cam1 = InputSystem.actions.FindAction("Cam1");
        Cam2 = InputSystem.actions.FindAction("Cam2");
        Cam3 = InputSystem.actions.FindAction("Cam3");
        CamLock = InputSystem.actions.FindAction("CamLock");
    
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


    

    void OnEnable()
    {
        Scrubber.OnCamSwitch += changeToCam;
        PopupManager.OnHelpPopupOn += SetForceLockOn;
        PopupManager.OnHelpPopupOff += SetForceLockOff;
        controls.Enable();  
    } 
    void OnDisable(){
        Scrubber.OnCamSwitch -= changeToCam;
        PopupManager.OnHelpPopupOn -= SetForceLockOn;
        PopupManager.OnHelpPopupOff -= SetForceLockOff;
        controls.Disable();
    }

    void Start(){

        // starting pitch and yaw based on the current state of camera
        yaw   = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;

    }

    void SetForceLock(bool on){
        forceLock = on;
    }
    void SetForceLockOn(){
        SetForceLock(true);
    }
    void SetForceLockOff(){
        SetForceLock(false);
    }

    void lockUnLockCamera() {
        bool lockPressed = (CamLock?.ReadValue<float>() ?? 0f) > .5f;
        if (!lockPressed) return;

        double currTime = Time.realtimeSinceStartupAsDouble * 1000;
        
        if (currTime - lastTimePressed < inputBuffer) return;

        lastTimePressed = currTime;
        rotationLocked = !rotationLocked;
    }


    void changeToCam(int index) {
        switch (index) {
            case 1:
                state = CameraState.ORBIT;
                break;
            case 2:
                state = CameraState.FREE;
                break;
            case 3:
                state = CameraState.POV;
                break;
        }
        
        OnCamSwitch?.Invoke(index);
    }
    void changeCams() {
        bool toCam1 = (Cam1?.ReadValue<float>() ?? 0f) == 1.0f;
        bool toCam2 = (Cam2?.ReadValue<float>() ?? 0f) == 1.0f;
        bool toCam3 = (Cam3?.ReadValue<float>() ?? 0f) == 1.0f;

        if (toCam1) changeToCam(1);
        if (toCam2) changeToCam(2);
        if (toCam3) changeToCam(3);

    }

    void Update() {
        if(forceLock) return;

        changeCams();
        lockUnLockCamera();
        
        switch (state) {
            case CameraState.ORBIT: {
                UpdateOrbit();
                break;
            }
            case CameraState.FREE: {
                UpdateFreeCam();
                break;
            }
            case CameraState.POV: {
                UpdatePOVCam();
                break;
            }
        }
    }


    void UpdatePOVCam() {
        transform.position = POVTarget.transform.position;
        transform.rotation = Quaternion.LookRotation(POVTarget.transform.parent.rotation * Vector3.forward);
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

        float rotate = rotationLocked ? 0.0f : 1.0f;
        yaw += lookInput.x * orbitSensitivity * rotate;
        pitch -= lookInput.y * orbitSensitivity * rotate;
        pitch = Mathf.Clamp(pitch, -80, 80);
        
        // calculate zoom based on up/down input (basically how close to the target))
        float zoom   = upDownInput.y * moveSpeed * Time.deltaTime;
        
    
        // apply zoom to base orbit distance
        orbitDistance -= zoom;
        // take the max of orbit to prevent from going through the target
        orbitDistance  = Mathf.Max(5f, orbitDistance);

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);

        transform.position = orbitTarget.position - rot * Vector3.forward * orbitDistance;
        
        if(rotationLocked) return;
        
        transform.rotation = rot;
    }

}
using openplx.Physics3D.Interactions;
using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public enum CameraState {
    ORBIT,
    FREE,
    POV,
}

public class CameraController : MonoBehaviour{
    [Header("Free Cam")]
    public float moveSpeed = 25f;
    public float fastSpeed = 35f;
    public float sensitivity = 1.5f;

    [Header("Orbit")]
    public Transform orbitTarget;
    public float orbitDistance = 20f;
    public float orbitSensitivity = 1.5f;

    CameraControls controls;
    Vector2 moveInput;
    Vector2 lookInput;
    Vector2 upDownInput;

    bool sprinting;
    float yaw;
    float pitch;
    [SerializeField] bool rotationLocked;
    [SerializeField] GameObject POVTarget;
    bool freeCamReset;
    
    CameraState state;
    
    public delegate void CamSwitchEvent(int index); // TODO: should be passing CameraState or smth instead for uniformity 

    public static event CamSwitchEvent OnCamSwitch;

    bool forceLock;

    void Awake() {
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
    
    void OnEnable() {
        controls.Enable();  
    } 
    void OnDisable(){
        controls.Disable();
    }

    void Start(){
        // starting pitch and yaw based on the current state of camera
        yaw   = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    public void SetForceLock(bool on){
        forceLock = on;
    }

   public void lockUnLockCamera() {
        rotationLocked = !rotationLocked;
    }


    public void changeToCam(int index) {
        rotationLocked = false;
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

    void Update() {
        if(forceLock) return;
        
        switch (state) {
            case CameraState.ORBIT: {
                UpdateOrbit();
                break;
            }
            case CameraState.FREE: {
                UpdateFreeCam();
                // if the free cam reset is pressed, set pos/rot to pov cam
                if (freeCamReset){
                    UpdatePOVCam();
                    freeCamReset = false;       
                }
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
        transform.position += move * speed * Time.unscaledDeltaTime; // TODO: use smth else bc time is set to 0 on pause 
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
        float zoom   = upDownInput.y * moveSpeed * Time.unscaledDeltaTime;
        
    
        // apply zoom to base orbit distance
        orbitDistance -= zoom;
        // take the max of orbit to prevent from going through the target
        orbitDistance  = Mathf.Max(5f, orbitDistance);

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);

        transform.position = orbitTarget.position - rot * Vector3.forward * orbitDistance;
        
        if(rotationLocked) return;
        
        transform.rotation = rot;
    }


    public void resetFreeCam()
    {
        freeCamReset = true;
    }



}
using agxCollide;
using AGXUnity.Utils;
using UnityEngine;
using UnityEngine.InputSystem;



public class MovementRB : MonoBehaviour
{
    CameraControls controls;
    Vector3 startPosition;
    bool resetTriggered;
    public float speed = 10000000f;
    public float angularSpeed = 1f;
    AGXUnity.RigidBody rb;
    [SerializeField] GameObject target;

    void Awake()
    {
        controls = new CameraControls();
        rb = GetComponent<AGXUnity.RigidBody>();
        startPosition = transform.position;
    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void Update()
    {
        // rb.Native.setLocalPosition(target.transform.position.ToHandedVec3());
        // rb.Native.setLocalRotation(target.transform.rotation.ToHandedQuat());
        // rb.SyncNativeTransform();
        if (Keyboard.current.rKey.isPressed){
            resetTriggered = true;            
        }

        Keyboard keyboard = Keyboard.current;

        Vector3 dir = Vector3.zero;
        Vector3 velocityDir = Vector3.zero;

        if (keyboard.wKey.isPressed){
            dir += transform.forward;            
        }

        if (keyboard.sKey.isPressed){
            dir -= transform.forward;            
        }

        if (keyboard.aKey.isPressed){
            dir -= transform.right;            
        }

        if (keyboard.dKey.isPressed){
            dir += transform.right;            
        }


        if (keyboard.upArrowKey.isPressed){
            velocityDir += Vector3.right * angularSpeed;            
        }

        if (keyboard.downArrowKey.isPressed){
            velocityDir +=  Vector3.left * angularSpeed;            
        }

        if (keyboard.leftArrowKey.isPressed){
            velocityDir += Vector3.up * angularSpeed;            
        }

        if (keyboard.rightArrowKey.isPressed){
            velocityDir +=  Vector3.down * angularSpeed;            
        }

        rb.Native.addForce((dir * speed * Time.deltaTime).ToHandedVec3());
        rb.AngularVelocity = velocityDir;
    }
}
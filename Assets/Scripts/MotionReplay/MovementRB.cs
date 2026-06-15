using UnityEngine;



public class MovementRB : MonoBehaviour
{
    CameraControls controls;
    Vector3 startPosition;
    bool resetTriggered;
    public float speed = 5f;
    Rigidbody rb;

    void Awake()
    {
        controls = new CameraControls();
        rb = GetComponent<Rigidbody>();
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



   void FixedUpdate()
    {
        if (resetTriggered)
        {
            transform.position = startPosition; 
            resetTriggered = false;
            return; 
        }

        // Move forward by speed amount 
        Vector3 newPosition = rb.position + Vector3.forward * speed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);

    }
    void Update()
    {
        if (controls.Player.Reset.triggered)
            resetTriggered = true;
    }
}
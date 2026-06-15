using UnityEngine;



public class Movement : MonoBehaviour
{
    CameraControls controls;
    float StartposZ;
    public float speed = 5f;

    void Awake()
    {
        controls = new CameraControls();
    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void Start()
    {
        StartposZ = transform.position.z;
    }

    void Update()
    {
        float movez = speed * Time.deltaTime;
        transform.Translate(0, 0, movez);

        // IF reset button pressed then reset position
        if (controls.Player.Reset.triggered)
        {
            transform.position = new Vector3(0, 5, 0);
        }
    }
}
using UnityEngine;
using MotionDataPacketClass;
using System.Collections;
using System.Collections.Generic;
public class WhaleMovementMotionData : MonoBehaviour
{   
    CameraControls controls;
    public TextAsset csvData;
    public List<MotionDataPacket> motionDataPacketList = new List<MotionDataPacket>();

    public float positionSmoothSpeed = 5f;
    public float rotationSmoothSpeed = 5f;

    public float SeaLevel;

    //List of columns need for motion data packet
    private int[] cols = { 0, 1, 8, 9, 10, 12, 14, 23 };

    public int currentItemIndex = 0;
    private Vector3 startPos;
    private Quaternion startRot;

    private Vector3 targetPosition;
    private Quaternion targetRotation;

    [SerializeField] Animator animator;

    void Start()
    {
        LoadCSV();


    }
 void Awake()
    {
        startPos = transform.position;
        startRot = transform.rotation;

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

    void LoadCSV()
    {
        // Split file into lines 
        string[] lines = csvData.text.Split(new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');

            if (values.Length >= cols.Length) 
            {
                MotionDataPacket dataPacket = new MotionDataPacket();
                
                dataPacket.timestep = float.Parse(values[cols[0]]);
                dataPacket.depth = float.Parse(values[cols[1]]);
                dataPacket.head = float.Parse(values[cols[2]]) * Mathf.Rad2Deg;
                dataPacket.pitch = -float.Parse(values[cols[3]]) * Mathf.Rad2Deg;
                dataPacket.roll = float.Parse(values[cols[4]])* Mathf.Rad2Deg;
                if (values[cols[6]]=="NaN") { dataPacket.speed= 0.0f;}
                else { dataPacket.speed = float.Parse(values[cols[5]]);}
               
                dataPacket.fluking_signal = float.Parse(values[cols[6]]);
                dataPacket.MouthOpen = int.Parse(values[cols[7]]);
               
                motionDataPacketList.Add(dataPacket);
            }
        }
        
        Debug.Log("Loaded " + motionDataPacketList.Count + " items from CSV.");
    }

  void FixedUpdate()
{

    if (motionDataPacketList.Count == 0) return;

    //use lerp and slerp to adjust to target
    transform.position = Vector3.Lerp(transform.position, targetPosition, Time.fixedDeltaTime * positionSmoothSpeed);
    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSmoothSpeed);
    
    // get next csv packet
    if (currentItemIndex < motionDataPacketList.Count)
    {   
        MotionDataPacket currentPacket = motionDataPacketList[currentItemIndex];

        /// data integrity
        if (float.IsNaN(currentPacket.speed) || float.IsNaN(currentPacket.pitch) || 
            float.IsNaN(currentPacket.head) || float.IsNaN(currentPacket.roll))
        {
            Debug.LogWarning($"Row {currentItemIndex} contains a NaN value");
            currentPacket.speed = 0f;
            currentPacket.pitch = 0f;
            currentPacket.head = 0f;
            currentPacket.roll = 0f;
        }

        // Capture initial starting position on frame 1 
        if (currentItemIndex == 0)
        {
            // startPos = transform.position;
            // Initialize targets so the object doesn't jump wildly on the first frame
            targetPosition = transform.position;
            targetRotation = transform.rotation;
        }

        // Set rotation target
        targetRotation = Quaternion.Euler(currentPacket.pitch, currentPacket.head, currentPacket.roll);

        // Calculate next forward step target
        Vector3 forwardStep = targetRotation * Vector3.forward * currentPacket.speed * (Time.fixedDeltaTime*10);
        targetPosition += forwardStep;

        // Apply depth to the target position
        targetPosition.y = SeaLevel - currentPacket.depth; 

        // Move to the next index for the next frame
        currentItemIndex++;
    }
    else
    {
        Debug.Log("Reached end of File");
    }

    if (controls.Player.Reset.triggered){

        transform.position = startPos;
        transform.rotation = startRot;
        animator.Play("R Whale Armature|Whale Swimming",0,0);
        currentItemIndex = 0;      
    }


}
}

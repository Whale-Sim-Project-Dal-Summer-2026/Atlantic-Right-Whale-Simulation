using UnityEngine;
using MotionDataPacketClass;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem.XR;
public class WhaleTailMotionData : MonoBehaviour
{   
    CameraControls controls;
    public TextAsset csvData;
    public List<MotionDataPacket> motionDataPacketList = new List<MotionDataPacket>();

    public List<GameObject> bonesList = new List<GameObject>();

    private int counter = 0;

    public GameObject tailRoot;
    public Transform tailStop;

    public GameObject bodyRoot; 

    public float positionSmoothSpeed = 5f;
    public float rotationSmoothSpeed = 5f;

    //List of columns need for motion data packet
    private int[] cols = { 0, 1, 8, 9, 10, 12, 14, 15, 23 };

    private int flukeSignal =  14;
    private int bodyAngle = 15;

    private int currentItemIndex = 0;

    private Quaternion bodyStartRot;
    private Quaternion tailStartRot;

    private Dictionary<string,Quaternion> boneStart = new Dictionary<string,Quaternion>();

    private Quaternion bodyTargetRotation;
    private Quaternion tailTargetRotation;
    public bool useSlerp = true;
    
    void saveBoneStart()
    {
        foreach (GameObject bone_ in bonesList)
        {
            string cur_boneName = bone_.name;
            boneStart.Add(cur_boneName,bone_.transform.localRotation);
        }
    }

    void Start()
    {
        LoadMotionDataCSV();
        findBones();
        saveBoneStart();

    }
 void Awake()
    {
        tailStartRot = tailRoot.transform.localRotation;
        bodyStartRot = bodyRoot.transform.rotation;

        bodyTargetRotation = bodyStartRot;
         tailTargetRotation = tailStartRot;

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

    void LoadMotionDataCSV()
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
               
                dataPacket.fluking_signal = float.Parse(values[cols[6]])* Mathf.Rad2Deg;
                dataPacket.body_signal = float.Parse(values[cols[7]]) * Mathf.Rad2Deg;
                dataPacket.MouthOpen = int.Parse(values[cols[8]]);
               
                motionDataPacketList.Add(dataPacket);
            }
        }
        
        Debug.Log("Loaded " + motionDataPacketList.Count + " items from CSV.");
    }
    void findBones()
    {

        bonesList.Add(tailRoot);

        GetAllChildren(tailRoot,bonesList);

    }

    void GetAllChildren(GameObject parent, List<GameObject> addingList){
        foreach (Transform child in parent.transform){

            if (child.name == "Colliders"|| child.name == "Collider"){ continue;}
            addingList.Add(child.gameObject);

            if (child == tailStop)
            {
                break;
            }
            // Recursively add this child's children
            GetAllChildren(child.gameObject,addingList);
        }
   
    }



    void updateTail(MotionDataPacket currentPacket)
    {
        
        float equalUpdate = (currentPacket.fluking_signal/bonesList.Count); 
        foreach (GameObject bone_ in bonesList)
        {
            Quaternion currentStart = boneStart[bone_.name];
            bone_.transform.localRotation = Quaternion.Euler(equalUpdate, 0, 0) * currentStart;

        }
    }

  void FixedUpdate()
{

    //FIXED TIME STEP SET TO 0.1 WHICH MATCHES THE 10 HZ OF THE MOTION DATA, SO EACH FIXED UPDATE SHOULD CORRESPOND TO ONE ROW IN THE CSV FILE.

    if (motionDataPacketList.Count == 0) return;

    if (useSlerp)
    {
        bodyRoot.transform.rotation = Quaternion.Slerp(bodyRoot.transform.rotation, bodyTargetRotation, Time.fixedDeltaTime * rotationSmoothSpeed);
        tailRoot.transform.localRotation = Quaternion.Slerp(tailRoot.transform.localRotation, tailTargetRotation, Time.fixedDeltaTime * rotationSmoothSpeed);
    }
    //bodyRoot.transform.rotation = Quaternion.Slerp(bodyRoot.transform.rotation, bodyTargetRotation, Time.fixedDeltaTime);// * rotationSmoothSpeed);

    
    //bodyRoot.transform.rotation = Quaternion.Slerp(bodyRoot.transform.rotation, bodyTargetRotation, (Time.fixedDeltaTime*0.1f) * rotationSmoothSpeed);

    //tailRoot.transform.localRotation = Quaternion.Slerp(tailRoot.transform.localRotation, tailTargetRotation, Time.fixedDeltaTime);//* rotationSmoothSpeed);
        
    
    // if (counter == 50)
    // {
    //     tailRoot.transform.localRotation = Quaternion.Slerp(tailRoot.transform.localRotation, tailTargetRotation, (Time.fixedDeltaTime*(counter/2)) * rotationSmoothSpeed);
    //     counter = 0;
    // }
    
    // get next csv packet
    if (currentItemIndex < motionDataPacketList.Count)
    {   
        MotionDataPacket currentPacket = motionDataPacketList[currentItemIndex];

        /// data integrity
        if (float.IsNaN(currentPacket.body_signal) || float.IsNaN(currentPacket.fluking_signal))
        {
            Debug.LogWarning($"Row {currentItemIndex} contains a NaN value");
            currentPacket.body_signal = 0f;
            currentPacket.fluking_signal = 0f;
          
        }

        // Capture initial starting position on frame 1 
        if (currentItemIndex == 0)
        {
            // startPos = transform.position;
            // Initialize targets so the object doesn't jump on the first frame
            bodyTargetRotation = bodyRoot.transform.rotation;
            tailTargetRotation = tailRoot.transform.localRotation;
        }

        updateTail(currentPacket);
        if (useSlerp)
        {
            bodyTargetRotation = Quaternion.Euler(currentPacket.body_signal, 0, 0) * bodyStartRot;
            //tailTargetRotation = Quaternion.Euler(currentPacket.fluking_signal*1.1f, 0, 0) * tailStartRot;
        }
        else
        {
            bodyRoot.transform.rotation = Quaternion.Euler(currentPacket.body_signal, 0, 0) * bodyStartRot;
            //tailRoot.transform.localRotation = Quaternion.Euler(currentPacket.fluking_signal, 0, 0) * tailStartRot;
        }
        // Set rotation target
        //bodyTargetRotation = Quaternion.Euler(currentPacket.body_signal, 0, 0) * bodyStartRot;

        //SCALING THE FLUKE AMOUNG TO MAKE THE MOTION HAVE MORE AMPLITUDE 
        //tailTargetRotation = Quaternion.Euler(currentPacket.fluking_signal*1.1f, 0, 0) * tailStartRot;

        // bodyRoot.transform.rotation = Quaternion.Euler(currentPacket.body_signal, 0, 0) * bodyStartRot;
        // tailRoot.transform.localRotation = Quaternion.Euler(currentPacket.fluking_signal, 0, 0) * tailStartRot;
        
        counter++;
        

        // Move to the next index for the next frame
        currentItemIndex++;
    }
    else
    {
        Debug.Log("Reached end of File");
    }

    if (controls.Player.Reset.triggered){

        bodyRoot.transform.rotation = bodyStartRot;
        tailRoot.transform.localRotation = tailStartRot;
    
        currentItemIndex = 0;      
    }


}







}

using UnityEngine;
using MotionDataPacketClass;
using System.Collections.Generic;
public class WhaleFlukingMotionDriver: MonoBehaviour
{   
    CameraControls controls;
    private List<MotionDataPacket> motionDataPacketList = new List<MotionDataPacket>();

    private List<GameObject> bonesList = new List<GameObject>();


    public GameObject tailRoot;
    public Transform tailStop;

    public GameObject bodyRoot; 

    public float positionSmoothSpeed = 5f;
    public float rotationSmoothSpeed = 5f;

    //List of columns need for motion data packet
    private int[] cols = { 0, 1, 8, 9, 10, 12, 14, 15, 23 };


    private int currentItemIndex = 0;

    private Quaternion bodyStartRot;
    private Quaternion tailStartRot;

    private Dictionary<string,Quaternion> boneStart = new Dictionary<string,Quaternion>();

    private Quaternion bodyTargetRotation;
    private Quaternion tailTargetRotation;
    public bool useSlerp;

    [Header("Animation Settings")]
    public AnimationSettings animationSettings;
    
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

    void LoadMotionDataCSV(){
        var loaded_CsvData = loadCSV(animationSettings.MotionData_csv,animationSettings.MotionData_ContainsHeaders);
        string[][] motionData = loaded_CsvData.data;

        for (int i = 0;i<motionData.Length; i++)
        {
            MotionDataPacket dataPacket = new MotionDataPacket {
                timestep = float.Parse(motionData[i][cols[0]]),
                depth = float.Parse(motionData[i][cols[1]]),
                head = float.Parse(motionData[i][cols[2]]) * Mathf.Rad2Deg,
                pitch = -float.Parse(motionData[i][cols[3]]) * Mathf.Rad2Deg,
                roll = float.Parse(motionData[i][cols[4]]) * Mathf.Rad2Deg,
                fluking_signal = float.Parse(motionData[i][cols[6]])* Mathf.Rad2Deg,
                body_signal = float.Parse(motionData[i][cols[7]]) * Mathf.Rad2Deg,
                MouthOpen = int.Parse(motionData[i][cols[8]]),
                speed = motionData[i][cols[6]] == "NaN" ? 0.0f : float.Parse(motionData[i][cols[5]]);
            };
           
            motionDataPacketList.Add(dataPacket);
        }
        
        Debug.Log("Loaded " + motionDataPacketList.Count + " items from CSV.");
    }

    // maybe refactor into a class???
    private (string[][] data ,Dictionary<string,int> columnIndices) loadCSV(TextAsset csvFile,bool hasHeaders){
        
        // dict for getting the index of a column from its name
        Dictionary<string,int> columnIndices = new Dictionary<string, int>();

        int rowCount;
        //sets start index for saving csvData
        int startIndex = 0;

        string[] lines = csvFile.text.Split(new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        rowCount = lines.Length; 
    
        // Gets columns from csv if present
        if (hasHeaders){
            string[] headers = lines[0].Split(',');
            for (int i = 0; i < headers.Length; i++){columnIndices[headers[i].Trim()] = i;}
            startIndex++;
            rowCount--;
        } 

        string[][] outputData = new string[rowCount][];

        for (int i = startIndex; i < lines.Length; i++){
            string[] currentRowValues = lines[i].Split(',');

            if (currentRowValues.Length >= cols.Length){

                if (hasHeaders){outputData[i-1] = currentRowValues;} 
                else {outputData[i] = currentRowValues;}
            }
        }
        
        return (outputData,columnIndices);
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
            tailTargetRotation = Quaternion.Euler(currentPacket.fluking_signal*1.1f, 0, 0) * tailStartRot;
        }
        else
        {
            bodyRoot.transform.rotation = Quaternion.Euler(currentPacket.body_signal, 0, 0) * bodyStartRot;
            tailRoot.transform.localRotation = Quaternion.Euler(currentPacket.fluking_signal, 0, 0) * tailStartRot;
        }
        // Set rotation target
        //bodyTargetRotation = Quaternion.Euler(currentPacket.body_signal, 0, 0) * bodyStartRot;

        //SCALING THE FLUKE AMOUNG TO MAKE THE MOTION HAVE MORE AMPLITUDE 
        //tailTargetRotation = Quaternion.Euler(currentPacket.fluking_signal*1.1f, 0, 0) * tailStartRot;

        // bodyRoot.transform.rotation = Quaternion.Euler(currentPacket.body_signal, 0, 0) * bodyStartRot;
        // tailRoot.transform.localRotation = Quaternion.Euler(currentPacket.fluking_signal, 0, 0) * tailStartRot;
        
        
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

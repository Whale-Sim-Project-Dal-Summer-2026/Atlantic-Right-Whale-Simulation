using UnityEngine;
using MotionDataPacketClass;
using System.Collections.Generic;
using FlukeWaveAmplitudeLookUpClass;
using System;

// So 
// whale class which contains the main functions
// there should be an index step which handles the processing of the animation
// maybe pre-process it?? so all of the states are saved into a file and then loaded or kept in ram 

// needs to be able to jump to a timestep, lets lock in time steps as 1/10 of a second
// so the random walk will need to generate a similair timestep thing 
// 

// new class whale controller
// controller can operate in two modes cats tag replay, or random walk for entanglement

// both will use same timestep so UI can remain the same 

//so lets abstract the controller to relay on what we will call a motionSource
// this source abstract will have get nextStep, and getStep for a timestep speficic one

//whale controller will hold a list of bones with certain sections being denoted as certian parts (ie tail) - this will be done using a index range

// this is also where drag and other things will be exposed - maybe a listener type situation? 


public class WhaleFlukingMotionDriver: MonoBehaviour
{   
    CameraControls controls;
    private List<MotionDataPacket> motionDataPacketList = new List<MotionDataPacket>();

    public List<GameObject> bonesList = new List<GameObject>();


    public GameObject tailRoot;
    public Transform tailStop;

    public GameObject bodyRoot; 

    public float positionSmoothSpeed = 5f;
    public float rotationSmoothSpeed = 5f;


    FlukeWaveAmplitudeLookUp lookUp;
    private int currentItemIndex = 0;

    private Quaternion bodyStartRot;
    private Quaternion tailStartRot;

    private Dictionary<string,Quaternion> boneStart = new Dictionary<string,Quaternion>();

    private Quaternion bodyTargetRotation;
    private Quaternion tailTargetRotation;
    public bool useSlerp;

    [Header("Animation Settings")]
    public AnimationSettings animationSettings;

    public bool updateAmpFreq = false; 

    [Header("Tweaking Parameters")]
    public double amplitude = 0.0101001492483101;
    public double frequency = 0.142049987510281;
    public float wave_offset;

    public float phaseShiftPerUnit = 0.8f; 

    // update to be around normal limits of whale tail movement 
    public List<float> boneMaxAngles = new List<float>
{
    0.1f, 
    0.1f, 
    0.1f, 
    0.1f, 
    0.1f, 
    0.1f, 
    0.1f, 
    0.1f, 
    0.1f, 
    0.1f, 
    0.1f, 
    0.1f, 
    0.1f, 
    0.1f, 
    0.1f
};
public float boneLength = 0.1f;
    
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
        loadFlukeWaveAmplitudeLookUpCSV();
        findBones();
        saveBoneStart();

    }
 void Awake(){

        tailStartRot = tailRoot.transform.localRotation;
        bodyStartRot = bodyRoot.transform.rotation;

        bodyTargetRotation = bodyStartRot;
        tailTargetRotation = tailStartRot;

        controls = new CameraControls();
    }

    void OnEnable(){
        controls.Enable();
    }

    void OnDisable(){
        controls.Disable();
    }

    void LoadMotionDataCSV(){
        var loaded_CsvData = loadCSV(animationSettings.MotionData_csv,animationSettings.MotionData_ContainsHeaders);
        string[][] motionData = loaded_CsvData.data;
        Dictionary<string,int> columnIndices= loaded_CsvData.columnIndices;

        for (int i = 0;i<motionData.Length; i++)
        {
            MotionDataPacket dataPacket = new MotionDataPacket {
                timestep = float.Parse(motionData[i][columnIndices["Date"]]),
                depth = float.Parse(motionData[i][columnIndices["Depth"]]),
                head = float.Parse(motionData[i][columnIndices["head"]]) * Mathf.Rad2Deg,
                pitch = -float.Parse(motionData[i][columnIndices["pitch"]]) * Mathf.Rad2Deg,
                roll = float.Parse(motionData[i][columnIndices["roll"]]) * Mathf.Rad2Deg,
                fluking_signal = float.Parse(motionData[i][columnIndices["fluking_signal"]])* Mathf.Rad2Deg,
                body_signal = float.Parse(motionData[i][columnIndices["body_orientation"]]) * Mathf.Rad2Deg,
                MouthOpen = int.Parse(motionData[i][columnIndices["MouthOpen"]]),
                speed = motionData[i][columnIndices["speed"]] == "NaN" ? 0.0f : float.Parse(motionData[i][columnIndices["speed"]])
            };
           
            motionDataPacketList.Add(dataPacket);
        }
        
        Debug.Log("Loaded " + motionDataPacketList.Count + " items from CSV.");
    }
    void loadFlukeWaveAmplitudeLookUpCSV(){
        var loaded_CsvData = loadCSV(animationSettings.FlukeAmpLookUp_csv,animationSettings.FlukeAmp_ContainsHeaders);
        string[][] csvData = loaded_CsvData.data;
        Dictionary<string,int> columnIndices= loaded_CsvData.columnIndices;

        lookUp = new FlukeWaveAmplitudeLookUp(csvData,columnIndices);

        Debug.Log("Loaded Fluke Wave Amplitude LookUp with " + lookUp.Count() + " entries.");
    }

    // maybe refactor into a class???
    (string[][] data ,Dictionary<string,int> columnIndices) loadCSV(TextAsset csvFile,bool hasHeaders){
        
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

            if (currentRowValues.Length >= columnIndices.Count){

                if (hasHeaders){outputData[i-1] = currentRowValues;} 
                else {outputData[i] = currentRowValues;}
            }
        }
        
        return (outputData,columnIndices);
    }


    //---------------TAIL ANIMATION---------------------
    void findBones()
    {

        bonesList.Add(tailRoot);

        GetAllChildren(tailRoot,bonesList);

    }
    // this could be jumped into whale class? then build the tail bones inside that and then this can be used to call for update
//-- keeps timer static and on track for everything eles
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
    MotionDataPacket getNextPacket() {
        if (motionDataPacketList.Count == 0) return null;
        MotionDataPacket nextPacket = null;
        if (currentItemIndex < motionDataPacketList.Count){   
            nextPacket = motionDataPacketList[currentItemIndex];
            // Move to the next index for the next frame
            currentItemIndex++;
        } else {
            Debug.Log("Reached end of File");
        }
        return nextPacket;
    }


    // TOGGLE BUTTON FOR USING CSV MOTION DATA - BASICALLY MAKE IT SO THAT THE RANDOM WALK WHCIH PROVIDES PHASE SPEED AND MOUTH STATUS 
    // mouth !!!!!!
    void oldTailUpdate(MotionDataPacket currentPacket) {

        if (useSlerp)
        {
            bodyRoot.transform.rotation = Quaternion.Slerp(bodyRoot.transform.rotation, bodyTargetRotation, Time.fixedDeltaTime * rotationSmoothSpeed);
            tailRoot.transform.localRotation = Quaternion.Slerp(tailRoot.transform.localRotation, tailTargetRotation, Time.fixedDeltaTime * rotationSmoothSpeed);
        }

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
        
        if (useSlerp){
            bodyTargetRotation = Quaternion.Euler(currentPacket.body_signal, 0, 0) * bodyStartRot;
            tailTargetRotation = Quaternion.Euler(currentPacket.fluking_signal*1.1f, 0, 0) * tailStartRot;
        } else {
            bodyRoot.transform.rotation = Quaternion.Euler(currentPacket.body_signal, 0, 0) * bodyStartRot;
            tailRoot.transform.localRotation = Quaternion.Euler(currentPacket.fluking_signal, 0, 0) * tailStartRot;
        }

        if (controls.Player.Reset.triggered){

            bodyRoot.transform.rotation = bodyStartRot;
            tailRoot.transform.localRotation = tailStartRot;

            currentItemIndex = 0;      
        }
    }


    // add thing to turn off next packet stuff from odl update so it doesnt tend towards 0 

    //refacotr to make it so that csv data is not needed !!!!!!


    void FixedUpdate(){

    // Get Next Motion Data Packet
    MotionDataPacket currentPacket = getNextPacket();

  
    // handles the larger motions like tail angle and body angle (needs csv data)
    //oldTailUpdate(currentPacket);


    // updates Amp and Freq of Fluking motion
    if (updateAmpFreq){ setAmplitudeAndFrequencyFromLookUp(currentPacket);} 


    // handes the actual fluking motion along the tail, driven by jays math model
    newTailUpdate();
    }
    
    void setAmplitudeAndFrequencyFromLookUp(MotionDataPacket currentPacket){

        float speed = (float)Math.Round(currentPacket.speed,1);

        
        double[] ampAndFreq = lookUp.lookUp("\"bottom\"", speed, false);

        double found_amplitude = ampAndFreq[0];
        double found_frequency = ampAndFreq[1];

        changeLookUpInstance((float)found_amplitude, (float)found_frequency);
    }
    
    private float _timer = 0f;
 void newTailUpdate()
{

    // GOTTA UPDATE TO INBETWEEN STEPS, MAYE FIXEDUP changes the target amp litude and this happens in update using deltatime ??? 
    _timer += Time.fixedDeltaTime;

    // Tan et al. (2011): q_i(t) = A_i * sin(2π*t/T_i + φ_i) + C_i
    // equation for moving each tail bone based on the wave parameters and the position of the bone along the tail
    float T = 1f / (float)frequency; 
    float cumulativeDistance = 0f;

    for (int i = 0; i < bonesList.Count; i++)
    {   
        // adjusts amplitude to be within range of motion for bone (could try clamping the final angle too??) LOOKS OKAY JUST NEED TO TUNE
        float A_i   = (float)amplitude* boneMaxAngles[i];        
        //   the shift of amount of the wave based on the distance (negative since going backwards) LOWER THIS!!!!!
        float phi_i = -(cumulativeDistance * phaseShiftPerUnit);   
        // static wave offset (not sure if this can be tuned without breaking anything so keeping it 0)
        float C_i   = wave_offset;                                         


        // use tan et al swimming gait formula
        float currentAngle = A_i * Mathf.Sin((2f * Mathf.PI * _timer / T) + phi_i) + C_i;

        // apply to bone (local roation makes it forward kinematic builds on each other)
        bonesList[i].transform.localRotation =
            Quaternion.Euler(currentAngle * Mathf.Rad2Deg, 0f, 0f)
            * boneStart[bonesList[i].name];

        cumulativeDistance += boneLength;
    }
}


// used to swap between the look up instances
public void changeLookUpInstance(float targetAmp, float targetFreq)
{
    amplitude  = Mathf.Lerp((float)amplitude,  targetAmp,  Time.fixedDeltaTime*0.1f);
    frequency  = Mathf.Lerp((float)frequency,  targetFreq, Time.fixedDeltaTime * 0.1f);
}


string determinePhase() {
    string output = null;
    // descent
    if (bodyRoot.transform.localRotation.x >-160.0f) {
        output="\"descent\"";
    //ascent
    } else if (bodyRoot.transform.localRotation.x < -200.0f){
        output="\"ascent\"";
    // bottom (straight on)
    } else {
        output="\"bottom\"";
    }
    return output; 
    }




}

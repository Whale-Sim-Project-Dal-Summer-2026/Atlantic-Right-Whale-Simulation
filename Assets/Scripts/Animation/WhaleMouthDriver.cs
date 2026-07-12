using UnityEngine;
using MotionDataPacketClass;
using System.Collections.Generic;

using System;


public class WhaleMouthDriver: MonoBehaviour
{   
    CameraControls controls;
    private List<MotionDataPacket> motionDataPacketList = new List<MotionDataPacket>();



    public GameObject jawRoot;
    public GameObject topJawRoot;




    Quaternion jawMaxRot = new Quaternion(0.344820946f,-0.0132084452f,-0.0802880526f,0.935135305f);
    Quaternion topJawMaxRot= new Quaternion(-0.18183507f,0.0130922776f,0.0438416749f,0.982263923f);


    Quaternion jawStartRot;
    Quaternion topJawStartRot;


    public float positionSmoothSpeed = 5f;
    public float rotationSmoothSpeed = 5f;


    private int currentItemIndex = 0;

    
    bool isMouthOpen = false;

    [Header("Animation Settings")]
    public AnimationSettings animationSettings;

    public bool updateAmpFreq = false; 

  
    
    void saveBoneStart()
    {
        
        jawStartRot = jawRoot.transform.localRotation;
        topJawStartRot = topJawRoot.transform.localRotation;
    }

    void Start()
    {
        LoadMotionDataCSV();
       
        saveBoneStart();

    }
 void Awake(){

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


    


    // add thing to turn off next packet stuff from odl update so it doesnt tend towards 0 

    //refacotr to make it so that csv data is not needed !!!!!!


    void FixedUpdate(){

    // Get Next Motion Data Packet
    MotionDataPacket currentPacket = getNextPacket();



    updateMouth();
    }
    
    void updateMouth(){

        if (currentItemIndex < motionDataPacketList.Count){
            MotionDataPacket currentPacket = motionDataPacketList[currentItemIndex];

            if (currentPacket.MouthOpen == 1){
                isMouthOpen = true;
            } else {
                isMouthOpen = false;
            }

            if (isMouthOpen){
                jawRoot.transform.localRotation = Quaternion.Slerp(jawRoot.transform.localRotation, jawMaxRot, rotationSmoothSpeed * Time.deltaTime);
                topJawRoot.transform.localRotation = Quaternion.Slerp(topJawRoot.transform.localRotation, topJawMaxRot, rotationSmoothSpeed * Time.deltaTime);
            } else {
                jawRoot.transform.localRotation = Quaternion.Slerp(jawRoot.transform.localRotation, jawStartRot, rotationSmoothSpeed * Time.deltaTime);
                topJawRoot.transform.localRotation = Quaternion.Slerp(topJawRoot.transform.localRotation, topJawStartRot, rotationSmoothSpeed * Time.deltaTime);
            }
        }
    }
    




}

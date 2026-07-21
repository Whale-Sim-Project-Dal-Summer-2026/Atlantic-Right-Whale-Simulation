using System.Collections.Generic;
using UnityEngine;
using System;
using AnimationDataStructs;
using Animation.DataSources;
using AnimationDataStorageManager;
using MotionDataPacketClass;
using FlukeWaveAmplitudeLookUpClass;
using Unity.VisualScripting;
using AGXUnity;
using Animation.DataSources;

public class WhaleMotionFromCSV_AGX : DataSource{

    public List<MotionDataPacket> motionDataPacketList = new List<MotionDataPacket>();
    private float fixedTimeStep = 0.004f;
    public int totalTimesteps = 0;
    private float timer = 0.0f;
    DataStorageManager dataStorageManager;
    CSVLoader cSVLoader; 
    private WhaleAnimationStreamer streamer;
    private bool isWaitingForLoad = false;

    private WhaleState currentWhaleState;
    private WhaleState whaleStartState;
    private WhaleState previousWhaleState;
    private WhaleBlueprint blueprint;
    
    MouthSolver mouthSolver;
    FlukeSolver flukeSolver;
    FinSolver finSolver;
    MainBodySolverAbstract mainBodySolver;
  
     [SerializeField] UserInputManager userInputManager; // neds to be created

    //--Class Specific
    FlukeWaveAmplitudeLookUp lookUp;


    int tailStartIndex = 0;

    int currentTimestep = 0;

    //THIS COUDL BE THE CONSTRUCTOR?????
    public override void LoadSource(AnimationSettings animationSettings, WhaleState startState, WhaleBlueprint blueprint)
    {

        cSVLoader = new CSVLoader();
        // seed motion 
        LoadMotionDataCSV(animationSettings);
        loadFlukeWaveAmplitudeLookUpCSV(animationSettings);


        userInputManager = GameObject.FindAnyObjectByType<UserInputManager>();

        this.blueprint = blueprint;

        // set up storage
        dataStorageManager = new DataStorageManager(blueprint);

        mouthSolver = new MouthSolver(startState.Mouth);
        flukeSolver = new FlukeSolver(blueprint.BodyLengthCount, fixedTimeStep, lookUp, tailStartIndex);
        mainBodySolver = new AGXCSVMainBodySolver(fixedTimeStep, startState, userInputManager.rb);
        finSolver = new FinSolver(startState.LeftFin.Length, fixedTimeStep);
        
        //build states
        //WhaleState[] temp = calculateStates(startState, blueprint);
        
        int currentTotalTimesteps = motionDataPacketList.Count;
        this.totalTimesteps = currentTotalTimesteps;
        
        currentWhaleState = startState;
        whaleStartState = startState;
        this.previousWhaleState = startState;

        //save states
        //dataStorageManager.SaveWhaleAnimationData(temp,Application.dataPath+"/testDATA");

        //start streamer
        //streamer = new WhaleAnimationStreamer(dataStorageManager, Application.dataPath+"/testDATA",
                                               //batchSizeIn: 1500, refillThresholdIn: 500);
        


        //clear data no longer needed 
        cSVLoader = null; 
      
        GC.Collect();
        
    }

    void loadFlukeWaveAmplitudeLookUpCSV(AnimationSettings animationSettings){
        var loaded_CsvData = cSVLoader.loadCSV(animationSettings.FlukeAmpLookUp_csv,animationSettings.FlukeAmp_ContainsHeaders);
        string[][] csvData = loaded_CsvData.data;
        Dictionary<string,int> columnIndices= loaded_CsvData.columnIndices;

        lookUp = new FlukeWaveAmplitudeLookUp(csvData,columnIndices);

    }

     void LoadMotionDataCSV(AnimationSettings animationSettings){
        var loaded_CsvData = cSVLoader.loadCSV(animationSettings.MotionData_csv,animationSettings.MotionData_ContainsHeaders);
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
    }
    WhaleState[] calculateStates(WhaleState startState, WhaleBlueprint blueprint){

        WhaleState previousState = startState;
     
        WhaleState[] output = new WhaleState[motionDataPacketList.Count+1];
        output[0]= startState;
        for (int i = 0; i < motionDataPacketList.Count; i++)
        {
            MotionDataPacket currentPacket = motionDataPacketList[i];

            // Data integrity
            if (float.IsNaN(currentPacket.speed) || float.IsNaN(currentPacket.pitch) ||
                float.IsNaN(currentPacket.head)  || float.IsNaN(currentPacket.roll))
            {
                currentPacket.speed = 0f;
                currentPacket.pitch = 0f;
                currentPacket.head = 0f;
                currentPacket.roll = 0f;
            }

          
            WhaleState newState = new WhaleState(blueprint);
            
            //Calculate Main Body
            newState.Root = mainBodySolver.solveMainBody(currentPacket, previousState.Root);

            if (mainBodySolver is AGXCSVMainBodySolver improvedSolver)
            {
                newState.Head = improvedSolver.getHeadState();
                newState.BodyLength = improvedSolver.getBodyState();
            }
            else
            {
                newState.BodyLength = previousState.BodyLength;
            }
            newState.Mouth = mouthSolver.solveMouth(currentPacket.MouthOpen == 1 ? true : false , previousState.Mouth);
            newState.LeftFin = finSolver.solveFin(currentPacket, true, previousState.LeftFin);
            newState.RightFin = finSolver.solveFin(currentPacket,  false, previousState.RightFin);
            
            // set previous state to prevent compounding fluke calc
            previousState = newState;

            //Calculate Fluke based on body roll state
            newState.BodyLength= flukeSolver.solveFuke(currentPacket, newState);
           
      
            output[i+1] = newState;
            
        }
        return output;
    }
       WhaleState calculateState(){

        
            MotionDataPacket currentPacket = motionDataPacketList[currentTimestep];
            currentTimestep++;

            // Data integrity
            if (float.IsNaN(currentPacket.speed) || float.IsNaN(currentPacket.pitch) ||
                float.IsNaN(currentPacket.head)  || float.IsNaN(currentPacket.roll))
            {
                currentPacket.speed = 0f;
                currentPacket.pitch = 0f;
                currentPacket.head = 0f;
                currentPacket.roll = 0f;
            }

          
            WhaleState newState = new WhaleState(this.blueprint);
            
            //Calculate Main Body
            newState.Root = mainBodySolver.solveMainBody(currentPacket, this.previousWhaleState.Root);

            if (mainBodySolver is AGXCSVMainBodySolver improvedSolver)
            {
                newState.Head = improvedSolver.getHeadState();
                newState.BodyLength = improvedSolver.getBodyState();
            }
            else
            {
                newState.BodyLength = this.previousWhaleState.BodyLength;
            }
            newState.Mouth = mouthSolver.solveMouth(currentPacket.MouthOpen == 1 ? true : false , previousWhaleState.Mouth);
            newState.LeftFin = finSolver.solveFin(currentPacket, true, previousWhaleState.LeftFin);
            newState.RightFin = finSolver.solveFin(currentPacket,  false, this.previousWhaleState.RightFin);
            
            // set previous state to prevent compounding fluke calc
            previousWhaleState = newState;

            //Calculate Fluke based on body roll state
            newState.BodyLength= flukeSolver.solveFuke(currentPacket, newState);
           
      
          
            
        
        return newState;
    }
  
   
    

    public override WhaleState getNextWhaleState()
    {
        WhaleState next = calculateState();
        return next;
    }

    public override void loadWhaleStateAt(int timestep){

        if (timestep != 0) throw new NotImplementedException("Loading a state at a specific timestep is not implemented for WhaleMotionFromCSV_AGX.");

        currentTimestep = timestep;
        previousWhaleState = whaleStartState;
        userInputManager.rb.LinearVelocity = Vector3.zero;
        userInputManager.rb.AngularVelocity = Vector3.zero;

        userInputManager.rb.Native.setPosition(new agx.Vec3(-500, -25, 500));
        userInputManager.rb.Native.setRotation(new agx.Quat(0, 0, 0,1));
        userInputManager.rb.GameObject().transform.position = new Vector3(-500, -25, 500);
        userInputManager.rb.GameObject().transform.rotation = Quaternion.identity;

        
      
        
        
    
    }
    public override int GetTotalTimesteps()
    {
        return this.totalTimesteps;
    }
}
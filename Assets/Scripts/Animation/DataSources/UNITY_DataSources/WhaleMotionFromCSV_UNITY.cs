using System.Collections.Generic;
using UnityEngine;
using System;
using AnimationDataStructs;
using Animation.DataSources;
using AnimationDataStorageManager;
using MotionDataPacketClass;
using FlukeWaveAmplitudeLookUpClass;

public class WhaleMotionFromCSV_UNITY : DataSource{

    public List<MotionDataPacket> motionDataPacketList = new List<MotionDataPacket>();
    private float fixedTimeStep = 0.004f;
    int totalTimesteps = 0;
    private float timer = 0.0f;
    DataStorageManager dataStorageManager;
    CSVLoader cSVLoader; 
    private WhaleAnimationStreamer streamer;
    private bool isWaitingForLoad = false;
    private WhaleState currentWhaleState;
    private WhaleBlueprint blueprint;
    
    MouthSolver mouthSolver;
    FlukeSolver flukeSolver;
    FinSolver finSolver;
    MainBodySolverAbstract mainBodySolver;
  

    //--Class Specific
    FlukeWaveAmplitudeLookUp lookUp;


    int tailStartIndex = 0;

    //THIS COUDL BE THE CONSTRUCTOR?????
    public override void LoadSource(AnimationSettings animationSettings, WhaleState startState, WhaleBlueprint blueprint)
    {

        cSVLoader = new CSVLoader();
        // seed motion 
        LoadMotionDataCSV(animationSettings);
        loadFlukeWaveAmplitudeLookUpCSV(animationSettings);


        this.blueprint = blueprint;

        // set up storage
        dataStorageManager = new DataStorageManager(blueprint);

        mouthSolver = new MouthSolver(startState.Mouth);
        flukeSolver = new FlukeSolver(blueprint.BodyLengthCount, fixedTimeStep, lookUp, tailStartIndex);
        mainBodySolver = new ImprovedMainBodySolver(fixedTimeStep, startState);
        finSolver = new FinSolver(startState.LeftFin.Length, fixedTimeStep);
        
        //build states
        WhaleState[] temp = calculateStates(startState, blueprint);
        totalTimesteps = temp.Length;
        currentWhaleState = startState;

        //save states
        dataStorageManager.SaveWhaleAnimationData(temp,Application.dataPath+"/testDATA");

        //start streamer
        streamer = new WhaleAnimationStreamer(dataStorageManager, Application.dataPath+"/testDATA",
                                               batchSizeIn: 1500, refillThresholdIn: 500);
        


        //clear data no longer needed 
        motionDataPacketList = null;
        cSVLoader = null; 
        temp = null;
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

            if (mainBodySolver is ImprovedMainBodySolver improvedSolver)
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
  
   
    

    public override WhaleState getNextWhaleState()
    {
         // if waiting for background load of a state jump, just return the last state until the new state is ready
        if (isWaitingForLoad) {
    
            if (!streamer.IsLoading && streamer.TryGetNextState(out var state)) {
                //data has loaded, update the current state and stop waiting
                currentWhaleState = state;
                isWaitingForLoad = false; 
                return state;
           
            } else {
            
                return currentWhaleState;
            }
        // normal play back not waiting for loading 
        } else {
            if (streamer.TryGetNextState(out var state)) {
               return state;
            } else {
                Debug.LogWarning("Streamer error or no more states available");
                return new WhaleState(blueprint);
            }
        }   
    }

    public override void loadWhaleStateAt(int timestep){
       
        streamer.SeekTo(timestep);
        isWaitingForLoad = true; 
       
    }
    public override int GetTotalTimesteps()
    {
        return this.totalTimesteps;
    }
}
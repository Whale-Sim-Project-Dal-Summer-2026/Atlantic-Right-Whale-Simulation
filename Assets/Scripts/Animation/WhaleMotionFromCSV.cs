using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AnimationDataStructs;
using DataSources;
using AnimationDataStorageManager;
using static WhaleAnimationStreamer;
using MotionDataPacketClass;
using FlukeWaveAmplitudeLookUpClass;
using static CSVLoader;
using System.Reflection;


public class WhaleMotionFromCSV : DataSource
{
    private int[] cols = { 0, 1, 8, 9, 10, 12, 14, 23 };

    public List<MotionDataPacket> motionDataPacketList = new List<MotionDataPacket>();
    private float fixedTimeStep = 0.004f;
    private float timer = 0.0f;
    DataStorageManager dataStorageManager;
    CSVLoader cSVLoader; 
    private WhaleAnimationStreamer streamer;
    private bool isWaitingForLoad = false;
    private WhaleState currentWhaleState;
    private WhaleState startState; 
    private WhaleBlueprint blueprint;
    
    MouthSolver mouthSolver;

    //--Class Specific
    FlukeWaveAmplitudeLookUp lookUp;

    private float phaseShiftPerUnit = 1.25f;
    private float wave_offset = 0 ;
    private float boneLength = 0.1f;
  public List<float> boneMaxAngles = new List<float>
{
    0.1f, 
    0.1f, 
    0.1f, 
    0.1f, 
    0.15f, 
    0.15f, 
    0.15f, 
    0.15f, 
    0.2f, 
    0.2f, 
    0.2f, 
    0.2f, 
    0.3f, 
    0.3f, 
    0.8f,
    0.15f, 
    0.15f, 
    0.15f, 
    0.15f,
    0.15f, 
    0.15f, 
    0.15f,
    0.15f, 
    0.15f, 
    0.15f,
    0.15f, 
    0.15f, 
    0.15f,
    0.15f, 
    0.15f 
};


    //THIS NEEDS TO GO
    public double amplitude = 0.0101001492483101;
    public double frequency = 0.142049987510281;


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
        //build states
        WhaleState[] temp = calculateStates(startState, blueprint);
        currentWhaleState = startState;
        this.startState = startState;

        //save states
        dataStorageManager.SaveWhaleAnimationData(temp,Application.dataPath+"/testDATA");

        //start streamer
        streamer = new WhaleAnimationStreamer(dataStorageManager, Application.dataPath+"/testDATA",
                                               batchSizeIn: 1500, refillThresholdIn: 500);
        


        //clear data no longer needed 
        // motionDataPacketList = null;
        // cSVLoader = null; 
        // temp = null;
        // GC.Collect();
        
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
        Vector3 targetPosition   = startState.MainBody.Position;
        Quaternion targetRotation = startState.MainBody.Rotation;
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

            targetRotation  = Quaternion.Euler(currentPacket.pitch, currentPacket.head, currentPacket.roll);
            targetPosition += targetRotation * Vector3.forward * currentPacket.speed * (fixedTimeStep * 10);
            targetPosition.y = 75f - currentPacket.depth;

            WhaleState newState = new WhaleState(blueprint);
            newState.MainBody.Position = Vector3.Lerp(previousState.MainBody.Position, targetPosition,fixedTimeStep* 0.5f);
            newState.MainBody.Rotation = Quaternion.Slerp(previousState.MainBody.Rotation, targetRotation, fixedTimeStep * 0.5f);

            setAmplitudeAndFrequencyFromLookUp(currentPacket);

            //Calculate Fluke
            newState.Tail = calculateFlukeStates(currentPacket,startState);
            //Solve Mouth State
            newState.Mouth = mouthSolver.solveMouth(currentPacket.MouthOpen == 1 ? true : false , previousState.Mouth);

            output[i+1] = newState;
            previousState = newState;
        }
        return output;
    }
    void setAmplitudeAndFrequencyFromLookUp(MotionDataPacket currentPacket){

        float speed = (float)Math.Round(currentPacket.speed,1);

        
        double[] ampAndFreq = lookUp.lookUp("\"bottom\"", speed, false);

        double found_amplitude = ampAndFreq[0];
        double found_frequency = ampAndFreq[1];

        changeLookUpInstance((float)found_amplitude, (float)found_frequency);
    }

    LocalRotation_AnimationData[] calculateFlukeStates(MotionDataPacket packet, WhaleState startState){

        LocalRotation_AnimationData[] flukeState = new LocalRotation_AnimationData[blueprint.TailCount];
        
        // GOTTA UPDATE TO INBETWEEN STEPS, MAYE FIXEDUP changes the target amp litude and this happens in update using deltatime ??? 
        timer += fixedTimeStep;

        // Tan et al. (2011): q_i(t) = A_i * sin(2π*t/T_i + φ_i) + C_i
        // equation for moving each tail bone based on the wave parameters and the position of the bone along the tail
        float T = 1f / (float)frequency; 
        float cumulativeDistance = 0f;

        for (int i = 0; i < blueprint.TailCount; i++){

            // adjusts amplitude to be within range of motion for bone (could try clamping the final angle too??) LOOKS OKAY JUST NEED TO TUNE
            float A_i   = (float)amplitude* boneMaxAngles[i];        
            //   the shift of amount of the wave based on the distance (negative since going backwards) LOWER THIS!!!!!
            float phi_i = -(cumulativeDistance * phaseShiftPerUnit);   
            // static wave offset (not sure if this can be tuned withouxt breaking anything so keeping it 0)
            float C_i   = wave_offset;                                         


            // use tan et al swimming gait formula
            float currentAngle = A_i * Mathf.Sin((2f * Mathf.PI * timer / T) + phi_i) + C_i;

            // apply to bone (local roation makes it forward kinematic builds on each other)
            flukeState[i].Rotation =
                Quaternion.Euler(currentAngle * Mathf.Rad2Deg, 0f, 0f)
                * startState.Tail[i].Rotation;

            cumulativeDistance += boneLength;
        }
        return flukeState; 

    }
    
    // used to swap between the look up instances
    public void changeLookUpInstance(float targetAmp, float targetFreq){
        amplitude  = Mathf.Lerp((float)amplitude,  targetAmp,  Time.fixedDeltaTime*0.1f);
        frequency  = Mathf.Lerp((float)frequency,  targetFreq, Time.fixedDeltaTime * 0.1f);
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

    public override void loadWhaleStateAt(int timestep)
    {
        streamer.SeekTo(timestep);
        isWaitingForLoad = true; 
        
    }
}
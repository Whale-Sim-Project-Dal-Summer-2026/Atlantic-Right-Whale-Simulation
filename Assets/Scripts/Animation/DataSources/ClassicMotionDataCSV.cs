using MotionDataPacketClass;
using AnimationDataStructs; 
using DataSources;
using UnityEngine;
using System.Collections.Generic;
using AnimationDataStorageManager;

// classic means no body movement just the up and down
// made from refacotring original code from v0 for testing 
public class ClassicMotionDataCSV : DataSource
{
    private int[] cols = { 0, 1, 8, 9, 10, 12, 14, 23 };

    public List<MotionDataPacket> motionDataPacketList = new List<MotionDataPacket>();

    DataStorageManager dataStorageManager;

    int stateCount;
    int chunkSize;
    private WhaleAnimationStreamer streamer;
    private bool isWaitingForLoad = false;
    private WhaleState currentWhaleState;

    public override void LoadSource(AnimationSettings animationSettings, WhaleState startState, WhaleBlueprint blueprint)
    {
        LoadCSV(animationSettings.MotionData_csv);
       
        dataStorageManager = new DataStorageManager(blueprint);
        WhaleState[] temp = calculateStates(startState, blueprint);
        currentWhaleState = startState;
        dataStorageManager.SaveWhaleAnimationData(temp,Application.dataPath+"/testDATA");
        streamer = new WhaleAnimationStreamer(dataStorageManager, Application.dataPath+"/testDATA",
                                               batchSizeIn: 1500, refillThresholdIn: 500);
        

    }

    void LoadCSV(TextAsset csvData)
    {
        string[] lines = csvData.text.Split(new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');

            if (values.Length >= cols.Length)
            {
                MotionDataPacket dataPacket = new MotionDataPacket();

                dataPacket.timestep       = float.Parse(values[cols[0]]);
                dataPacket.depth          = float.Parse(values[cols[1]]);
                dataPacket.head           = float.Parse(values[cols[2]]) * Mathf.Rad2Deg;
                dataPacket.pitch          = -float.Parse(values[cols[3]]) * Mathf.Rad2Deg;
                dataPacket.roll           = float.Parse(values[cols[4]]) * Mathf.Rad2Deg;
                dataPacket.speed          = values[cols[5]] == "NaN" ? 0f : float.Parse(values[cols[5]]);
                dataPacket.fluking_signal = values[cols[6]] == "NaN" ? 0f : float.Parse(values[cols[6]]);
                dataPacket.MouthOpen      = int.Parse(values[cols[7]]);

                motionDataPacketList.Add(dataPacket);
            }
        }
    }

    WhaleState[] calculateStates(WhaleState startState, WhaleBlueprint blueprint)
    {
        WhaleState previousState = startState;
        Vector3 targetPosition   = startState.Root.Position;
        Quaternion targetRotation = startState.Root.Rotation;
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
                currentPacket.head  = 0f;
                currentPacket.roll  = 0f;
            }

            targetRotation  = Quaternion.Euler(currentPacket.pitch, currentPacket.head, currentPacket.roll);
            targetPosition += targetRotation * Vector3.forward * currentPacket.speed * (0.004f * 10);
            targetPosition.y = 75f - currentPacket.depth;

            WhaleState newState = new WhaleState(blueprint);
            newState.Root.Position = Vector3.Lerp(previousState.Root.Position, targetPosition, 0.004f * 0.5f);
            newState.Root.Rotation = Quaternion.Slerp(previousState.Root.Rotation, targetRotation, 0.004f * 0.5f);

            output[i+1] = newState;
            previousState = newState;
        }
        return output;
    }

    public override WhaleState getNextWhaleState(){
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
                return new WhaleState(new WhaleBlueprint(0, 0, 0, 0, 1,0,0));
            }
        }   
    }

    public override void loadWhaleStateAt(int timestep){
        streamer.SeekTo(timestep);
        isWaitingForLoad = true; 
        
    }
 
}

using MotionDataPacketClass;
using AnimationDataStructs; 
using DataSources;
using UnityEngine;
using Unity.Mathematics;
using System.Collections.Generic;


//classic means no body movement just the up and down
// made from refacotring original code from v0 for testing 
public class ClassicMotionDataCSV : DataSource
{
    private int[] cols = { 0, 1, 8, 9, 10, 12, 14, 23 };

    WhaleState[] states;
    List<MotionDataPacket> motionDataPacketList = new List<MotionDataPacket>();

    public override void LoadSource(TextAsset file, WhaleState startState, WhaleBlueprint blueprint)
    {
        LoadCSV(file);
        states = new WhaleState[motionDataPacketList.Count + 1];
        states[0] = startState;
        calculateStates(startState, blueprint);
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

    void calculateStates(WhaleState startState, WhaleBlueprint blueprint)
    {
        WhaleState previousState = startState;
        Vector3 targetPosition   = startState.MainBody[0].Position;
        Quaternion targetRotation = startState.MainBody[0].Rotation;

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
            newState.MainBody[0].Position = Vector3.Lerp(previousState.MainBody[0].Position, targetPosition, 0.004f * 0.5f);
            newState.MainBody[0].Rotation = Quaternion.Slerp(previousState.MainBody[0].Rotation, targetRotation, 0.004f * 0.5f);

            states[i + 1] = newState;
            previousState = newState;
        }
    }

    public override WhaleState getNextWhaleState(int currentTimeStep)
    {
        return states[currentTimeStep];
    }

    public override WhaleState getWhaleStateAt(int timestep)
    {
        throw new System.NotImplementedException();
    }
}
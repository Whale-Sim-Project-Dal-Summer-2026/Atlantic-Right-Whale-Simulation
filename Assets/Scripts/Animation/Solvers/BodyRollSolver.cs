using AnimationDataStructs;
using MotionDataPacketClass;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BodyRollSolver
{
    float fixedTimeStep;

    public float rootTurnSpeed = .5f;

    public float headTurnSpeed = 0.1f;
    public float depthAdjustmentSpeed = 0.1f;

    public float followRotationSpeed = 16f;
    public float segmentSpacing = 0.1f;

    public float responseAtHead = 1f;

    public float responseAtTail = 0.3f;

    public BodyRollSolver(float fixedTimeStepIn)
    {
        fixedTimeStep = fixedTimeStepIn;
    }

    private WhaleState solveBodyRoll(WhaleState previousState, Global_AnimationData solvedRoot, WhaleBlueprint blueprint, float lastTravelDistance)
    {
        WhaleState newState = new WhaleState(blueprint);

        
        Quaternion desiredWorldHeading = solvedRoot.Rotation;

      
        Quaternion rootRotationNow = Quaternion.Slerp(
            previousState.Root.Rotation, desiredWorldHeading, rootTurnSpeed
        );

        Vector3 recalculatedPosition = previousState.Root.Position + (rootRotationNow * Vector3.forward * lastTravelDistance);
      
        
        recalculatedPosition.y = Mathf.Lerp(
            previousState.Root.Position.y,
            solvedRoot.Position.y,
            depthAdjustmentSpeed * fixedTimeStep
        );

        newState.Root = new Global_AnimationData
        {
            Position = recalculatedPosition,
            Rotation = rootRotationNow
        };

        Quaternion headWorldPrev = previousState.Root.Rotation * previousState.Head.Rotation;
      
        Quaternion headWorldNow = Quaternion.Slerp(
            headWorldPrev, 
            desiredWorldHeading, 
            headTurnSpeed * fixedTimeStep
        );
        
        Quaternion headLocalNow = Quaternion.Inverse(newState.Root.Rotation) * headWorldNow;

       
        newState.Head = new LocalRotation_AnimationData { Rotation = headLocalNow };

        newState.BodyLength = new LocalRotation_AnimationData[previousState.BodyLength.Count()];

        for (int i = 0; i < newState.BodyLength.Count(); i++)
        {
            newState.BodyLength[i] = new LocalRotation_AnimationData { Rotation = previousState.BodyLength[i].Rotation };
        }

        Quaternion parentWorldPrev = headWorldPrev;
        Quaternion parentWorldNow = headWorldNow;


        // minus 8 to not roll the tail segments
        int n = previousState.BodyLength.Count() - 8;
        for (int i = 0; i < n; i++)
        {   
            // segment count 
            float sc = n > 1 ? (float)i / (n - 1) : 0f;
            float rotationCurve = Mathf.Lerp(responseAtHead, responseAtTail, sc);

            Quaternion segWorldPrev = parentWorldPrev * previousState.BodyLength[i].Rotation;
            Quaternion segWorldNow = Quaternion.Slerp(
                segWorldPrev, parentWorldNow, followRotationSpeed * rotationCurve * fixedTimeStep
            );

            Quaternion segLocalNow = Quaternion.Inverse(parentWorldNow) * segWorldNow;


            newState.BodyLength[i] = new LocalRotation_AnimationData { Rotation = segLocalNow };

            parentWorldPrev = segWorldPrev;
            parentWorldNow = segWorldNow;
        }

        return newState;
    }
}
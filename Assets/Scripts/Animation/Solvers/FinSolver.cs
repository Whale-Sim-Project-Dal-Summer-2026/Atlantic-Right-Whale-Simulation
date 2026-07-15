using AnimationDataStructs;
using MotionDataPacketClass;
using UnityEngine;

public class FinSolver {

    float fixedTimeStep;
    int finLength;

    float switchSpeed = 0.1f;


    //hard coded rotation tarets
   

    Quaternion LeftMaxRotationTarget = new Quaternion(0.0901189223f, 0.203842238f, 0.0354886539f, 0.974200904f);
    Quaternion LeftMinRotationTarget = new Quaternion(0.0978288874f, 0.224736199f, -0.053717792f, 0.968007088f);

  
    Quaternion RightMaxRotationTarget = Quaternion.Inverse(new Quaternion(0.0901189223f, 0.203842238f, 0.0354886539f, 0.974200904f));
    Quaternion RightMinRotationTarget = Quaternion.Inverse(new Quaternion(0.0978288874f, 0.224736199f, -0.053717792f, 0.968007088f));


     //plan to include somesort of scaler based on the heading/pitch 
    float timer = 0f;

    public FinSolver(int finLength, float fixedTimeStepIn) {
        fixedTimeStep = fixedTimeStepIn;
        this.finLength = finLength;
    }

    public LocalRotation_AnimationData[] solveFin(MotionDataPacket currentPacket, bool isLeft, LocalRotation_AnimationData[] previousState) {

        LocalRotation_AnimationData[] currentFinStates = new LocalRotation_AnimationData[finLength];

        timer += fixedTimeStep;

        // sinwave to move between the two exremetes of the fin rotation (0-1)
        float wave = (Mathf.Sin(timer * switchSpeed * Mathf.PI * 2f) + 1f) * 0.5f;

        Quaternion minTarget = isLeft ? LeftMinRotationTarget : RightMinRotationTarget;
        Quaternion maxTarget = isLeft ? LeftMaxRotationTarget : RightMaxRotationTarget;

        LocalRotation_AnimationData current = new LocalRotation_AnimationData {
            Rotation = Quaternion.Slerp(minTarget, maxTarget, wave)
        };

        // dont adjust first since it leads to mesh deformation
        currentFinStates[0] = current;

        //everything else the same too
        for (int i = 2; i < finLength; i++) {
            currentFinStates[i] = new LocalRotation_AnimationData { Rotation = previousState[i].Rotation };
        }

        return currentFinStates;
    }
}
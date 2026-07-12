using UnityEngine;
using System.Collections;
using AnimationDataStructs;
using MotionDataPacketClass;
using NUnit.Framework;
public class MouthSolver {
   
    Quaternion topMaxRot= new Quaternion(-0.18183507f,0.0130922776f,0.0438416749f,0.982263923f);
    Quaternion bottomMaxRot = new Quaternion(0.344820946f,-0.0132084452f,-0.0802880526f,0.935135305f);
  
    Quaternion topStart;
    Quaternion bottomStart;
    float positionSmoothSpeed = 5f;
    float rotationSmoothSpeed = 5f;

    bool isMouthOpen = false;
   
    

    public MouthSolver(LocalRotation_AnimationData[] startingMouthState){
        isMouthOpen = false;
        topStart = startingMouthState[0].Rotation;
        bottomStart = startingMouthState[1].Rotation;

    }
    

    public LocalRotation_AnimationData[] solveMouth(bool isMouthOpenIn, LocalRotation_AnimationData[] currentMouthState){
        LocalRotation_AnimationData[] solvedMouthData = new LocalRotation_AnimationData[2];

        isMouthOpen = isMouthOpenIn;

        if (isMouthOpen){
            solvedMouthData[0].Rotation = Quaternion.Slerp(currentMouthState[0].Rotation, topMaxRot, rotationSmoothSpeed * Time.deltaTime);
            solvedMouthData[1].Rotation = Quaternion.Slerp(currentMouthState[1].Rotation, bottomMaxRot, rotationSmoothSpeed * Time.deltaTime);
        } else {
            solvedMouthData[0].Rotation = Quaternion.Slerp(currentMouthState[0].Rotation, topStart, rotationSmoothSpeed * Time.deltaTime);
            solvedMouthData[1].Rotation = Quaternion.Slerp(currentMouthState[1].Rotation, bottomStart, rotationSmoothSpeed * Time.deltaTime);
        }

        return solvedMouthData;
    }
    


}
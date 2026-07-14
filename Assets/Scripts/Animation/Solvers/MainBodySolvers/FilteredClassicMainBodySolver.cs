using AnimationDataStructs;
using UnityEngine;
using MotionDataPacketClass;

public class FilteredClassicMainBodySolver : MainBodySolverAbstract {
    private float fixedTimeStep;
    Vector3 targetPosition;
    Quaternion targetRotation;
    private float filteredDepth;
    private Quaternion filteredRotation;
    private float depthFilter = 0.01f;    
    private float rotationFilter= .01f; 

    public FilteredClassicMainBodySolver(float fixedTimeStepIn, Global_AnimationData startState){
        fixedTimeStep = fixedTimeStepIn;
        this.targetPosition = startState.Position;
        this.targetRotation = startState.Rotation;
        this.filteredDepth = startState.Position.y;
        this.filteredRotation = startState.Rotation;
    }

    public override Global_AnimationData solveMainBody(MotionDataPacket currentPacket, Global_AnimationData previousState){
        Global_AnimationData newState = new Global_AnimationData();

        Quaternion rawRotation = Quaternion.Euler(currentPacket.pitch, currentPacket.head, currentPacket.roll);

        // filter to fix jittery motion data 
        filteredRotation = Quaternion.Slerp(filteredRotation, rawRotation, rotationFilter);
        targetRotation = filteredRotation;

        targetPosition += targetRotation * Vector3.forward * currentPacket.speed * (fixedTimeStep*10);

        filteredDepth = Mathf.Lerp(filteredDepth, currentPacket.depth, depthFilter);
        targetPosition.y = 75f - filteredDepth;

        // Lerp and Slerp toward filtered targets
        newState.Position = Vector3.Lerp(previousState.Position, targetPosition, fixedTimeStep * 0.5f);
        newState.Rotation = Quaternion.Slerp(previousState.Rotation, targetRotation, fixedTimeStep * 0.5f);

        return newState;
    }
}
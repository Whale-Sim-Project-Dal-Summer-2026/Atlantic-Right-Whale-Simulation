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

    public FilteredClassicMainBodySolver(float fixedTimeStepIn, WhaleState startState){
        fixedTimeStep = fixedTimeStepIn;
        this.targetPosition = startState.Root.Position;
        this.targetRotation = startState.Root.Rotation;
        this.filteredDepth = startState.Root.Position.y;
        this.filteredRotation = startState.Root.Rotation;
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
      public override void resetSolver(WhaleState startState) {
        this.targetPosition = startState.Root.Position;
        this.targetRotation = startState.Root.Rotation;
        this.filteredDepth = startState.Root.Position.y;
        this.filteredRotation = startState.Root.Rotation;
    }
}
using AnimationDataStructs;
using UnityEngine;
using MotionDataPacketClass;
public class ClassicMainBodySolver : MainBodySolverAbstract {
    private float fixedTimeStep;

    Vector3 targetPosition;
    Quaternion targetRotation;
    public ClassicMainBodySolver(float fixedTimeStepIn, WhaleState startState){
        fixedTimeStep = fixedTimeStepIn;
        this.targetPosition = startState.Root.Position;
        this.targetRotation = startState.Root.Rotation;
     
    }

    public override Global_AnimationData solveMainBody(MotionDataPacket currentPacket, Global_AnimationData previousState){
        Global_AnimationData newState = new Global_AnimationData();

        // Calculate tagrets
        targetRotation  = Quaternion.Euler(currentPacket.pitch, currentPacket.head, currentPacket.roll);
        targetPosition += targetRotation * Vector3.forward * currentPacket.speed * (fixedTimeStep * 10);
        targetPosition.y = 75f - currentPacket.depth;

        // Lerp and Slerp 
        newState.Position = Vector3.Lerp(previousState.Position, targetPosition,fixedTimeStep* 0.5f);
        newState.Rotation = Quaternion.Slerp(previousState.Rotation, targetRotation, fixedTimeStep * 0.5f);

        return newState;
    }
      public override void resetSolver(WhaleState startState) {
        this.targetPosition = startState.Root.Position;
        this.targetRotation = startState.Root.Rotation;
    }
}
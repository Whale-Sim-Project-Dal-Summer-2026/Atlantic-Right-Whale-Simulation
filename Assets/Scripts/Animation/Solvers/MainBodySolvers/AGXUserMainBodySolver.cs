using AnimationDataStructs;
using UnityEngine;
using MotionDataPacketClass;
using AGXUnity.Utils;

public class AGXUserMainBodySolver : MainBodySolverAbstract {
    private float fixedTimeStep;

    Vector3 targetPosition;
    Quaternion targetRotation;



    //--Body Roll Tail State
    WhaleState currentState;
    
    private float rootTurnSpeed = 150f;

    private float headTurnSpeed = 1f;

    private float followRotationSpeed = 8f;

    private float responseAtHead = 1f;

    private float responseAtTail = 0.3f;

    AGXUnity.RigidBody rb;



    public AGXUserMainBodySolver(float fixedTimeStepIn, WhaleState startState, AGXUnity.RigidBody rb){
        fixedTimeStep = fixedTimeStepIn;
        this.targetRotation = startState.Root.Rotation;
        this.targetPosition = startState.Root.Position;
        this.rb = rb;
        currentState = startState;

    }

    // combines filtered classic main body solver with body roll solver for better motion 
    public override Global_AnimationData solveMainBody(MotionDataPacket currentPacket, Global_AnimationData previousState){
        Global_AnimationData solvedRoot = classicMainBodySolve(currentPacket, previousState);
        currentState = solveBodyRoll(solvedRoot);
        return currentState.Root;
    }
    

    // saves the internal position and travel distance for the body roll solver to use
    private Global_AnimationData classicMainBodySolve ( MotionDataPacket currentPacket, Global_AnimationData previousState){

        Global_AnimationData newState = new Global_AnimationData();

        Vector3 currentRbPosition = rb.Native.getPosition().ToHandedVector3();


        targetRotation = Quaternion.Euler(currentPacket.pitch, currentPacket.head, currentPacket.roll);

        // Lerp and Slerp toward filtered targets
        newState.Speed = currentPacket.speed;

        newState.Rotation = Quaternion.Slerp(previousState.Rotation, targetRotation, fixedTimeStep * 0.8f);

        Vector3 forward = newState.Rotation * Vector3.forward;

        newState.Position = currentRbPosition + (newState.Speed * forward * fixedTimeStep);

        Debug.DrawLine(newState.Position, newState.Position + (forward * newState.Speed), Color.red);

        return newState;
    }

    //getters for outputting the new body roll state to the whale model
    public LocalRotation_AnimationData[] getBodyState(){
        return currentState.BodyLength;
    }
    public LocalRotation_AnimationData getHeadState(){
        return currentState.Head;
    }
    // body roll solver
    private WhaleState solveBodyRoll(Global_AnimationData solvedRoot)
    {
        // create a new state based on the current state
        WhaleState newState = new WhaleState(currentState);
        WhaleState previousState = currentState;
        
        Quaternion desiredWorldHeading = solvedRoot.Rotation;

      
        Quaternion rootRotationNow = Quaternion.Slerp(
            previousState.Root.Rotation, 
            desiredWorldHeading, 
            rootTurnSpeed * fixedTimeStep
        );

    
    
        newState.Root = new Global_AnimationData
        {
            Position = solvedRoot.Position, 
            Rotation = rootRotationNow
        };
    
        Quaternion headWorldPrev = previousState.Root.Rotation *  previousState.Head.Rotation;
      
        Quaternion headWorldNow = Quaternion.Slerp(
            headWorldPrev, 
            desiredWorldHeading, 
            headTurnSpeed * fixedTimeStep
        );
        
        Quaternion headLocalNow = Quaternion.Inverse(newState.Root.Rotation) * headWorldNow;

       
        newState.Head = new LocalRotation_AnimationData { Rotation = headLocalNow };

        newState.BodyLength = new LocalRotation_AnimationData[previousState.BodyLength.Length];

        for (int i = 0; i < newState.BodyLength.Length; i++)
        {
            newState.BodyLength[i] = new LocalRotation_AnimationData { Rotation = previousState.BodyLength[i].Rotation };
        }

        Quaternion parentWorldPrev = headWorldPrev;
        Quaternion parentWorldNow = headWorldNow;


        // minus 8 to not roll the tail segments
        int n = previousState.BodyLength.Length - 8;
        for (int i = 0; i < n; i++)
        {
            float t = n > 1 ? (float)i / (n - 1) : 0f;
            float rotationCurve = Mathf.Lerp(responseAtHead, responseAtTail, t);

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
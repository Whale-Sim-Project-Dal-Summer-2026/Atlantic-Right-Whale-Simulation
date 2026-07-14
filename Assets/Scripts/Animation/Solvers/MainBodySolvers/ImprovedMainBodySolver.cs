using AnimationDataStructs;
using UnityEngine;
using MotionDataPacketClass;
using Unity.VisualScripting;

public class ImprovedMainBodySolver : MainBodySolverAbstract {
    private float fixedTimeStep;
    Vector3 targetPosition;
    Quaternion targetRotation;

    private float filteredDepth;
    private Quaternion filteredRotation;
    private Vector3 interalClassicPosition;
    private float lastTravelDistance;

    private float depthFilter = 0.01f;    
    private float rotationFilter= .01f; 


    //--Body Roll Tail State
    WhaleState currentState;
    
    private float rootTurnSpeed = 150f;

    private float headTurnSpeed = 1f;
    private float depthAdjustmentSpeed = 1.25f;

    private float followRotationSpeed = 8f;

    private float responseAtHead = 1f;

    private float responseAtTail = 0.3f;


    public ImprovedMainBodySolver(float fixedTimeStepIn, WhaleState startState){
        fixedTimeStep = fixedTimeStepIn;
        this.targetPosition = startState.Root.Position;
        this.targetRotation = startState.Root.Rotation;
        this.filteredDepth = startState.Root.Position.y;
        this.filteredRotation = startState.Root.Rotation;
        this.interalClassicPosition = startState.Root.Position;
        currentState = startState;
    }

    // combines filtered classic main body solver with body roll solver for better motion 
    public override Global_AnimationData solveMainBody(MotionDataPacket currentPacket, Global_AnimationData previousState){
        (Global_AnimationData solvedRoot, float lastTravelDistance) = classicMainBodySolve(currentPacket, previousState);
        currentState = solveBodyRoll(solvedRoot);
        return currentState.Root;
    }
    

    // saves the internal position and travel distance for the body roll solver to use
    private (Global_AnimationData, float) classicMainBodySolve ( MotionDataPacket currentPacket, Global_AnimationData previousState){
        Global_AnimationData newState = new Global_AnimationData();

        Quaternion rawRotation = Quaternion.Euler(currentPacket.pitch, currentPacket.head, currentPacket.roll);

        // filter to fix jittery motion data 
        filteredRotation = Quaternion.Slerp(filteredRotation, rawRotation, rotationFilter);
        targetRotation = filteredRotation;

        targetPosition += targetRotation * Vector3.forward * currentPacket.speed * (fixedTimeStep*10);

        filteredDepth = Mathf.Lerp(filteredDepth, currentPacket.depth, depthFilter);
        targetPosition.y = 75f - filteredDepth;

        // Lerp and Slerp toward filtered targets
        newState.Position = Vector3.Lerp(interalClassicPosition, targetPosition, fixedTimeStep * 0.5f);
        newState.Rotation = Quaternion.Slerp(previousState.Rotation, targetRotation, fixedTimeStep * 0.5f);

        lastTravelDistance = Vector3.Distance(newState.Position, interalClassicPosition);
        interalClassicPosition = newState.Position;
        return (newState, lastTravelDistance);
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
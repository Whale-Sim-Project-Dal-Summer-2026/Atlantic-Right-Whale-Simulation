// abstract verison of the whale model so the rive ronly has to pass in a new state and the whale modle will update to it
using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using AnimationDataStructs;
using System.Linq;
using AGXUnity.IO.OpenPLX;
using AGXUnity.Utils;

/// <summary>
/// Makes use of AGX rigid body to move the whale using forces calculated based on its speed and root forward dircitonb 
/// </summary>
/// 

public class WhaleModelAGX : WhaleModelAbstract {

    //--Bone Counts--
    int rootBoneCount;
    int tailBoneCount;
    int leftFinBoneCount;
    int rightFinBoneCount;
    int mouthBoneCount;
    int headBoneCount;
    int tailStartIndex;
   
    //--Bone Lists--
    GameObject rootBone;
    GameObject headBone;
    GameObject tailStartBone;
    List<GameObject> bodyLengthBones;
    List<GameObject> leftFinBones;
    List<GameObject> rightFinBones;
    List<GameObject> mouthBones;

    AGXUnity.RigidBody rootRB;

    int bodyEndIndex;
    int startTailIndex;
    int tailEndIndex;
    int leftFlukeEndIndex;
    int leftFlukeStartIndex;

    float seaLevel;

    public WhaleModelAGX(WhaleBones whaleBones){
        bodyLengthBones = new List<GameObject>();
        leftFinBones = new List<GameObject>();
        rightFinBones = new List<GameObject>();
        mouthBones = new List<GameObject>();

        InitializeBones(whaleBones);

        rootBoneCount = 1;
        tailBoneCount = bodyLengthBones.Count;
        leftFinBoneCount = leftFinBones.Count;
        rightFinBoneCount = rightFinBones.Count;
        mouthBoneCount = mouthBones.Count;
        rootBone = whaleBones.rootBone;
        headBoneCount = 1;

        rootRB = rootBone.GetComponentInChildren<AGXUnity.RigidBody>().GetInitialized<AGXUnity.RigidBody>();
        seaLevel = 0.0f;

    }
    void InitializeBones(WhaleBones blueprintSettings){

        GetAllChildren(blueprintSettings.tailStartBone, bodyLengthBones, blueprintSettings.tailStopBone);
        GetAllChildren(blueprintSettings.leftFinStartBone, leftFinBones, blueprintSettings.leftFinStopBone);
        GetAllChildren(blueprintSettings.rightFinStartBone, rightFinBones, blueprintSettings.rightFinStopBone);
        mouthBones.Add(blueprintSettings.mouthTopBone);
        mouthBones.Add(blueprintSettings.mouthBottomBone);
        headBone = blueprintSettings.headBone;
        tailStartBone = blueprintSettings.tailStartBone;

    }
    void GetAllChildren(GameObject parent, List<GameObject> addingList, GameObject stopObject){
        foreach (Transform child in parent.transform){
            // skips colldiers 
            if (child.name == "Colliders"|| child.name == "Collider"){ continue;}

            if (child == tailStartBone) {
                tailStartIndex = addingList.Count;
            }

            addingList.Add(child.gameObject);

            if (child.gameObject == stopObject)
            {
                break;
            }
            // Recursively add this child's children
            GetAllChildren(child.gameObject,addingList,stopObject);
        }
   
    }

    public override WhaleBlueprint getBlueprint(){
        return new WhaleBlueprint(tailBoneCount, mouthBoneCount, leftFinBoneCount, rightFinBoneCount,rootBoneCount,headBoneCount, tailStartIndex);
    }

    public override WhaleState getCurrentState(){
        return new WhaleState(castGameObjectsToWhaleState());
    }

    // cast the current gameobject transforms to a whale state object
    (Global_AnimationData, LocalRotation_AnimationData[], LocalRotation_AnimationData[], LocalRotation_AnimationData[], LocalRotation_AnimationData[], LocalRotation_AnimationData) castGameObjectsToWhaleState(){
        Global_AnimationData rootState = new Global_AnimationData();
        rootState.Position = rootRB.Native.getPosition().ToVector3();
        rootState.Rotation = rootRB.Native.getRotation().ToHandedQuaternion();

        LocalRotation_AnimationData[] bodyLengthStates = new LocalRotation_AnimationData[bodyLengthBones.Count];
        for (int i = 0; i < bodyLengthBones.Count; i++)
        {
            bodyLengthStates[i].Rotation = bodyLengthBones[i].transform.localRotation;
        }

        LocalRotation_AnimationData[] leftFinStates = new LocalRotation_AnimationData[leftFinBones.Count];
        for (int i = 0; i < leftFinBones.Count; i++)
        {
            leftFinStates[i].Rotation = leftFinBones[i].transform.localRotation;
        }

        LocalRotation_AnimationData[] rightFinStates = new LocalRotation_AnimationData[rightFinBones.Count];
        for (int i = 0; i < rightFinBones.Count; i++)
        {
            rightFinStates[i].Rotation = rightFinBones[i].transform.localRotation;
        }

        LocalRotation_AnimationData[] mouthStates = new LocalRotation_AnimationData[mouthBones.Count];
        for (int i = 0; i < mouthBones.Count; i++)
        {
            mouthStates[i].Rotation = mouthBones[i].transform.localRotation;
        }
        LocalRotation_AnimationData headState = new LocalRotation_AnimationData();
        headState.Rotation = headBone.transform.localRotation;
        

        return (rootState, bodyLengthStates, leftFinStates, rightFinStates, mouthStates, headState);
    }

    void updateMovement(WhaleState newState){

        agx.Vec3 currPos = rootRB.Native.getPosition();

        if (currPos.y >= seaLevel) {
           return;
        }
        
        agx.Vec3 targetPos = newState.Root.Position.ToHandedVec3();
        agx.Vec3 currentVelocity = rootRB.Native.getVelocity();
        
        agx.Vec3 displacement = targetPos - currPos;
        float mass = (float)rootRB.Native.getMassProperties().getMass();

        float positionalStiffness = 500f; 
        
        float velocityDamping = 10f; 

        float inWater = currPos.y < seaLevel ? 1.0f : 0.0f;

        agx.Vec3 desiredVelocity = displacement * positionalStiffness * inWater;

        agx.Vec3 velocityError = desiredVelocity - currentVelocity;

        agx.Vec3 force = velocityError * mass * velocityDamping;

        rootRB.Native.addForce(force);  
    }

    public override void updateWhaleState(WhaleState newState) {

        updateMovement(newState);

        rootRB.Native.setRotation(newState.Root.Rotation.ToHandedQuat());

         //swapYandX(newState.Head.Rotation, out Quaternion headRot);
        headBone.transform.localRotation = newState.Head.Rotation;

        for (int i = 0; i < newState.BodyLength.Count(); i++)
        {   
            if (i < bodyLengthBones.Count-8){
                swapZandY(newState.BodyLength[i].Rotation, out Quaternion newRot);
                bodyLengthBones[i].transform.localRotation = newRot;
            } else {
                bodyLengthBones[i].transform.localRotation = newState.BodyLength[i].Rotation;
            }

        }

        for (int i = 0; i < newState.LeftFin.Count(); i++)
        {
            leftFinBones[i].transform.localRotation = newState.LeftFin[i].Rotation;
        }

        for (int i = 0; i < newState.RightFin.Count(); i++)
        {
            rightFinBones[i].transform.localRotation = newState.RightFin[i].Rotation;
        }

        for (int i = 0; i < newState.Mouth.Count(); i++)
        {
            mouthBones[i].transform.localRotation = newState.Mouth[i].Rotation;
        }
    }
    
    string determinePhase(WhaleState startState){ 
        string output = null;

        float angleFromCenter = Mathf.DeltaAngle(startState.Root.Rotation.eulerAngles.x, 0f);
        // descent
        if (angleFromCenter >5.0f) {
             Debug.Log("ascent");
            output="\"ascent\"";
            
        //ascent
        } else if (angleFromCenter < -4.0f){
           Debug.Log("descent");
            output="\"descent\"";
        // bottom (straight on)
        } else {
            Debug.Log("bottom");
            output="\"bottom\"";
        }
        return output; 
    }
    void swapZandY(Quaternion input, out Quaternion output){
        Vector3 euler = input.eulerAngles;
        float currentY = euler.y;
        float currentZ = euler.z;

        euler.y = currentZ;
        euler.z = currentY;

        output = Quaternion.Euler(euler.x, euler.y, euler.z);
    }
      void swapYandX(Quaternion input, out Quaternion output){
        Vector3 euler = input.eulerAngles;
        float currentX = euler.x;
        float currentY = euler.y;

        euler.y = currentX;
        euler.x = currentY;

        output = Quaternion.Euler(euler.x, euler.y, euler.z);
    }

}
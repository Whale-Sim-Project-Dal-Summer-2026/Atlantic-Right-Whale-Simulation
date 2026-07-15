// abstract verison of the whale model so the rive ronly has to pass in a new state and the whale modle will update to it
using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using AnimationDataStructs;
using System.Linq;
/// <summary>
/// Handles Forward movement based on dead reckoning using speed and direction
/// </summary>
public class WhaleModelUnity : WhaleModelAbstract {

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

    int bodyEndIndex;
    int startTailIndex;
    int tailEndIndex;
    int leftFlukeEndIndex;
    int leftFlukeStartIndex;

    public WhaleModelUnity(WhaleBones whaleBones){
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
        rootState.Position = rootBone.transform.position;
        rootState.Rotation = rootBone.transform.rotation;

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

    public override void updateWhaleState(WhaleState newState){
        //determinePhase(newState);
        rootBone.transform.position = newState.Root.Position;
        rootBone.transform.rotation = newState.Root.Rotation;

        swapYandX(newState.Head.Rotation, out Quaternion headRot);
        headBone.transform.localRotation = headRot;


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
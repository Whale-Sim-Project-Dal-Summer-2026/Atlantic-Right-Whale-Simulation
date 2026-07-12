// abstract verison of the whale model so the rive ronly has to pass in a new state and the whale modle will update to it
using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using AnimationDataStructs;
using System.Linq;
public class WhaleModel {

    //--Bone Counts--
    int mainBodyBoneCount;
    int tailBoneCount;
    int leftFinBoneCount;
    int rightFinBoneCount;
    int mouthBoneCount;
   
    //--Bone Lists--
    GameObject mainBodyBone;
    List<GameObject> tailBones;
    List<GameObject> leftFinBones;
    List<GameObject> rightFinBones;
    List<GameObject> mouthBones; 

    public WhaleModel(WhaleBones whaleBones){
        tailBones = new List<GameObject>();
        leftFinBones = new List<GameObject>();
        rightFinBones = new List<GameObject>();
        mouthBones = new List<GameObject>();

        InitializeBones(whaleBones);

        mainBodyBoneCount = 1;
        tailBoneCount = tailBones.Count;
        leftFinBoneCount = leftFinBones.Count;
        rightFinBoneCount = rightFinBones.Count;
        mouthBoneCount = mouthBones.Count;
        mainBodyBone = whaleBones.mainBodyBone;
    }
    void InitializeBones(WhaleBones blueprintSettings){

        GetAllChildren(blueprintSettings.tailStartBone, tailBones, blueprintSettings.tailStopBone);
        GetAllChildren(blueprintSettings.leftFinStartBone, leftFinBones, blueprintSettings.leftFinStopBone);
        GetAllChildren(blueprintSettings.rightFinStartBone, rightFinBones, blueprintSettings.rightFinStopBone);
        mouthBones.Add(blueprintSettings.mouthTopBone);
        mouthBones.Add(blueprintSettings.mouthBottomBone);

    }
    void GetAllChildren(GameObject parent, List<GameObject> addingList, GameObject stopObject){
        foreach (Transform child in parent.transform){
            // skips colldiers 
            if (child.name == "Colliders"|| child.name == "Collider"){ continue;}
            addingList.Add(child.gameObject);

            if (child.gameObject == stopObject)
            {
                break;
            }
            // Recursively add this child's children
            GetAllChildren(child.gameObject,addingList,stopObject);
        }
   
    }

    public WhaleBlueprint getBlueprint(){
        return new WhaleBlueprint(tailBoneCount, mouthBoneCount, leftFinBoneCount, rightFinBoneCount,mainBodyBoneCount);
    }

    public WhaleState getCurrentState(){
        return new WhaleState(castGameObjectsToWhaleState());
    }

    // cast the current gameobject transforms to a whale state object
    (Global_AnimationData, LocalRotation_AnimationData[], LocalRotation_AnimationData[], LocalRotation_AnimationData[], LocalRotation_AnimationData[]) castGameObjectsToWhaleState(){
        Global_AnimationData mainBodyState = new Global_AnimationData();
        mainBodyState.Position = mainBodyBone.transform.position;
        mainBodyState.Rotation = mainBodyBone.transform.rotation;

        LocalRotation_AnimationData[] tailStates = new LocalRotation_AnimationData[tailBones.Count];
        for (int i = 0; i < tailBones.Count; i++)
        {
            tailStates[i].Rotation = tailBones[i].transform.localRotation;
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

        return (mainBodyState, tailStates, leftFinStates, rightFinStates, mouthStates);
    }

    public void updateWhaleState(WhaleState newState){
        mainBodyBone.transform.position = newState.MainBody.Position;
        mainBodyBone.transform.rotation = newState.MainBody.Rotation;

        for (int i = 0; i < newState.Tail.Count(); i++)
        {
            tailBones[i].transform.localRotation = newState.Tail[i].Rotation;
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

}
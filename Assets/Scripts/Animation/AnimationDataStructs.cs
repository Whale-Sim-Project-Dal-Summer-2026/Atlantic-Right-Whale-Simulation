using Unity.Mathematics;
using UnityEngine;
///Contains all of the data structs for the animation of the whale
/// trying to minimize memory but idk if its actually doing much
namespace AnimationDataStructs
{
/// <summary>
/// Stores the tail/fin animation as 4 halves which are applied as a rotation
/// </summary>
/// 
/// this should save on memory since a transform is apparently kinda hefty
/// 



public struct LocalRotation_AnimationData {
    public half4 Rotation; // smaller than a quaternion
}


/// <summary>
/// Stores the global animation data such as body movements as transform vector and quaternion 
/// </summary>
/// 
/// same here
public struct Global_AnimationData {
    public Vector3 Position; 
    public Quaternion Rotation; 
}

/// <summary>
/// Defines the "Shape" of the whale, ie how many bones in each section
/// </summary>
public class WhaleBlueprint {
    public int TailCount;
    public int MouthCount;
    public int LeftFinCount;
    public int RightFinCount;
    public int MainBodyCount;

    public WhaleBlueprint(int tail, int mouth, int lFin, int rFin, int body) {
        TailCount = tail;
        MouthCount = mouth;
        LeftFinCount = lFin;
        RightFinCount = rFin;
        MainBodyCount = body;
    }
}

/// <summary>
///  Defines a single frame/snapshot of the entires whales state
/// </summary>
public class WhaleState
{
    // should just be a solid state of the whale, built from the whale blueprint 

    // seems to be decently memory effcient???

    public LocalRotation_AnimationData[] Tail;
    public LocalRotation_AnimationData[] Mouth;
    public LocalRotation_AnimationData[] LeftFin;
    public LocalRotation_AnimationData[] RightFin;
    public Global_AnimationData[] MainBody;

    public WhaleState(WhaleBlueprint blueprint) {
        Tail = new LocalRotation_AnimationData[blueprint.TailCount];
        Mouth = new LocalRotation_AnimationData[blueprint.MouthCount];
        LeftFin = new LocalRotation_AnimationData[blueprint.LeftFinCount];
        RightFin = new LocalRotation_AnimationData[blueprint.RightFinCount];
        MainBody = new Global_AnimationData[blueprint.MainBodyCount];
    }


}
}
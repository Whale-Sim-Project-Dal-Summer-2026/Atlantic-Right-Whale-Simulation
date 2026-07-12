using Unity.Mathematics;
using UnityEngine;
///Contains all of the data structs for the animation of the whale
/// trying to minimize memory but idk if its actually doing much
namespace AnimationDataStructs{

/// <summary>
/// Stores the tail/fin animation as 4 halves which are applied as a rotation
/// </summary>
/// 
/// this should save on memory since a transform is apparently kinda hefty
/// 
/// switch back to float since it has difficuly being written to a binary fiel 
/// 
/// OTHER OPTION:
/// Use giant arrays and then just store the index, this would be for literally every frame in the animation for each bone'
/// basically use the amount of tail bones as an frame size 
/// example: 
///     {1,2,3,4,5,6,7,8,43,32,64,7,5,345,7,3,234,8,634,2...}  // list of all rotations to be applied to tail for all animation timesteps
///   then there is frame number which denotes how many bones are in the tail 
///    framewWindow = 4
/// at timestep 1:  {[1,2,3,4],5,6,7,8,43,32,64,7,5,345,7,3,234,8,634,2...} // frame is first 4 roations in list'
/// at timestep 2:  {1,2,3,4,[5,6,7,8],43,32,64,7,5,345,7,3,234,8,634,2...} // frame is second 4 roations in list'
///  this contiunes 
/// 
/// this is better than storing whale state as a class object since it will be less memeroy used
///  also faster to access since its in a giant array
/// 
/// cons:
///     would be hard to manage this many windows for different sections of the whale,
///     since each section has a different amount of bones, so a different frame size
/// 
/// I think this could be switched to later if needed, but for now just use the class object since its easier 



public struct LocalRotation_AnimationData {
    public Quaternion Rotation; 
}


/// <summary>
/// Stores the global animation data such as body movements as transform vector and quaternion 
/// </summary>
/// 
/// same here
public struct Global_AnimationData {

    // used for dead reckoning
    public Vector3 Position; 
    public Quaternion Rotation; 
    
    // used for applying force
    public float Speed;
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
public struct WhaleState
{
    // should just be a solid state of the whale, built from the whale blueprint 

    // seems to be decently memory effcient???

    public LocalRotation_AnimationData[] Tail;
    public LocalRotation_AnimationData[] Mouth;
    public LocalRotation_AnimationData[] LeftFin;
    public LocalRotation_AnimationData[] RightFin;
    public Global_AnimationData MainBody;

    public WhaleState(WhaleBlueprint blueprint) {
        Tail = new LocalRotation_AnimationData[blueprint.TailCount];
        Mouth = new LocalRotation_AnimationData[blueprint.MouthCount];
        LeftFin = new LocalRotation_AnimationData[blueprint.LeftFinCount];
        RightFin = new LocalRotation_AnimationData[blueprint.RightFinCount];
        MainBody = new Global_AnimationData();
    }
    public WhaleState((Global_AnimationData, LocalRotation_AnimationData[], LocalRotation_AnimationData[], LocalRotation_AnimationData[], LocalRotation_AnimationData[]) stateTuple) {
        MainBody = stateTuple.Item1;
        Tail = stateTuple.Item2;
        LeftFin = stateTuple.Item3;
        RightFin = stateTuple.Item4;
        Mouth = stateTuple.Item5;
    }


}
}
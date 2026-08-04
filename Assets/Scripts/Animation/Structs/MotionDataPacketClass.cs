using UnityEngine;

namespace MotionDataPacketClass{
/// <summary>
/// Stores one single timestep of the whales motion data 
/// </summary>
[System.Serializable]
public class MotionDataPacket
{
    /// <summary>
    /// the timestep which <see langword="this"/ motion is happening 
    /// </summary>
    public float timestep;
    /// <summary>
    /// Current depth in meters from the ocean surface
    /// </summary>
    public float depth;

    /// <summary>
    /// Current heading <see langword="as"/> radians <see langword="with"/> magnetic north <see langword="as"/> ref   
    /// </summary>
    public float head;

    /// <summary>
    /// Overall pitch angle referenced <see langword="throw"/> <see langword="float"/> body position <see langword="in"/> radians   
    /// </summary>
    public float pitch;

    /// <summary>
    /// Roll orientation referenced <see langword="throw"/><see langword="float"/> body position <see langword="in"/> radians 
    /// </summary>
    public float roll;
    /// <summary>
    /// forward movement speed (m/s)
    /// </summary>
    public float speed; 

    /// <summary>
    /// Fluke signal <see langword="in"/> radians  
    /// </summary>
    public float fluking_signal;

    /// <summary>
    /// Body signal <see langword="in"/> radians  
    /// </summary>
    public float body_signal;
    
    /// <summary>
    /// <see langword="bool"/> <see langword="for"/> flagging <see langword="if"/> mouth <see langword="is"/> open <see langword="or"/>closed       
    /// </summary>
    public int MouthOpen;


}
} 
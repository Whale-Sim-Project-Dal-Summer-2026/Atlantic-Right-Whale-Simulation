using System;
using AnimationDataStructs;
using Unity.Mathematics;
using UnityEngine;
using MotionDataPacketClass;
public abstract class MainBodySolverAbstract
{
    public abstract Global_AnimationData solveMainBody(MotionDataPacket currentPacket, Global_AnimationData previousState);

    public abstract void resetSolver(WhaleState startState); 
    
}


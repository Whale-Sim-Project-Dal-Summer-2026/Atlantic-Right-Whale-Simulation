using System;
using AnimationDataStructs;
using Mono.Cecil.Cil;
using Unity.Mathematics;
using UnityEngine;

namespace DataSources{
public abstract class DataSource
{
    //public DataLoader dataLoader;

    //int currentTimeStep = 0;

    public abstract void LoadSource(AnimationSettings animationSettings, WhaleState startState, WhaleBlueprint blueprint);
    public abstract WhaleState getNextWhaleState();
    public abstract void loadWhaleStateAt(int timestep);
    public abstract void GetTotalTimesteps(out int totalTimesteps);
    
}


// so somehow need to have a datasource feed the driver, maybe both could be passed in the whale structure??
// maybe they are passed in the current whale state, and return the next whale state? as well as delta time both fixed and regular

// BOTH WILL RETURN WHALE STATES which hold lists of different bones in the whale
// okay so they will have their own logic for doing this? maybe two motion data csv ones? one for the classic method of just moving using the forward vec 
// and another with full body motion??
// 

//FIRST DRAFT OF DATASOURCES JUST FOR MY OWN MEMEORY
//NOTHING IS IMPLEMENTED YET

public class MotionDataCSV : DataSource
{
    // each one should have a constructor which builds itself using the blueprint provided
    // maybe the blueprint can be passed in from a settings scriptable asset???
    
    public override void LoadSource(AnimationSettings animationSettings, WhaleState startState, WhaleBlueprint blueprint)
    {
        throw new System.NotImplementedException();
    }


    public override WhaleState getNextWhaleState()
    {
        throw new System.NotImplementedException();
    }

    public override void loadWhaleStateAt(int timestep)
    {
        throw new System.NotImplementedException();
    }
    public override void GetTotalTimesteps(out int totalTimesteps)
    {
        throw new System.NotImplementedException();
    }
}

public class RandomWalk : DataSource
{
    public override void LoadSource(AnimationSettings animationSettings, WhaleState startState, WhaleBlueprint blueprint)
    {
        throw new System.NotImplementedException();
    }


    public override WhaleState getNextWhaleState()
    {
        throw new System.NotImplementedException();
    }

    
    public override void loadWhaleStateAt(int timestep)
    {
        throw new System.NotImplementedException();
    }
     public override void GetTotalTimesteps(out int totalTimesteps)
    {
        throw new System.NotImplementedException();
    }
}

}
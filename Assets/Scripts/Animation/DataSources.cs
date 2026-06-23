using System;
using AnimationDataStructs;
using Unity.Mathematics;
using UnityEngine;

namespace DataSources{
public abstract class DataSource
{
    public abstract void LoadSource(TextAsset file, WhaleState startState, WhaleBlueprint blueprint);
    public abstract WhaleState getNextWhaleState(int currentTimeStep);
    public abstract WhaleState getWhaleStateAt(int timestep);
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
    public override void LoadSource(TextAsset file, WhaleState startState, WhaleBlueprint blueprint)
    {
        throw new System.NotImplementedException();
    }


    public override WhaleState getNextWhaleState(int currentTimeStep)
    {
        throw new System.NotImplementedException();
    }

    public override WhaleState getWhaleStateAt(int timestep)
    {
        throw new System.NotImplementedException();
    }
}

public class RandomWalk : DataSource
{
    public override void LoadSource(TextAsset file, WhaleState startState, WhaleBlueprint blueprint)
    {
        throw new System.NotImplementedException();
    }


    public override WhaleState getNextWhaleState(int currentTimeStep)
    {
        throw new System.NotImplementedException();
    }

    public override WhaleState getWhaleStateAt(int timestep)
    {
        throw new System.NotImplementedException();
    }
}

}
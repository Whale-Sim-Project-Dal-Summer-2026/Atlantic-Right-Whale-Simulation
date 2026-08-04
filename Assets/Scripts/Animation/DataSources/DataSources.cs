using AnimationDataStructs;

namespace Animation.DataSources{

/// <summary>
/// Abstract Class from which all datasources are based on
/// </summary>
public abstract class DataSource{
    public abstract void LoadSource(AnimationSettings animationSettings, WhaleState startState, WhaleBlueprint blueprint);
    public abstract WhaleState getNextWhaleState();
    public abstract void loadWhaleStateAt(int timestep);
    public abstract int GetTotalTimesteps();
    
}

/// current work in prog
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
     public override int GetTotalTimesteps()
    {
        throw new System.NotImplementedException();
    }
}

}
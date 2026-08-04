using System.Collections;
using System.Collections.Generic;
using AnimationDataStructs;

public abstract class WhaleModelAbstract
{
    public abstract void updateWhaleState(WhaleState newState);
    public abstract WhaleBlueprint getBlueprint();
    public abstract WhaleState getCurrentState();

}
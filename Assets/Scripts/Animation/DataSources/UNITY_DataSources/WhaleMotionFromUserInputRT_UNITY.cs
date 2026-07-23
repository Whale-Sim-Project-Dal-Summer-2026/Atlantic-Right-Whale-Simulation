using AnimationDataStructs;
using MotionDataPacketClass;
using UnityEngine;
using Animation.DataSources;
using System.Collections.Generic;
using FlukeWaveAmplitudeLookUpClass;

/// <summary>
/// Real Time User input turned into whale movement, No Saving or Streaming, WhaleState Processed in Real Time.
/// Uses Unity Physics
/// </summary>
/// 
/// 
/// 
/// this may not be useful to have but i just kinda want things to be even since who knows what future development may happen
public class WhaleMotionFromUserInputRT_UNITY
 : DataSource
{
    //not real value since we are getting real time user input
    int totalTimesteps = -1; 
    // same thing here
    int timestep = -1;
    float fixedTimeStep = 0.004f;

    //might be cool to eventually be able to save user input to a file and then play it back as a motion data csv source

    // passes user input in 
    [SerializeField] UserInputManager userInputManager; 

    WhaleState currentWhaleState;
    WhaleState startState; // initial state of the whale, will be used to reset the whale to its initial position and rotation
    WhaleBlueprint blueprint;
    CSVLoader cSVLoader;
    FlukeSolver flukeSolver;
    FlukeWaveAmplitudeLookUp lookUp;

    // Solvers
    int tailStartIndex = 0;
    MouthSolver mouthSolver;
    FinSolver finSolver;

    MainBodySolverAbstract mainBodySolver;

    public override void LoadSource(AnimationSettings animationSettings, WhaleState startState, WhaleBlueprint blueprint)
    {
        // needs to be able to return:
            // pitch, roll, yaw, speed, mouth open/close

        cSVLoader = new CSVLoader();

        userInputManager = GameObject.FindAnyObjectByType<UserInputManager>();

        loadFlukeWaveAmplitudeLookUpCSV(animationSettings);

        this.startState = startState;
        this.currentWhaleState = startState;
        this.blueprint = blueprint;

        mouthSolver = new MouthSolver(startState.Mouth);
        flukeSolver = new FlukeSolver(blueprint.BodyLengthCount, fixedTimeStep, lookUp, tailStartIndex);
        mainBodySolver = new AGXUserMainBodySolver(fixedTimeStep, startState, userInputManager.rb);
        finSolver = new FinSolver(startState.LeftFin.Length, fixedTimeStep);

    }

    void loadFlukeWaveAmplitudeLookUpCSV(AnimationSettings animationSettings){
        var loaded_CsvData = cSVLoader.loadCSV(animationSettings.FlukeAmpLookUp_csv,animationSettings.FlukeAmp_ContainsHeaders);
        string[][] csvData = loaded_CsvData.data;
        Dictionary<string,int> columnIndices= loaded_CsvData.columnIndices;

        lookUp = new FlukeWaveAmplitudeLookUp(csvData,columnIndices);

    }

    public override WhaleState getNextWhaleState()
    {



        WhaleState newState = new WhaleState(blueprint);

        // Get user input for movement
        MotionDataPacket motionDataPacket = createMotionDataPacketFromUserInput(userInputManager);

        // Solve for the new state based on user input
        newState.Root = mainBodySolver.solveMainBody(motionDataPacket, currentWhaleState.Root);

        if (mainBodySolver is AGXUserMainBodySolver agxSolver)
        {
            newState.BodyLength = agxSolver.getBodyState();
            newState.Head = agxSolver.getHeadState();
        }
        else
        {
            Debug.LogError("mainBodySolver is not of type AGXUserMainBodySolver");
        }
        newState.Mouth = mouthSolver.solveMouth(motionDataPacket.MouthOpen == 1 ? true : false , currentWhaleState.Mouth);
        this.currentWhaleState = newState;

        newState.BodyLength= flukeSolver.solveFuke(motionDataPacket, newState);
        newState.LeftFin = finSolver.solveFin(motionDataPacket, true, currentWhaleState.LeftFin);
        newState.RightFin = finSolver.solveFin(motionDataPacket,  false, currentWhaleState.RightFin);

       
        return newState;
    }

    MotionDataPacket createMotionDataPacketFromUserInput(UserInputManager userInputManager)
    {
        MotionDataPacket motionDataPacket = new MotionDataPacket();
        motionDataPacket.head = userInputManager.yaw; //yaw
        motionDataPacket.pitch = userInputManager.pitch;
        motionDataPacket.roll = userInputManager.roll;
        motionDataPacket.speed = userInputManager.speed;
        motionDataPacket.MouthOpen = userInputManager.mouthOpen ? 1 : 0;

        return motionDataPacket;
    }
    public override void loadWhaleStateAt(int timestep)
    {
       if (timestep != 0)
        {
            Debug.LogWarning("WhaleMotionFromUserInputRT does not support loading states at specific timesteps. Resetting to initial state.");
        }
        this.currentWhaleState = startState;
        userInputManager.rb.Native.setPosition(new agx.Vec3(-500, 0, 500));
    }

    public override int GetTotalTimesteps()
    {
        return this.totalTimesteps;
    }
}

using Animation.DataSources; 
using Unity.Mathematics;
using UnityEngine;
using AnimationDataStructs;
using AnimationDataStorageManager;
using static WhaleAnimationStreamer;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using agx;
using Unity.Collections;
using System;
using agxDriveTrain;



//-- ROADMAP NOTES--


// ARCH
//1. animation is preprocessed from data source into a series of whale states
//2. then the animation or list of whale states are saved into a binary file
//3. then the binary file is loaded using a background thread in large chunks and fed into the driver
//4. the driver then applies the current whale state to the whale model in the scene
//5. once the chunk is nearly finished, the next chunk is loaded in the background and fed into the driver
//6. repeat until the end of the animation is reached



// to make the timestep scrubbing work properly, the driver needs to be able to get the whale state at a specific timestep, 
// and also get the next whale state from the current timestep. the chunk loading should be able to do those tasks.

// FIXED TIMESTEP OF 0.02 

// EVERY 5 FIXED UPDATES A NEW FRAME OR SNAPCHOT OF THE WHALE IS SWITCHED TO (lerp between the two states for the 5 fixed updates).
// count the updates and every 5 switch frames 

// chunkloading for a time step will work by dividing the current time by 0.1 and getting the floor to then get the index of the frame in the file
// then dividne by chunk size to get which chunk its in
//then modulo to get the offset inside the chunk and then load the chunk and get the frame at the offset. 
// then contiune the animation
// 

// all logic will be the same so there can be an AGX based dirver and a pure unity based driver

//SAME THING WITH THE MULTIPLE DATA SOURCES!!!




//TO DO:
// MAKE BETTER FILE SYSTEM FOR SAVING THINGS - no overwriting
// set up agx forces saving
// complete whale destruvtion and re-creation for better consistancy 
// refactor the user manager stuff 

public enum DataSourceType
{
    ClassicMotionDataCSV,
    WhaleMotionFromCSV,
    WhaleMotionFromUserInputRT,
    WhaleMotionFromCSV_AGX,
    MotionDataCSV,
    RandomWalk
}
public enum WhaleModelType
{
    Unity,
    AGX
}


public class WhaleDriver : MonoBehaviour
{
   
    CameraControls controls;
    public TextAsset dataSourceFile;

    public bool isPaused = false;
    public int jumpToTimestep = -1;

    [Header("Data Source Settings")]
    public DataSourceType dataSourceType;
    DataSource dataSource; 

    [Header("Whale Model Settings")]
    public WhaleModelType whaleModelType;
    [SerializeField] Animator animator;

    public int currentTimestep = 0;
    public int totalTimesteps = 0;

    [Header("Animation Settings")]
    public AnimationSettings animationSettings;

    public int CSV_ResetTimeStep;
    public int PauseTime; 
    private int StartingPauseTime = 0;

// we care about seaLevel only here
    public ProcessingSettings processingSettings;
    
    public ResetManager ResetManager;
   

    WhaleModelAbstract whaleModel;
    WhaleBones whaleBones;
    

    //UI EXTRAS


    public float whaleYaw;
    public float whalePitch;
    public float whaleRoll;

    public float whaleSpeed; 
    
    void Start(){


        StartingPauseTime = PauseTime;  

        // set data source based on the enum value
        dataSource = dataSourceType switch{
            DataSourceType.ClassicMotionDataCSV => new ClassicMotionDataCSV(),
            DataSourceType.WhaleMotionFromCSV => new WhaleMotionFromCSV_UNITY(),
            DataSourceType.RandomWalk => new RandomWalk(),
            DataSourceType.WhaleMotionFromCSV_AGX => new WhaleMotionFromCSV_AGX(),
            DataSourceType.WhaleMotionFromUserInputRT => new WhaleMotionFromUserInputRT_AGX(),
            _ => throw new System.ArgumentOutOfRangeException()
        };

        // Retrieve the WhaleBones from the GameObject on the Whale 
        WhaleBones whaleBones =  this.gameObject.GetComponent<WhaleBones>();

   

        //Set up the model which actually applies the state to the bones in the scene
        whaleModel = whaleModelType switch{
            WhaleModelType.Unity => new WhaleModelUnity(whaleBones),
            WhaleModelType.AGX => new WhaleModelAGX(whaleBones),
            _ => throw new System.ArgumentOutOfRangeException()
        };
        
        // get the start state 
        WhaleState startState = whaleModel.getCurrentState();

        // get the blueprint which is just the counts of the bones in the whale
        WhaleBlueprint blueprint = whaleModel.getBlueprint();

        // set the start state position and rotation to the current transform of the whale
        startState.Root.Position = transform.position;
        startState.Root.Rotation = transform.rotation;

        // load the data source
        dataSource.LoadSource(animationSettings, startState, blueprint);
             int totalTimeStes = dataSource.GetTotalTimesteps();
        this.totalTimesteps = totalTimeStes;

        ResetManager.OnReset += () => {
            jumpToTimestep = 0;
            PauseTime = StartingPauseTime;
        };
    }


    void Awake(){
        controls = new CameraControls();
    }

    void OnEnable(){
        controls.Enable();
    }

    void OnDisable(){
        controls.Disable();
    }

    void updateWhaleState() {
        WhaleState newState = dataSource.getNextWhaleState();
        updateUIParameters(newState);
        whaleModel.updateWhaleState(newState);
    }

    void Update(){
        if (PauseTime>0) {
            isPaused = true; 
        }
        if (isPaused) return;
        if (jumpToTimestep >= 0){
            dataSource.loadWhaleStateAt(jumpToTimestep);
            updateWhaleState();
        
            currentTimestep = jumpToTimestep;
            jumpToTimestep = -1; 
        }
    }

    void updateUIParameters(WhaleState newState) {
        whaleSpeed = newState.Root.Speed;
        
       

        float pitch = newState.Root.Rotation.eulerAngles.x;

        if (pitch > 180f) pitch -= 360f;
        //Debug.Log("Pitch: "+ pitch);
        //float pitch360Percent  = Mathf.InverseLerp(0,360, pitch);
        //float newPitch = Mathf.Lerp(-90,90,pitch360Percent);

        //if (newPitch < 0) newPitch= -(newPitch + 90);
        //else newPitch= -(newPitch-90);

        whalePitch = -pitch;

        whaleYaw = newState.Root.Rotation.eulerAngles.y;
        
        float roll = newState.Root.Rotation.eulerAngles.z;

        if (roll > 180f) roll -= 360f;


        whaleRoll = -roll; 
    }
  void FixedUpdate(){
    PauseTime -= 1;
    if (PauseTime <= 0){
        PauseTime = 0;
        isPaused = false;
    }

    if (isPaused) return;
    // if button is pushed then pass the number to load a certain timestep
    updateWhaleState();

 
    currentTimestep ++;
   
    // looping parameter
    if (currentTimestep >= CSV_ResetTimeStep){
        currentTimestep = 0;
        ResetManager.TriggerReset();
    }
    
    if (controls.Player.Reset.triggered){
       jumpToTimestep = 0;
       ResetManager.TriggerReset();
       //animator.Play("R Whale Armature|Whale Swimming",0,0);  
    }




}


}
using UnityEngine;
using System.Collections.Generic;
using AnimationDataStructs;
using MotionDataPacketClass;
using FlukeWaveAmplitudeLookUpClass;
using System;
public class FlukeSolver {
    

    int tailCount;
    float timer = 0f;
    float currentPhase = 0f; 
        float fixedTimeStep; 

    float frequency = 0.1f;
    float amplitude = 0.1f;   
    private float phaseShiftPerUnit = 1.25f;
    private float wave_offset = 0 ;
    private float boneLength = 0.1f;

    private int tailStartIndex;

    FlukeWaveAmplitudeLookUp lookUp;

    public List<float> boneMaxAngles = new List<float>
{
    0.1f, 
    0.1f, 
    0.1f, 
    0.1f, 
    0.15f, 
    0.15f, 
    0.15f, 
    0.15f, 
    0.2f, 
    0.2f, 
    0.2f, 
    0.2f, 
    0.3f, 
    0.3f, 
    0.8f,
    0.15f, 
    0.15f, 
    0.15f, 
    0.15f,
    0.15f, 
    0.15f, 
    0.15f,
    0.15f, 
    0.15f, 
    0.15f,
    0.15f, 
    0.15f, 
    0.15f,
    0.15f, 
    0.15f 
};

    public FlukeSolver(int tailCountIn, float fixedTimeStepIn, FlukeWaveAmplitudeLookUp lookUpIn, int tailStartIndexIn){
        tailCount = tailCountIn;
        fixedTimeStep = fixedTimeStepIn;
        lookUp = lookUpIn;
        tailStartIndex = tailStartIndexIn;
    }

    public LocalRotation_AnimationData[] solveFuke(MotionDataPacket packet, WhaleState startState){
        string phase = determinePhase(startState);
        setAmplitudeAndFrequencyFromLookUp(packet,phase);
        return calculateFlukeStates(packet, startState);
    }

    LocalRotation_AnimationData[] calculateFlukeStates(MotionDataPacket packet, WhaleState startState){

        LocalRotation_AnimationData[] flukeState = new LocalRotation_AnimationData[tailCount];

        // GOTTA UPDATE TO INBETWEEN STEPS, MAYE FIXEDUP changes the target amp litude and this happens in update using deltatime ??? 
            
        currentPhase += (2f * Mathf.PI * frequency) * fixedTimeStep;

        currentPhase %= (Mathf.PI * 2f);
        // Tan et al. (2011): q_i(t) = A_i * sin(2π*t/T_i + φ_i) + C_i
        // equation for moving each tail bone based on the wave parameters and the position of the bone along the tail
        float T = 1f / (float)frequency; 
        float cumulativeDistance = 0f;  
        for (int i = tailStartIndex; i < tailCount; i++){  
            // adjusts amplitude to be within range of motion for bone (could try clamping the final angle too??) LOOKS OKAY JUST NEED TO TUNE
            float A_i   = (float)amplitude* boneMaxAngles[i];        
            //   the shift of amount of the wave based on the distance (negative since going backwards) LOWER THIS!!!!!
            float phi_i = -(cumulativeDistance * phaseShiftPerUnit);   
            // static wave offset (not sure if this can be tuned withouxt breaking anything so keeping it 0)
            float C_i   = wave_offset;                                          
            // use tan et al swimming gait formula
            float currentAngle = A_i * Mathf.Sin(currentPhase + phi_i) + C_i;    
            // apply to bone (local roation makes it forward kinematic builds on each other)
            flukeState[i].Rotation = Quaternion.Euler(currentAngle * Mathf.Rad2Deg, 0f, 0f) * startState.BodyLength[i+tailStartIndex].Rotation;

            cumulativeDistance += boneLength;
        }

    
    
        return flukeState; 

    }

    
    void setAmplitudeAndFrequencyFromLookUp(MotionDataPacket currentPacket, string phase){

        float speed = (float)Math.Round(currentPacket.speed,1);

        
        double[] ampAndFreq = lookUp.lookUp(phase, speed, currentPacket.MouthOpen == 1 ? true : false );

        double found_amplitude = ampAndFreq[0];
        double found_frequency = ampAndFreq[1];

        changeLookUpInstance((float)found_amplitude, (float)found_frequency);
    }
    
     // used to swap between the look up instances
    public void changeLookUpInstance(float targetAmp, float targetFreq){
        amplitude  = Mathf.Lerp((float)amplitude,  targetAmp,  fixedTimeStep);
        frequency  = Mathf.Lerp((float)frequency,  targetFreq, fixedTimeStep);
    }

    string determinePhase(WhaleState startState){ 
        string output = null;

        float angleFromCenter = Mathf.DeltaAngle(startState.Root.Rotation.eulerAngles.x, 0f);
        // descent
        if (angleFromCenter >5.0f) {
             //Debug.Log("ascent");
            output="\"ascent\"";
            
        //ascent
        } else if (angleFromCenter < -4.0f){
           //Debug.Log("descent");
            output="\"descent\"";
        // bottom (straight on)
        } else {
            //Debug.Log("bottom");
            output="\"bottom\"";
        }
        return output; 
    }


}
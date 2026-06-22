using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using FlukeWaveAmplitudeLookUpClass;
using System.Linq;
using std;

namespace FlukeWaveAmplitudeLookUpClass
{
/// <summary>
/// Stores the lookup table of fluke amplitude
/// </summary>

public class FlukeWaveAmplitudeLookUp{


    // this should contain the list of the all amplitudes for lookup 

    //smaller dict of all things 
    
    //  gives back the freq and amplitude for the current phase

    private Dictionary<string,FlukeWaveAmplitudeInstance> lookUpTable; 


    // builds the lookup table from csv data
    public FlukeWaveAmplitudeLookUp(string[][] csvData, Dictionary<string,int> cols) {
        lookUpTable = new Dictionary<string, FlukeWaveAmplitudeInstance>();

        FlukeWaveAmplitudeInstance[] instances = buildInstances(csvData, cols);
        foreach (FlukeWaveAmplitudeInstance instance in instances) {
                string name = hashName(instance.phase,instance.mean_speed,instance.mouthOpen);
                lookUpTable.Add(name,instance);
        }
    }

    private FlukeWaveAmplitudeInstance[] buildInstances(string[][] csvData, Dictionary<string,int> cols) {

        int rowCount = csvData.Length;

        FlukeWaveAmplitudeInstance[] instances = new FlukeWaveAmplitudeInstance[rowCount];

        for (int i = 0;i<rowCount; i++){

                FlukeWaveAmplitudeInstance instance = new FlukeWaveAmplitudeInstance {
                    phase = csvData[i][cols["phase"]],
                    mean_speed = float.Parse(csvData[i][cols["mean_speed"]]),
                    mouthOpen = csvData[i][cols["MouthOpen"]].Contains("1"),
                    amplitude = float.Parse(csvData[i][cols["pred_amplitude"]]),
                    frequency = float.Parse(csvData[i][cols["pred_frequency"]])
                };
               
                instances[i] = instance;
        }        
        return instances;
    }

    private string hashName(string phase, float speed, bool mouthOpen){
        // hash is combo phase speed mouthOpen
        string output = phase + "_" + speed + "_" + mouthOpen;
        return output;        
    }




    public double[] lookUp(string phase, float speed, bool mouthOpen) {
        double[] output= new double[2];

        string hashedName = hashName(phase,speed,mouthOpen);

        if (speed == 0f) {
                output[0]=0d;
                output[1]=0d;
                return output;
            }

        FlukeWaveAmplitudeInstance foundInstance = lookUpTable[hashedName];

        output[0] = foundInstance.amplitude;
        output[1] = foundInstance.frequency;

        return output;

    }
    public int Count() {
        return lookUpTable.Count;


    }
}}
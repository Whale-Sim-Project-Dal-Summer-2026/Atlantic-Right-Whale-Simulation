using System.Collections.Generic;
using UnityEngine;

namespace FlukeWaveAmplitudeLookUpClass{

/// <summary>
/// Stores the lookup table of fluke amplitude - derived from Jay's Model
/// </summary>

public class FlukeWaveAmplitudeLookUp{

    private Dictionary<string,FlukeWaveAmplitudeInstance> lookUpTable;

    float minSpeed = .5f;
    float maxSpeed = 5f;


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

        float clampedSpeed = Mathf.Clamp(speed,minSpeed,maxSpeed);

        string hashedName = hashName(phase,clampedSpeed,mouthOpen);

        FlukeWaveAmplitudeInstance foundInstance = lookUpTable[hashedName];

        output[0] = foundInstance.amplitude;
        output[1] = foundInstance.frequency;

        return output;

    }
    public int Count() {
        return lookUpTable.Count;
    }
}}
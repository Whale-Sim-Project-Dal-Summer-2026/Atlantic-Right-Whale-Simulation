using UnityEngine;

public enum DataArea
{
    GSL,
    BoF
}



[CreateAssetMenu(fileName = "ProcessingSettings", menuName = "Scriptable Objects/ProcessingSettings")]
public class ProcessingSettings : ScriptableObject
{
    [Header("Processing Area (Hover for Key Meanings)")]
    [Tooltip("GSL : Gulf Of Saint Lawrence\nBoF : Bay Of Fundy")]
    public DataArea DataArea;
    [Header("Sea Level")]
    [Tooltip("Normally you keep this on 0")]
    public int SeaLevel;
    [Header("Max Depth, NOTE must be negative!!!!")]
    [Tooltip("Clips any values past the max depth to max depth as a fall back")]
    public int MaxDepth;

    [Header("How many files to process? -1 : run all")]
    public int numToRun = 1;

    [Header("Noise Settings")]

    [Tooltip("Noise Strength, increasing allows for more details/less regularity but is less 'accurate' to the real data")]
    public float noiseAmplitude;

    public float noiseFrequency;

    public float chunkSize;
    

    public string AreaToFilePath(){
        return DataArea == DataArea.GSL ? "GSL" : "BoF";
    }
}

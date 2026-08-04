using System.Collections.Generic;
using System.Linq;
using UnityEngine;


struct ExperimentResult{
    public float mean;
    public float median;
}


public class PerformanceTesting : MonoBehaviour{

    [SerializeField] List<int> ropeCounts;
    [SerializeField] int numRepeatPerScneario;
    [SerializeField] int msPerRepeat;

    [SerializeField] GameObject wire;
    [SerializeField] GameObject cable;


    [SerializeField] RopePositioning ropePositioning;

    List<GameObject> gameObjectsToTest;

    bool logTime;

    int numFrames;
    float totalTime;





    void runAllTests(){

        int numRopesToTest = ropeCounts.Count;
        int gameObjectsCount = gameObjectsToTest.Count;

        int totalNumTests = numRopesToTest * numRepeatPerScneario * gameObjectsCount;

        List<ExperimentResult> results = new List<ExperimentResult>(numRopesToTest * gameObjectsCount);

        List<float> means = new List<float>();

        foreach(GameObject GOtoTest in gameObjectsToTest){
            foreach(int count in ropeCounts){
                float mean = runExperiment(count, GOtoTest);
                means.Add(mean);
            }
            ExperimentResult experimentResult = new ExperimentResult();
            
            float meanOfRuns = means.Average();
            float medianOfRunes = GetMedian(means);

            experimentResult.mean = meanOfRuns;
            experimentResult.median = medianOfRunes;

        }
    }

    public float GetMedian(List<float> numbers) {
    if (numbers == null || numbers.Count == 0)
        {
            return -1.0f;
        }

    List<float> sortedNumbers = numbers.OrderBy(floatValue => floatValue).ToList();
    int count = sortedNumbers.Count;
    int middleIndex = count / 2;

    if (count % 2 == 0) {
        return (sortedNumbers[middleIndex - 1] + sortedNumbers[middleIndex]) / 2.0f;
    }

    return sortedNumbers[middleIndex];
}




    void Start(){

        gameObjectsToTest = new List<GameObject>();
        
        gameObjectsToTest.Add(wire);
        gameObjectsToTest.Add(cable);

        ropeCounts = new List<int>();
    }


// assumes division by 2
// returns mean
    float runExperiment(int numRopes, GameObject prefab){
        int numxRopes = numRopes / 2;

        int numYRopes = 2;

        Vector2Int ropeSpawnDims = new Vector2Int(numxRopes, numYRopes);
        ropePositioning.ClearAndCreate(ropeSpawnDims, prefab);

        while(totalTime <= msPerRepeat){
            numFrames++;
            totalTime += Time.deltaTime;
        }

        float mean = totalTime / numFrames;
        return mean;
    }

}



using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class BoulderSpawner : MonoBehaviour
{
    
    [SerializeField] GameObject boulderPrefab;
    [SerializeField] GameObject boulderParent;
    [SerializeField] GameObject meshParent;

    [Header("Number Of Clumps")]
    [SerializeField] int minNumClumps = 0;

    [SerializeField] int maxNumClumps = 20;


    [Header("Number Of Boulders Per Clump")]

    [SerializeField] int minNumBouldersPerClump = 5;
    [SerializeField] int maxNumBouldersPerClump = 15;

    [Header("Boulder Clump Deviation")]
    [SerializeField] float clumpBoulderDeviation = 90;


    [Header("Boulder Scale")]

    [SerializeField]float minScale = 10f;
    [SerializeField]float maxScale = 30f;

    [SerializeField]float scaleDeviation = 3f;

    [SerializeField] ProcessingSettings processingSettings;

    List<GameObject> activeBoulders;
    FileUtilities fileUtil;
    [SerializeField] bool shouldSpawnBoulders;

    void Start(){
        activeBoulders = new List<GameObject>();
        fileUtil = new FileUtilities();
        shouldSpawnBoulders = true;
    }

    void Update(){
        if (!shouldSpawnBoulders) return;

        clearBoulders();
        ReadBackscatterAndSpawnBoulders();

        shouldSpawnBoulders = false;
    }


    public void ReadBackscatterAndSpawnBoulders() {
        string area = processingSettings.AreaToFilePath();
        string bsDir = Path.Combine(Application.dataPath, "Data", "Processed", "Backscatter", area);

        if (!Directory.Exists(bsDir)) {
            Debug.LogError("The backscatter directory is missing: " + bsDir);
            return;
        }

        MeshFilter[] meshFilters;
        if (meshParent != null) {
            meshFilters = meshParent.GetComponentsInChildren<MeshFilter>(true);
        } else {
            meshFilters = GetComponentsInChildren<MeshFilter>(true);
        }

        int numFilters = meshFilters.Length;
        if (numFilters == 0) {
            Debug.LogWarning("No terrain mesh filters found to apply boulders to.");
            return;
        }

        string[] binSearchPattern = { "*.bytes" };
        string[] bins = binSearchPattern.SelectMany(pattern => Directory.EnumerateFiles(bsDir, pattern)).ToArray();
        int numFiles = bins.Length;

        for (int i = 0; i < numFiles; i++) {
            if (i >= numFilters) {
                break;
            }

            string binFile = bins[i];
            GeoTiffData chunkData = fileUtil.binToTiffData(binFile);

            if (chunkData == null || chunkData.Data == null) {
                continue;
            }

            if (chunkData.Data.Count(x => x > 1.0f) == chunkData.Data.Count()) {
                Debug.LogWarning("Renderer: this entire grabbed chunk is invalid!");
                continue;
            }

            List<float> vals = chunkData.Data;
            if (vals.Count == 0) {
                continue;
            }

            float chunkBSAverage = vals.Average();

            MeshFilter currentFilter = meshFilters[i];
            spawnBoulders(chunkBSAverage, currentFilter);
        }
    }

    public void spawnBoulders(float chunkBSAverage, MeshFilter meshFilter) {
        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;

        int numClumps = Mathf.RoundToInt(Mathf.Lerp(minNumClumps, maxNumClumps, chunkBSAverage));

        int width = Mathf.RoundToInt(Mathf.Sqrt(vertices.Length));
        int height = width;

        for (int i = 0; i < numClumps; i++) {
            

            float randNumBouldersT = Random.Range(0f, 1f);

            int numBouldersToSpawn = Mathf.RoundToInt(Mathf.Lerp(minNumBouldersPerClump, maxNumBouldersPerClump, randNumBouldersT));

            float randX = Random.Range(0f, 1f);
            float randZ = Random.Range(0f, 1f);


            int clumpX = Mathf.RoundToInt(randX * (width - 1));
            int clumpY = Mathf.RoundToInt(randZ * (height - 1));

            for(int j = 0; j < numBouldersToSpawn; j++){    

                float randDeviationXT = Random.Range(0f, 1f);
                float randDeviationYT = Random.Range(0f, 1f);

                int deviationX = Mathf.RoundToInt(Mathf.Lerp(-clumpBoulderDeviation / 2,clumpBoulderDeviation / 2, randDeviationXT));
                int deviationY = Mathf.RoundToInt(Mathf.Lerp(-clumpBoulderDeviation / 2,clumpBoulderDeviation / 2, randDeviationYT));

                int chosenVertexX = Mathf.Clamp(clumpX + deviationX, 0, vertices.Length - 1);
                int chosenVertexY = Mathf.Clamp(clumpY + deviationY, 0, vertices.Length - 1);


                int index = (chosenVertexY * width) + chosenVertexX;
                
                if (index < 0 || index >= vertices.Length) {
                    continue;
                }
                
                Vector3 localPos = vertices[index];

                Vector3 worldPos = meshFilter.transform.TransformPoint(localPos);

                float randScale = Random.Range(0f, 1f);

                float chosenScale = Mathf.Lerp(minScale, maxScale, randScale);

                Vector3 randScaleDeviationDir = Random.onUnitSphere;

                Vector3 scaleVec = (randScaleDeviationDir * scaleDeviation) + new Vector3(chosenScale, chosenScale, chosenScale);

                GameObject obj = Instantiate(boulderPrefab, worldPos, Quaternion.identity, boulderParent.transform);
                obj.transform.localScale = scaleVec;

                activeBoulders.Add(obj);
            }
        }
    }


    public void clearBoulders(){
        // negative index for efficient array removal
        for(int i = activeBoulders.Count - 1; i >= 0; i--){
            Destroy(activeBoulders[i]);
            activeBoulders.RemoveAt(i);
        }   
    }


    private DepthDataRecord readTiff(string filePath) {
        string cleanPath = Path.GetFullPath(filePath);

         DepthDataRecord depthDataRecord = null;
        if (string.IsNullOrEmpty(cleanPath)) {
            return depthDataRecord;
        }
        if (!File.Exists(cleanPath))
        {
            Debug.LogWarning("Bro where is the filE!!!!?????");
        }
        depthDataRecord = fileUtil.binToDepthRecord(cleanPath);

        return depthDataRecord;
    }
}
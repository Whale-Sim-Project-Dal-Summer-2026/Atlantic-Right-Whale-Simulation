using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BitMiracle.LibTiff.Classic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class BackscatterReader : MonoBehaviour {

    FileUtilities fileUtil;
    RasterProjector rasterProjector;

    [Header("Scriptable Objects")]
    [SerializeField] ProcessingSettings processingSettings;

    [ContextMenu("Bake Backscatter Data")]
    public void BakeData() {
        rasterProjector = new RasterProjector();
        fileUtil = new FileUtilities();

        string area = processingSettings.AreaToFilePath();
        string bsInPath = Path.Combine(Application.dataPath, "Data", "Backscatter", area);
        string bsOutDir = Path.Combine(Application.dataPath, "Data", "Processed", "Backscatter", area);
        string bathyDir = Path.Combine(Application.dataPath, "Data", "Processed", area);

        if (!Directory.Exists(bsOutDir)) {
            Directory.CreateDirectory(bsOutDir);
        }

        List<GeoTiffData> masterTiffs = readInAllTiffs(bsInPath);

        if (masterTiffs.Count == 0) {
            Debug.LogWarning("No master TIFFs found. Aborting bake.");
            return;
        }

        GeoTiffData masterBackscatter = processMasterBS(masterTiffs);

        if (masterBackscatter == null) {
            Debug.LogError("Master backscatter processing failed. Aborting bake.");
            return;
        }

        int numToRun = processingSettings.numToRun;

        List<GeoTiffData> croppedChunks = cropTiffs(masterBackscatter, bathyDir, numToRun);

        if (croppedChunks.Count == 0) {
            Debug.LogWarning("No chunks were cropped. Aborting bake.");
            return;
        }

        projectTiffs(croppedChunks);

        writeTiffsToDisk(croppedChunks, bathyDir, bsOutDir);

        Debug.Log("Backscatter baking complete. Data is ready for runtime loading.");
    }

    public List<GeoTiffData> cropTiffs(GeoTiffData masterBackscatter, string bathyDir, int numToRun) {
        List<GeoTiffData> croppedChunks = new List<GeoTiffData>();

        if (!Directory.Exists(bathyDir)) {
            Debug.LogError("Bathymetry directory missing: " + bathyDir);
            return croppedChunks;
        }
        int validCount = 0;
        
        Debug.Log(validCount + " valid points out of " + masterBackscatter.Data.Count);

        string[] bathyFiles = Directory.GetFiles(bathyDir, "*.bytes", SearchOption.TopDirectoryOnly);
        int numFiles = bathyFiles.Length;

        float resolutionX = (float)masterBackscatter.PixelScale[0];
        float resolutionY = Mathf.Abs((float)masterBackscatter.PixelScale[1]);

        for (int i = 0; i < numFiles; i++) {
            if (numToRun != -1 && i >= numToRun) {
                break;
            }

            string bathyFile = bathyFiles[i];
            DepthDataRecord depthRecord = fileUtil.binToDepthRecord(bathyFile);
            
            int chunkWidth = Mathf.CeilToInt(1001 / resolutionX);
            int chunkHeight = Mathf.CeilToInt(1001 / resolutionY);

            Vector2 chunkPos = CoordinateProjector.GeoToUTM(depthRecord.tiffData.startCoordsMeters);
            Vector2 backScatterPos = masterBackscatter.startCoordsMeters;

            float offsetX = (chunkPos.x - backScatterPos.x) / resolutionX;
            float offsetY = (backScatterPos.y - chunkPos.y) / resolutionY;

            int width = masterBackscatter.Width;
            int height = masterBackscatter.Height;
            
            List<float> chunkBS = new List<float>(chunkWidth * chunkHeight);

            float startIdx = (width * offsetY) + offsetX;

            for(int j = 0; j < chunkHeight; j++){
                int heightOffset = j * width;
                int currRowStart = Mathf.FloorToInt(heightOffset + startIdx);

                // grab starting at this row start and append, no other fancy logic. 
                chunkBS.AddRange(masterBackscatter.Data.GetRange(currRowStart, chunkWidth));
            } 

            GeoTiffData chunkTiff = new GeoTiffData();
            chunkTiff.Data = chunkBS;
            chunkTiff.Width = chunkWidth;
            chunkTiff.Height = chunkHeight;
            chunkTiff.startCoordsMeters = depthRecord.tiffData.startCoordsMeters;
            
            chunkTiff.PixelScale = new double[] { resolutionX, resolutionY, 0.0 };

            croppedChunks.Add(chunkTiff);
        }

        return croppedChunks;
    }

    public void projectTiffs(List<GeoTiffData> tiffChunks) {
        if (tiffChunks == null) {
            return;
        }


        int chunkCount = tiffChunks.Count;
        for (int i = 0; i < chunkCount; i++) {
            rasterProjector.UTMtoGEO(tiffChunks[i]);
        }
    }


    void normalizeTiff(GeoTiffData tiff){
        List<float> rawData = tiff.Data;

        float min = rawData.Min();
        float max = rawData.Max();

        // remove the max then nornamlize
        float newMax = float.MinValue;
        foreach(float val in rawData){
            if(val > newMax && val != max) newMax = val;
        }
        // rawData.RemoveAll(n => n == max);

        float range = newMax - min;
        
        List<float> normalized = new List<float>(rawData.Count);
        int numberNoData = 0;
        int rawDataCount = rawData.Count;
        for (int i = 0; i < rawDataCount; i++) {
            float dataPoint = rawData[i];
            float normal = (dataPoint - min) / range;
            normalized.Add(normal);
            if(normal == max) numberNoData++;
        }

        Debug.LogFormat("There are {0} max points, out of {1} total", numberNoData, rawDataCount);

        tiff.Data = normalized;
    }
    GeoTiffData processMasterBS(List<GeoTiffData> masterTiffs) {
        if (masterTiffs == null || masterTiffs.Count == 0) return null;

        GeoTiffData masterTiff = masterTiffs[0];
        normalizeTiff(masterTiff);
        return masterTiff;
    }

    float[] readInJSON(string filePath) {
        if (!File.Exists(filePath)) {
            return new float[2] { 0.0f, 0.0f };
        }

        string jsonContent = File.ReadAllText(filePath);
        JObject jsonObject = JObject.Parse(jsonContent);

        JToken intensityRange = jsonObject["productDefaults"]["intensityRange"];
        if (intensityRange == null) {
            return new float[2] { 0.0f, 0.0f };
        }

        string minStr = intensityRange["intensityRangeMin"]?.ToString();
        string maxStr = intensityRange["intensityRangeMax"]?.ToString();

        float min = float.TryParse(minStr, out float parsedMin) ? parsedMin : 0.0f;
        float max = float.TryParse(maxStr, out float parsedMax) ? parsedMax : 0.0f;

        return new float[2] { min, max };
    }
    
    List<GeoTiffData> readInAllTiffs(string dir) {
        if (!Directory.Exists(dir)) {
            Debug.LogError("The directory chosen is probably wrong: " + dir);
            return new List<GeoTiffData>();
        }

        string[] binSearchPattern = {"*.bytes"};
        string[] jsonSearchPattern = {"*.json"};

        IEnumerable<string> binFiles = binSearchPattern.SelectMany(pattern => Directory.EnumerateFiles(dir, pattern));
        IEnumerable<string> jsonFiles = jsonSearchPattern.SelectMany(pattern => Directory.EnumerateFiles(dir, pattern));

        int binCount = binFiles.Count();
        List<GeoTiffData> masterBackscatter = new List<GeoTiffData>(binCount);

        for (int i = 0; i < binCount; i++) {
            string binFile = binFiles.ElementAt(i);
            string jsonFile = jsonFiles.ElementAt(i);

            float[] range = readInJSON(jsonFile);
            GeoTiffData tiffData = fileUtil.ReadGeoTiff(binFile, range);
            
            masterBackscatter.Add(tiffData);
        }

        return masterBackscatter;
    }

    public void writeTiffsToDisk(List<GeoTiffData> tiffChunks, string bathyDir, string bsOutDir) {
        if (tiffChunks == null) {
            return;
        }

        if (tiffChunks.Count == 0) {
            return;
        }

        if (!Directory.Exists(bsOutDir)) {
            Directory.CreateDirectory(bsOutDir);
        }

        string[] bathyFiles = Directory.GetFiles(bathyDir, "*.bytes", SearchOption.TopDirectoryOnly);
        int chunkCount = tiffChunks.Count;

        for (int i = 0; i < chunkCount; i++) {
            if (i >= bathyFiles.Length) {
                Debug.LogError("Mismatch between number of tiff chunks and available bathymetry files.");
                break;
            }

            string fileName = Path.GetFileName(bathyFiles[i]);
            string outPath = Path.Combine(bsOutDir, fileName);

            fileUtil.writeGeoTiffToBinary(tiffChunks[i], outPath);
        }
    }
}
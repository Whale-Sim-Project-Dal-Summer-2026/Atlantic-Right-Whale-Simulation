using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

public class BathymetryReader : MonoBehaviour {

    BathymetryPatcher patcher;
    FileUtilities fileUtil;

    [Header("Scriptable Objects")]
    [SerializeField] ProcessingSettings processingSettings;

    char[] fileDelims = {'/', '\\'};

    [ContextMenu("Bake Bathymetry Data")]
    public void BakeData() {
        patcher = new BathymetryPatcher(processingSettings);
        fileUtil = new FileUtilities();
 
        string path = processingSettings.AreaToFilePath();

        string readingDir = Path.Combine(Application.dataPath,"Private", "Bathymetry", path);
        string writingDir = Path.Combine(Application.dataPath, "Private", "Processed", path);
        
        if (!Directory.Exists(writingDir)) {
            Directory.CreateDirectory(writingDir);
        }
        
        readInAllTiffs(readingDir, writingDir);        
    }

    private void generateChunkOffsets(List<DepthDataRecord> records, List<string> fileNames) {

        Vector2 min = new Vector2(int.MaxValue, int.MaxValue);

        int numRecords = records.Count;
        List<Vector2> coordsList = new List<Vector2>(numRecords);
        
        for(int i = 0; i < numRecords; i++) {
            string filename = fileNames[i];

            Tuple<Vector2,Vector2> coords = parseCoords(filename);
            Vector2 geoCoords = coords.Item2;

            if(geoCoords.x < min.x) {
                min.x = geoCoords.x;
            }
            
            if(geoCoords.y < min.y) {
                min.y = geoCoords.y;
            }

            coordsList.Add(geoCoords);
        }

        for(int i = 0; i < numRecords; i++) {

            DepthDataRecord chunk = records[i];
            Vector2 coord = coordsList[i];

            Vector2 normalized = coord - min;
            chunk.ChunkPosition = normalized * 10;

            chunk.tiffData.startCoordsMeters = coord;

            records[i] = chunk;
        }

    }
    
    private void readInAllTiffs(string readingDir, string writingDir) {
        
        if(!Directory.Exists(readingDir)) {
            Debug.LogError("The directory chosen is probably wrong: " + readingDir);
            return;
        }

        int numToRun = processingSettings.numToRun;

        string[] searchPatterns = {"*.bytes"};

        IEnumerable<string> files = searchPatterns.SelectMany(pattern => Directory.EnumerateFiles(readingDir, pattern));
        
        int numFiles = files.Count();

        List<string> fileNames = new List<string>(numFiles);
        List<DepthDataRecord> records = new List<DepthDataRecord>(numFiles);

        int count = 0;
        foreach(string file in files) {
            DepthDataRecord depthDataRecord = readTiff(Path.Combine(readingDir, file));

            string[] fileSplit = file.Split(fileDelims);
            string name = fileSplit[fileSplit.Length - 1];

            fileNames.Add(name);
            records.Add(depthDataRecord);
            count++;
            
            if(numToRun != -1 && count >= numToRun) break;
        }

        generateChunkOffsets(records, fileNames);

        for(int i = 0; i < records.Count; i++) {
            DepthDataRecord chunk = records[i];
            string fileName = fileNames[i];

            string path = Path.Combine(writingDir, fileName);
            fileUtil.writeToBinary(chunk, path);
        }

        Debug.Log("Bathymetry baking done.");
    }

    private Tuple<Vector2, Vector2> parseCoords(string fileName) {
        string nameWithExt = fileName.Split("_")[1];
        string extractedName = nameWithExt.Split(".")[0];

        string[] northSplit = extractedName.Split("N");
        string northStr = northSplit[0];
        string westStr = northSplit[1].Substring(0,northSplit[1].Length - 1);

        int north = int.Parse(northStr);
        int west = int.Parse(westStr);

        float lat = north / 100f;
        float lon = -(west / 100f);

        Vector2 coords = new Vector2(lon, lat);
        Vector2 utm = CoordToUTM.Convert(coords);
        Tuple<Vector2, Vector2> data = new Tuple<Vector2, Vector2>(utm, coords);
        
        return data;
    }

    private DepthDataRecord readTiff(string filePath) {
        DepthDataRecord depthDataRecord = new DepthDataRecord();

        if (string.IsNullOrEmpty(filePath)) {
            return depthDataRecord;
        }
        
        GeoTiffData data = fileUtil.ReadGeoTiff(filePath, new float[]{processingSettings.MaxDepth, processingSettings.SeaLevel});
        depthDataRecord.tiffData = data;
        depthDataRecord.tiffData.Data = patcher.patchChunk(data);

        return depthDataRecord;
    }
}
using UnityEngine;
using System.Collections.Generic;
using System;
using NUnit.Framework;
using Stopwatch = System.Diagnostics.Stopwatch;
using System.Linq;
using Unity.VisualScripting;

public class RasterProjector
{
    // hard coded UTM zone and north for now :')
    int zone;
    bool isNorth;

    float targetResolution;
    int numNeighbors;
    KNN kNN;
    // how many points along X and Y in each chunk?
    int pointsPerChunkDim;

    public RasterProjector()
    {
        targetResolution = .10f;
        kNN = new KNN();
        isNorth = true;
        zone = 20;
        numNeighbors = 4;
        pointsPerChunkDim = 1001;
    }
    
    // 1. Get Bounding Box
    // Determine new Array Length
    // iterate over target array, filling in data points
    public GeoTiffData UTMtoGEO(GeoTiffData geoTiffData)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        Vector4 bbox = getBoundingBox(geoTiffData);

        float xLength = bbox[1] - bbox[0];
        float yLength = bbox[3] - bbox[2];

        

        int numChunksX = Mathf.CeilToInt(xLength / targetResolution);
        int numChunksY = Mathf.CeilToInt(yLength / targetResolution);

        int arrLenX = numChunksX * pointsPerChunkDim;
        int arrLenY = numChunksY * pointsPerChunkDim;

        int height = geoTiffData.Height;
        int width = geoTiffData.Width;
    
        double[] pixelScale = geoTiffData.PixelScale;

        int geoPosSize = height * width;
        int geoDataSize = arrLenX * arrLenY;

        List<Vector3> geoPosArr = new List<Vector3>(geoPosSize);

        float[] geoDataArr = new float[geoDataSize];

        List<float> rawData = geoTiffData.Data;
        Debug.Assert(rawData.Count == geoPosSize, "The data and the height * width dont match for backscatter projection");
        // forward pass
        for(int y = 0; y < height; y++){
            for(int x = 0; x < width; x++){
                int idx = y * width + x;

                float val = rawData[idx];

                double xScaledPos = x * pixelScale[0];
                double yScaledPos = y * pixelScale[1];

                Vector2 utm = new Vector2((float)xScaledPos, (float)yScaledPos);

                Vector2 geo = CoordinateProjector.UTMToGeo(utm, 20, true);

                Vector3 geoDepth = new Vector3(geo.x,val, geo.y);
                geoPosArr.Add(geoDepth);
            }
        }

        Vector2 startCoordsUTM = geoTiffData.startCoordsMeters;

        Vector2 geoStart = CoordinateProjector.UTMToGeo(startCoordsUTM,20,true);

        // backward pass
        for(int y = 0; y < arrLenY; y++){
            for(int x = 0; x < arrLenX; x++){
                int idx = y * arrLenX + x;
                
                float xScaledPos = (x * targetResolution) + geoStart.x;
                float yScaledPos = (y * targetResolution) + geoStart.y;

                Vector2 geoPos = new Vector2(xScaledPos, yScaledPos);
                // grab 4 nearest neighbors in geo space
                
                Tuple<float[], int>[] nearest = kNN.nearestNeighbors(geoTiffData,geoPos,numNeighbors, false, 0);

                float intensitySum = 0.0f;
                 foreach(Tuple<float[], int> val in nearest){
                    float[] nieghborPos =  val.Item1;
                    int originalIndex = val.Item2;

                    float intensity = rawData[originalIndex];
                    intensitySum += intensity;
                }

                float avg = intensitySum / numNeighbors;
                
                geoDataArr[idx] = avg;
            }
        }
        geoTiffData.startCoordsMeters = geoStart;
        geoTiffData.Data = geoDataArr.ToList();
        geoTiffData.Width = arrLenX;
        geoTiffData.Height = arrLenY;
        geoTiffData.PixelScale = new double[] { targetResolution, targetResolution, 0.0 };
        stopwatch.Stop();
        Debug.LogFormat("backscatter processing took {0} ms", stopwatch.ElapsedMilliseconds);
        return geoTiffData;
    }


    public GeoTiffData GEOtoUTM(GeoTiffData geoTiffData) {
        if (geoTiffData == null) {
            return null;
        }

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        Vector4 bbox = getBoundingBox(geoTiffData);

        Vector2 minGeo = new Vector2(bbox[0], bbox[2]);
        Vector2 maxGeo = new Vector2(bbox[1], bbox[3]);
        
        Vector2 minUTM = CoordinateProjector.GeoToUTM(minGeo);
        Vector2 maxUTM = CoordinateProjector.GeoToUTM(maxGeo);

        float xLength = maxUTM.x - minUTM.x;
        float yLength = maxUTM.y - minUTM.y;

        int height = geoTiffData.Height;
        int width = geoTiffData.Width;

        float targetResolutionX = xLength / width;
        float targetResolutionY = yLength / height;
        
        float calculatedTargetResolution = Mathf.Min(targetResolutionX, targetResolutionY);

        int arrLenX = Mathf.CeilToInt(xLength / calculatedTargetResolution);
        int arrLenY = Mathf.CeilToInt(yLength / calculatedTargetResolution);

        int utmPosSize = height * width;
        int utmDataSize = arrLenX * arrLenY;

        List<Vector3> utmPosArr = new List<Vector3>(utmPosSize);
        float[] utmDataArr = new float[utmDataSize];

        List<float> rawData = geoTiffData.Data;
        Debug.Assert(rawData.Count == utmPosSize, "The data and the height * width dont match for backscatter projection");

        // forward pass
        double[] pixelScale = geoTiffData.PixelScale;
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                int idx = (y * width) + x;

                float val = rawData[idx];

                double xScaledPos = x * pixelScale[0];
                double yScaledPos = y * pixelScale[1];

                Vector2 geo = new Vector2((float)xScaledPos, (float)yScaledPos);
                Vector2 utm = CoordinateProjector.GeoToUTM(geo);

                Vector3 utmDepth = new Vector3(utm.x, val, utm.y);
                utmPosArr.Add(utmDepth);
            }
        }

        Vector2 startCoordsGeo = geoTiffData.startCoordsMeters;
        Vector2 utmStart = CoordinateProjector.GeoToUTM(startCoordsGeo);

        // backward pass
        for (int y = 0; y < arrLenY; y++) {
            for (int x = 0; x < arrLenX; x++) {
                int idx = (y * arrLenX) + x;
                
                float xScaledPos = (x * targetResolution) + utmStart.x;
                float yScaledPos = (y * targetResolution) + utmStart.y;

                Vector2 utmPos = new Vector2(xScaledPos, yScaledPos);
                
                Tuple<float[], int>[] nearest = kNN.nearestNeighbors(geoTiffData, utmPos, numNeighbors, false, 0);

                float intensitySum = 0.0f;
                foreach (Tuple<float[], int> val in nearest) {
                    int originalIndex = val.Item2;
                    float intensity = rawData[originalIndex];
                    intensitySum += intensity;
                }

                float avg = intensitySum / numNeighbors;
                
                utmDataArr[idx] = avg;
            }
        }

        geoTiffData.startCoordsMeters = utmStart;
        geoTiffData.Data = utmDataArr.ToList();
        geoTiffData.Width = arrLenX;
        geoTiffData.Height = arrLenY;
        geoTiffData.PixelScale = new double[] { targetResolution, targetResolution, 0.0 };
        
        stopwatch.Stop();
        Debug.LogFormat("forward projection took {0} ms", stopwatch.ElapsedMilliseconds);
        
        return geoTiffData;
    }


// gets the bounding box in lat long
// x min, x max, y min, y max
    Vector4 getBoundingBox(GeoTiffData tiffData)
    {
        int width = tiffData.Width;
        int height = tiffData.Height;

        double[] pixelScale = tiffData.PixelScale;

        Vector2 startingCoords = tiffData.startCoordsMeters;

        float maxX = startingCoords.x + (float)(height * pixelScale[1]);
        float maxY =  startingCoords.y + (float)(width * pixelScale[0]);
        

        Vector2 endCoords = new Vector2(maxX, maxY);

        Vector2 startCoordsGeo = CoordinateProjector.UTMToGeo(startingCoords,zone,isNorth);
        Vector2 EndCoordsGeo = CoordinateProjector.UTMToGeo(endCoords,zone,isNorth);

        Vector4 bbox = new Vector4(startCoordsGeo.x, EndCoordsGeo.x, startCoordsGeo.y, EndCoordsGeo.y);

        return bbox;
        
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

public class BathymetryPatcher{

    int numNeighbors = 10;
    ProcessingSettings settings;
    
    Noise noise;
    KNN Knn = new KNN();

    public BathymetryPatcher(ProcessingSettings settings)
    {
        this.settings = settings;
        noise = new Noise(settings);
    }


// add to Kd Tree
// grab nearest X neighbors using KNN
// do inverse distance weighting on nearest points to estimate missing data point
    public List<float> patchChunk(GeoTiffData data){
        Knn.ResetTree();
        
        List<float> depths = data.Data;

        int width = data.Width;
        int height = data.Height;

        int[] size = {width, height};
        Vector2 target;

        for(int i = 0; i < depths.Count; i++)
        {
            // skip valid points
            if(depths[i] < settings.SeaLevel) continue;

            float x = i % width;
            float y = i / width;

            target.x = x;
            target.y = y;

            Tuple<float[], int>[] nearest = Knn.nearestNeighbors(data, target, numNeighbors, true, settings.SeaLevel);

            float numerator = 0.0f;
            float denominator = 0.0f;
            bool exactMatch = false;
            for (int j = 0; j < nearest.Length; j++) {
                float[] neighborPosition = nearest[j].Item1;
                int originalIndex = nearest[j].Item2;
                
                
                float dx = target[0] - neighborPosition[0];
                float dy = target[1] - neighborPosition[1];
                float squaredDistance = (dx * dx) + (dy * dy);

                float knownDepth = noise.addNoiseToDepth(depths[originalIndex], target, size, dx + dy);

                if (squaredDistance <= 0.0f) {
                    depths[i] = knownDepth;
                    exactMatch = true;
                    break; 
                }

                float weight = 1.0f / squaredDistance; 
                numerator += knownDepth * weight;
                denominator += weight;
            }

            if(!exactMatch && !Mathf.Approximately(denominator, 0.0f))
            {
                float newValue = numerator / denominator;
 
                depths[i] = newValue;
            }

        }

        return depths;


    }
    
}
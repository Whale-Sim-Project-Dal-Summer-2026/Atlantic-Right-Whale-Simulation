using System;
using System.Collections.Generic;
using Supercluster.KDTree;
using UnityEngine;

public class KNN
{
    KDTree<float, int> tree;

    public void ResetTree() {
        tree = null;
    }
    private void generateTree(GeoTiffData geoTiffData, bool skipSeaLevel = false, float seaLevel = 0)
    {

        List<float> values = geoTiffData.Data;

        int width = geoTiffData.Width;
        int height = geoTiffData.Height;

        int depthsCount = values.Count;
        Func<float[], float[], double> L2Metric = (x, y) => {
            float dx = x[0] - y[0];
            float dy = x[1] - y[1];
            return (dx * dx) + (dy * dy);
        };

        int validPointCount = 0;
        for (int i = 0; i < depthsCount; i++) {
            if (values[i] < seaLevel || !skipSeaLevel) {
                validPointCount++;
            }
        }

        float[][] validPoints = new float[validPointCount][];
        int[] validIndices = new int[validPointCount];

        int currentIndex = 0;
        for (int i = 0; i < depthsCount; i++) {
            // skip invalid points
            if (values[i] >= seaLevel && skipSeaLevel) continue;

            float x = i % width;
            float y = i / width;

            validPoints[currentIndex] = new float[] { x, y };
            validIndices[currentIndex] = i;
            currentIndex++;
        }

        KDTree<float, int> tree = new KDTree<float, int>(dimensions: 2,points: validPoints, nodes: validIndices, metric: L2Metric);
        this.tree = tree;
    }


    public Tuple<float[], int>[] nearestNeighbors(GeoTiffData data, Vector2 targetPoint, int numNeighbors, bool skipSeaLevel, float seaLevel)
    {
        if(tree == null) generateTree(data,skipSeaLevel, seaLevel);

        float[] target = {targetPoint.x, targetPoint.y};
        Tuple<float[], int>[] res = tree.NearestNeighbors(target, numNeighbors);
        return res;
    }
}
    

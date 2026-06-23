using UnityEngine;
using System.Collections.Generic;

public class GeoTiffData {
    public int Width { get; set; }
    public int Height { get; set; }
    public double[] PixelScale { get; set; }


    public Vector2 startCoordsMeters { get; set; }
    public List<float> Data { get; set; }
}


using System.Collections.Generic;
using UnityEngine;

public class ChunkData{
    public DepthDataRecord MeshData {get; set;}
    public GeoTiffData BackscatterData {get; set;}

    public ChunkData(DepthDataRecord md = null, GeoTiffData gt = null)
    {
        MeshData = md;
        BackscatterData = gt;
    }
}

[CreateAssetMenu(fileName = "Chunks", menuName = "Scriptable Objects/Chunks")]

public class Chunks : ScriptableObject
{
    public List<ChunkData> chunks {get; set;}
}
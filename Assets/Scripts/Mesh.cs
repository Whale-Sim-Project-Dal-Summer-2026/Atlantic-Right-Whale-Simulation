using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

using UnityMesh = UnityEngine.Mesh;
using UnityEngine.InputSystem;

namespace MeshGeneration {

    public class Mesh : MonoBehaviour {

        [Header("General")]
        [Tooltip("Parent of all the mesh chunks")]
        [SerializeField] GameObject parent;

        [SerializeField] Material meshMaterial;

        [Header("Reload Meshes")]
        [Tooltip("Press this to reload all the meshes from the binary files found in Assets/Data/Processed/(Area)")]
        [SerializeField] bool reloadMesh;

        FileUtilities fileUtil;
        
        [Header("Scriptable Objects")]
        [SerializeField] ProcessingSettings processingSettings;
        [SerializeField] List<DepthDataRecord> records;

        string byteFileDir;

        void Start() {
            records = new List<DepthDataRecord>();
            fileUtil = new FileUtilities();
            string areaPath = processingSettings.AreaToFilePath();
            byteFileDir = Path.Combine(Application.dataPath, "Data", "Processed", areaPath);
            reloadMesh = true;
        }

        void Update()
        {
            if (Keyboard.current.mKey.isPressed)
            {
                reloadMesh = true;
            }
            if(!reloadMesh) return;
            
            clearOldChunks();
            startMeshPipeline();
            reloadMesh = false;
        }

        void clearOldChunks() {
            foreach(Transform child in parent.GetComponentsInChildren<Transform>(true)) {
                if(child == parent.transform) continue;

                Destroy(child.gameObject);
            }
        }

        void startMeshPipeline() {

            traversePath(byteFileDir);
            generateAllMeshes();
        }

        void traversePath(string path) {
            int numToRun = processingSettings.numToRun;
            
            if (string.IsNullOrEmpty(path)) return;
            if (!Directory.Exists(path)) return;

            string[] files = Directory.GetFiles(path, "*.bytes", SearchOption.TopDirectoryOnly);
            
            records.Clear();

            int count = 0;
            foreach (string file in files) {
                if (count >= numToRun && numToRun != -1) continue;
                
                DepthDataRecord depthDataRecord = fileUtil.binToDepthRecord(file);
                records.Add(depthDataRecord);
                count++;
            }
        }

        void generateAllMeshes() {
            float chunkSize = processingSettings.chunkSize;
            foreach (DepthDataRecord record in records) {

                Vector2 chunkPos = record.ChunkPosition;

                float west = Mathf.RoundToInt(chunkPos.x) * chunkSize;
                float north = Mathf.RoundToInt(chunkPos.y) * chunkSize;

                UnityMesh chunkMesh = generateMeshData(record);
                GameObject chunkObject = new GameObject("TerrainChunk_W" + west + "_N" + north);
                chunkObject.transform.SetParent(parent.transform);
                
                MeshFilter meshFilter = chunkObject.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = chunkObject.AddComponent<MeshRenderer>();
                meshRenderer.material = meshMaterial;

                meshFilter.mesh = chunkMesh;

                chunkObject.transform.position = new Vector3(north, 0, west);
            }
        }

        UnityMesh generateMeshData(DepthDataRecord record){
            float chunkSize = processingSettings.chunkSize;
            List<Vector3> positions = new List<Vector3>();

            UnityMesh mesh = new UnityMesh{
                indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
            };


            
            List<float> depths = record.tiffData.Data;

            int height = record.tiffData.Height;
            int width = record.tiffData.Width;
            float distanceBetweenPointsX = chunkSize / (width - 1);
            float distanceBetweenPointsZ = chunkSize / (height - 1);


            int depthsCount = depths.Count;

            positions.Capacity = positions.Count + depths.Count;
            Vector3 curr = new Vector3();
            for(int i = 0; i < depthsCount; i++){
                float x = (i / height) * distanceBetweenPointsX;
                float z = (i % width) * distanceBetweenPointsZ;

                float y = depths[i];

                curr.x = x;
                curr.y = y;
                curr.z = z;

                positions.Add(curr);
            }

            //generate indicies
            int numTrianglesPerCol = (height - 1) * 2;
            int numTriangles = numTrianglesPerCol * (width - 1);


            int numQuads = numTriangles / 2;

            List<int> triangles = new List<int>(numTriangles);
            for(int i = 0; i < numQuads; i++){
                int x = i % (width - 1);
                int y = i / (height - 1);

                int startingIndex = (y * width) + x;

                int v1 = startingIndex;
                int v2 = startingIndex + 1;
                int v3 = startingIndex + height;

                int v4 = v2;
                int v5 = v2 + width;
                int v6 = v3;

                triangles.Add(v1);
                triangles.Add(v2);
                triangles.Add(v3);
                triangles.Add(v4);
                triangles.Add(v5);
                triangles.Add(v6);

            }
            mesh.SetVertices(positions);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            Bounds bounds = mesh.bounds;
            float minX = bounds.min.x;
            float minZ = bounds.min.z;

            float sizeX = bounds.size.x;
            float sizeZ = bounds.size.z;

            List<Vector2> uvs = new List<Vector2>(positions.Count);

            foreach(Vector3 vertex in positions)
            {
                float u = (vertex.x - minX) / sizeX;
                float v = (vertex.z - minZ) / sizeZ;
                Vector2 uv = new Vector2(u,v);

                uvs.Add(uv);
            }
            
            mesh.uv = uvs.ToArray();
            return mesh;
        }
    }
}
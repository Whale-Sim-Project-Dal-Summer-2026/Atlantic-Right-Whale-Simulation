using agxCollide;
using AGXUnity;
using Unity.VisualScripting;
using UnityEngine;


// grab mesh
// where is pivot? io believe it is top left,
// grab bounds, take the min + sea level to know how tall the water should be

public class MeshWaterPosition : MonoBehaviour{
    [SerializeField] GameObject meshParent;

    [SerializeField] GameObject waterBoxPrefab;

    [SerializeField] ProcessingSettings settings;

    [SerializeField] WindAndWaterManager windAndWaterManager;

    [SerializeField] bool spawnWater = true;

    [SerializeField] GameObject waterParent;
    

    void Awake(){
        // MeshGeneration.Mesh.OnMeshGenerated += assignWaterToChunks;
    }


    void Update(){
        if (spawnWater){
            assignWaterToChunks();
            spawnWater = false;
        }
    }
    public void assignWaterToChunks() {
        Transform[] chunks = meshParent.GetComponentsInChildren<Transform>();

        foreach (Transform trans in chunks) {
            if (trans == meshParent.transform) {
                continue;
            }

            GameObject go = trans.gameObject;
            MeshFilter mf = go.GetComponent<MeshFilter>();

            if (mf == null) {
                Debug.LogWarningFormat("Cannot assign water to chunk {0}, theres no mesh filter", go.name);
                continue;
            }

            Bounds meshBounds = mf.sharedMesh.bounds;

            float worldSeaLevelY = settings.SeaLevel;
            float worldFloorY = trans.position.y + meshBounds.min.y;
            float totalHeightY = worldSeaLevelY - worldFloorY;

            if (totalHeightY <= 0f) {
                continue;
            }

            float halfHeightY = totalHeightY / 2f;
            float halfWidthX = meshBounds.size.x / 2f;
            float halfDepthZ = meshBounds.size.z / 2f;

            Vector3 worldWaterCenter = trans.TransformPoint(meshBounds.center);
            worldWaterCenter.y = worldSeaLevelY - halfHeightY;

            GameObject waterInstance = Instantiate(waterBoxPrefab, worldWaterCenter, trans.rotation, waterParent.transform);
            AGXUnity.Collide.Box box = waterInstance.GetComponent<AGXUnity.Collide.Box>();

            if (box == null) {
                Debug.LogErrorFormat("The waterBoxPrefab is missing an AGXUnity.Collide.Box component on {0}", waterInstance.name);
                continue;
            }

            box.HalfExtents = new Vector3(halfWidthX, halfHeightY, halfDepthZ);
        }
        windAndWaterManager.Water = waterParent;
    }
}
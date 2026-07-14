using UnityEngine;
using AGXUnity;
using System.Collections.Generic;
using AGXUnity.IO;

public class ColliderSwapper : ScriptComponent{
    

    [SerializeField] GameObject parent;
    [SerializeField] ShapeMaterial shapeMaterial;

    // WARNING! This will not apply the mesh decimation, you need to go one by one to "apply the changes"
    // I attempted to automate it, but I had no luck. I went into the source code to see how they apply it, but after folloiwng how they do it, nothing worked. 
    [ContextMenu("Attach AGX Mesh Colliders")]

    void traverseParent(){
        Transform[] children = parent.GetComponentsInChildren<Transform>();

        foreach(Transform trans in children){
            attachNewCollider(trans.gameObject);
        }
    }

    void attachNewCollider(GameObject go){
        MeshFilter mesh = go.GetComponent<MeshFilter>();

        if(!mesh) return;
        AGXUnity.Collide.Mesh agxMesh = go.GetComponent<AGXUnity.Collide.Mesh>();

        if(agxMesh == null){
           agxMesh = go.AddComponent<AGXUnity.Collide.Mesh>();
        }

        agxMesh.Options.Mode = AGXUnity.Collide.CollisionMeshOptions.MeshMode.Trimesh;

        agxMesh.Options.MergeNearbyDistance = 0.003f;
        agxMesh.Options.MergeNearbyEnabled = false;

        
        agxMesh.Options.ReductionRatio = .2f;
        agxMesh.Options.ReductionAggressiveness = 7f;

        agxMesh.Options.ReductionEnabled = true;
        agxMesh.Material = shapeMaterial;

        agxMesh.IsSensor = false;

        MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
        MeshCollider meshCollider = go.GetComponent<MeshCollider>();

        // AGXUnity.Collide.CollisionMeshGenerator collisionMeshGenerator = new AGXUnity.Collide.CollisionMeshGenerator();

        // collisionMeshGenerator.Generate(new AGXUnity.Collide.Mesh[] {agxMesh});
        // var results = collisionMeshGenerator.CollectResults();
        //     foreach ( var result in results ) {
        //         result.Mesh.Options = result.Options;
        //         result.Mesh.PrecomputedCollisionMeshes = result.CollisionMeshes;
        //     }

        // agxMesh.OnPrecomputedCollisionMeshDataDirty();

        if (meshRenderer != null) {
            meshRenderer.enabled = false;
        }
        
        if(meshCollider != null){
            meshCollider.enabled = false;
        }

    }
}
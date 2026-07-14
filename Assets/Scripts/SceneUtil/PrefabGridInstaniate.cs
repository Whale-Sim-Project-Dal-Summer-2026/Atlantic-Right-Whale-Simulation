using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using AGXUnity;
using AGXUnity.Utils;

public class RopePositioning : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField] GameObject ropeParent;

    [SerializeField] GameObject prefab;
    List<GameObject> ropes;
    [Header("Spawn Parameters")]
    [SerializeField] Vector2Int spawnDimension;

    [SerializeField] float ropeSpacing = 5.0f;

    [Header("Reload?")]
    [SerializeField] bool spawnRopes;

    void Start() {
        ropes = new List<GameObject>();
        spawnRopes = true;
    }

    // Destroys all currently tracked ropes and clears the list
    void ClearRopes() {
        foreach (GameObject rope in ropes) {
            if (rope == null) {
                continue;
            }
            Destroy(rope);
        }
        ropes.Clear();
    }

    void CreateRopes() {
        for (int z = 0; z < spawnDimension.y; z++) {
            for (int x = 0; x < spawnDimension.x; x++) {
                Vector3 position = new Vector3(x * ropeSpacing, 0, z * ropeSpacing);
                Vector3 finalPosition = position + ropeParent.transform.position;
                GameObject obj = Instantiate(prefab, finalPosition, Quaternion.identity, ropeParent.transform);

                // agx.RigidBody rb = obj.GetComponent<agx.RigidBody>();

                // rb.setVelocity(Random.onUnitSphere.ToHandedVec3());
                // rb.setPosition(position.ToHandedVec3());

                ropes.Add(obj);
            }
        }
    }

    // Update is called once per frame
    void Update() {
        if (!spawnRopes) {
            return;
        }

        ClearRopes();
        CreateRopes();

        spawnRopes = false;
    }
}

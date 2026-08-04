using UnityEngine;
using System.Collections.Generic;

public class EnvironmentReset : MonoBehaviour
{
    CameraControls controls;


    //Stores Transformation Data
    [System.Serializable]
    struct TransformData
    {
        public Vector3    position;
        public Quaternion rotation;
        public Vector3    scale;
    }

    //Stores the transformation and 
    Dictionary<Transform, TransformData> savedTransforms = new();

    void Awake()
    {
        controls = new CameraControls();
        SaveTransforms(transform); // Save before anything moves
    }
    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void SaveTransforms(Transform parent)
    {
        // Gets all transforms within the overall parent
        foreach (Transform t in parent.GetComponentsInChildren<Transform>())
        {
            savedTransforms[t] = new TransformData
            {
                position = t.localPosition,
                rotation = t.localRotation,
                scale    = t.localScale
            };
        }
    }

    void ResetEnvironment()
    {
        foreach (var item in savedTransforms)
        {
            GameObject currentGO = item.Key.gameObject;
            TransformData currentTD  = item.Value;


            currentGO.transform.localPosition = currentTD.position;
            currentGO.transform.localRotation = currentTD.rotation;
            currentGO.transform.localScale    = currentTD.scale;

            Rigidbody currentRB = currentGO.GetComponent<Rigidbody>();
            if (currentRB == null) continue;
            currentRB.linearVelocity = Vector3.zero;
            currentRB.angularVelocity = Vector3.zero;

            
        }
    }

    void Update()
    {
        if (controls.Player.Reset.triggered)
            ResetEnvironment();
    }
}
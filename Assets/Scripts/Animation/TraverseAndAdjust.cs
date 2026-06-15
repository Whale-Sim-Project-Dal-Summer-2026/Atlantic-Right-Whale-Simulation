
using UnityEngine;



public class TraverseAndAdjust : MonoBehaviour
{

    public Transform stopObject;


    float pitch, roll, yaw; 
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        applyRotation(0,0,0);
    }

    void applyRotation(float pitch, float roll, float yaw)
    {
        Transform nextObject;
        Transform current = transform;
        bool stop = false;
        while (!stop)
        {
            nextObject = current.GetChild(0);

            if (nextObject == stopObject)
            {
                stop=true;
                continue;
            }

            current.rotation =  Quaternion.Euler(15, 0, 0);



            current = nextObject;

        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

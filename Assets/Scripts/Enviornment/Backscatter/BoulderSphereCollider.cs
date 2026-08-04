using AGXUnity;
using openplx;
using UnityEngine;

public class BoulderSphereCollider : ScriptComponent{
    

    protected override bool Initialize() {
        

        changeSphereRadius();
        return base.Initialize();        
    }


    void changeSphereRadius(){
        Vector3 scale = gameObject.transform.localScale;


        AGXUnity.Collide.Sphere sphere = gameObject.GetComponent<AGXUnity.Collide.Sphere>().GetInitialized<AGXUnity.Collide.Sphere>();
        float avg = (scale.x + scale.y + scale.z) / 3.0f;

        sphere.Native.setRadius(avg / 2.0f);
    }
    
}
using System.Collections.Generic;
using AGXUnity.Utils;
using UnityEngine;

public class WhaleFlatener : MonoBehaviour{
    [SerializeField] WhaleBones whaleBones;

    [SerializeField] GameObject grandParent;


    void Start()
    {
        createFlat();
    }
    void createFlat(){

        // resetParent();

        GameObject leftFinParent = new GameObject("Left_Fin_Parent");
        GameObject rightFinParent = new GameObject("Right_Fin_Parent");
        GameObject headParent = new GameObject("Head_Parent");
        GameObject tailParent = new GameObject("Tail_Parent");

        grandParent.AddChild(leftFinParent);
        grandParent.AddChild(rightFinParent);
        grandParent.AddChild(headParent);
        grandParent.AddChild(tailParent);

        traverseANDcreateFlat(leftFinParent, whaleBones.leftFinStartBone, whaleBones.leftFinStopBone);
        traverseANDcreateFlat(rightFinParent, whaleBones.rightFinStartBone, whaleBones.rightFinStopBone);
        traverseANDcreateFlat(tailParent, whaleBones.tailStartBone, whaleBones.tailStopBone);

        assignHead(headParent);
    }


    void resetParent(){
        for (int i = grandParent.transform.childCount - 1; i >= 0; i--){
            Destroy(grandParent.transform.GetChild(i).gameObject);
        }
    }

    void assignHead(GameObject parent){

        GameObject OGbot = whaleBones.mouthBottomBone;
        GameObject bot = duplicateGameObject(OGbot);

        GameObject OGtop = whaleBones.mouthTopBone;
        GameObject top = duplicateGameObject(OGtop);
        
        parent.AddChild(top);
        parent.AddChild(bot);
    }

    List<GameObject> traverseANDcreateFlat(GameObject parent, GameObject start, GameObject end){
        List<GameObject> GOs = new List<GameObject>();


        Transform[] children = start.GetComponentsInChildren<Transform>();
        

        foreach(Transform childTransform in children){
            GameObject child = childTransform.gameObject;
            
            if(child == end) return GOs;

            GameObject dupe = duplicateGameObject(child);
            parent.AddChild(dupe);

            GOs.Add(child);
        }

        return GOs;

    }



    GameObject duplicateGameObject(GameObject go){
        GameObject dupe = new GameObject(go.name);


        Component[] components = go.GetComponents<Component>();

        foreach(Component component in components){
            if(component is Transform) continue;

            System.Type componentType = component.GetType();
            dupe.AddComponent(componentType);
        }
        return dupe;
    }
    
}
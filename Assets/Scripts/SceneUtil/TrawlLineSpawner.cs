using AGXUnity;
using AGXUnity.Rendering;
using AGXUnity.Utils;
using Unity.VisualScripting;
using UnityEngine;

public class TrawlLineSpawner : ScriptComponent{
    [SerializeField] GameObject lobsterTrap;
    [SerializeField] GameObject buoy;

    [SerializeField] Material wireMat;

    int minOnTrawl = 5;
    int maxOnTrawl = 8;

    [SerializeField] float spacingDelta;

    [SerializeField] float depth;

    protected override bool Initialize(){
        routeWire();
        return base.Initialize();
        
    }

    public void routeWire(){
        AGXUnity.RigidBody buoyRB = buoy.GetComponent<AGXUnity.RigidBody>().GetInitialized<AGXUnity.RigidBody>();

        Vector3 buoyCurrPos = buoy.transform.position;
        buoyCurrPos.y = depth;

        agx.Vec3 buoyPos = buoyCurrPos.ToHandedVec3();

        buoyRB.Native.setPosition(buoyPos);

        Wire wire = gameObject.AddComponent<Wire>();
        WireRenderer renderer = gameObject.AddComponent<WireRenderer>();

        renderer.Material = wireMat;
        
        wire.Diameter = .1f;
        
        float buoyHeight = buoy.GetComponentInChildren<AGXUnity.Collide.Capsule>().Height;

        WireRouteNode buoyRouteNode = WireRouteNode.Create(Wire.NodeType.BodyFixedNode,buoy,Vector3.up * -buoyHeight, Quaternion.identity);
        wire.Route.Add(buoyRouteNode);


        //float randT = Random.Range(0.0f,1.0f);
        int numTraps = 5; //Mathf.RoundToInt(Mathf.Lerp(minOnTrawl,maxOnTrawl,randT));
        
        
        float trapHeight = lobsterTrap.GetComponentInChildren<AGXUnity.Collide.Box>().HalfExtents.y;


        for(int i = 0; i < numTraps; i++){
            float deltaOffset = spacingDelta * i;
            
            Vector3 offset = new Vector3(deltaOffset, -depth, 0);
            Vector3 position = buoyRB.Native.getPosition().ToHandedVector3() + offset;

            GameObject trap = Instantiate(lobsterTrap,position,Quaternion.identity,gameObject.transform);

            Wire.NodeType nodeType = i == numTraps - 1 ? Wire.NodeType.BodyFixedNode : Wire.NodeType.EyeNode;

            WireRouteNode trapRouteNode = WireRouteNode.Create(nodeType,trap,(Vector3.up * trapHeight / 2), Quaternion.identity);
            wire.Route.Add(trapRouteNode);

        }
        GetSimulation().add(wire.Native);
        
    }

}
using AGXUnity;
using AGXUnity.Rendering;
using Unity.VisualScripting;
using UnityEngine;

public class TrawlLineSpawner : ScriptComponent{
    [SerializeField] GameObject lobsterTrap;
    [SerializeField] GameObject buoy;

    [SerializeField] Material wireMat;

    int minOnTrawl = 5;
    int maxOnTrawl = 15;

    [SerializeField] float spacingDelta;

    [SerializeField] float depth = 100;

    protected override bool Initialize(){
        routeWire();
        return base.Initialize();
        
    }

    public void routeWire(){
        Wire wire = gameObject.AddComponent<Wire>();
        WireRenderer renderer = gameObject.AddComponent<WireRenderer>();

        renderer.Material = wireMat;
        
        wire.Diameter = .1f;
        
        WireRouteNode buoyRouteNode = WireRouteNode.Create(Wire.NodeType.BodyFixedNode,buoy,Vector3.up * -2.12f, Quaternion.identity);
        wire.Route.Add(buoyRouteNode);


        int randT = Random.Range(0,1);
        int numTraps = Mathf.RoundToInt(Mathf.Lerp(minOnTrawl,maxOnTrawl, randT));

        for(int i = 0; i < numTraps; i++){
            float deltaOffset = spacingDelta * i;
            
            Vector3 offset = new Vector3(deltaOffset,-depth,0);
            Vector3 position = offset + buoy.transform.position;

            GameObject trap = Instantiate(lobsterTrap,position,Quaternion.identity,gameObject.transform);

            Wire.NodeType nodeType = i == numTraps - 1 ? Wire.NodeType.BodyFixedNode : Wire.NodeType.EyeNode;

            WireRouteNode trapRouteNode = WireRouteNode.Create(nodeType,trap,Vector3.zero, Quaternion.identity);
            wire.Route.Add(trapRouteNode);

        }
        GetSimulation().add(wire.Native);


        Debug.Log(wire.Route.NumNodes);
        
    }

}
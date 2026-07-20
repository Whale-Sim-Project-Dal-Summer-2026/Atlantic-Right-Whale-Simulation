using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using AGXUnity;
using AGXUnity.Utils;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;


// creates a border around the created meshs using agx box meshs
public class ChunkBorderCreator : ScriptComponent {
    [SerializeField] GameObject meshParent;

    [SerializeField] GameObject wallParent;
    [SerializeField] ProcessingSettings processingSettings;

    int chunkSize;
    
    List<MeshFilter> currMeshs;

    List<chunkBorderInfo> chunkWalls;

    List<wallInfo> finalWalls;

    List<Vector2> chunkPositions;

    int wallThickness = 10;
    int wallHeight = 10000;

    [SerializeField] bool createWalls;


    protected override bool Initialize(){

        chunkSize = (int) processingSettings.chunkSize;
        chunkWalls = new List<chunkBorderInfo>();
        chunkPositions = new List<Vector2>();
        finalWalls = new List<wallInfo>();

        createBorder();

        return base.Initialize();



    }


    void createBorder(){
        grabCurrMeshs();
        createAllMeshWalls();
        createFinalWallList();
        createAGXWallBorders();
    }

    void Update(){
        if (createWalls)
        {
            createWalls = false;
            createBorder();


        }
    }

    void grabCurrMeshs(){
        MeshFilter[] meshFilters = meshParent.GetComponentsInChildren<MeshFilter>();
        currMeshs = meshFilters.ToList();
    }


// create the chunkBorderInfos for each Mesh, ignoring chunk neighboring
    void createAllMeshWalls(){
        chunkWalls.Capacity = currMeshs.Count;

        foreach(MeshFilter mf in currMeshs){
            Bounds chunkBounds = mf.mesh.bounds;

            (int west, int north) = parseChunkName(mf);

            chunkBorderInfo info = createChunkWalls(west, north);


            Vector2 chunkPos = new Vector2(west, north);

            // cache position
            chunkPositions.Add(chunkPos);

            info.west = west;
            info.north = north;

            chunkWalls.Add(info);
        }
    }
// west, north
    (int, int) parseChunkName(MeshFilter mf){
        
        string name = mf.gameObject.name;

        string[] splitBy_ = name.Split("_");

        string westStr = splitBy_[1].Split("W")[1];
        int west = int.Parse(westStr);

        string northStr = splitBy_[2].Split("N")[1];
        int north = int.Parse(northStr);

        return (west, north);
    }


    chunkBorderInfo createChunkWalls(int west, int north){

        Vector2 z = new Vector2(west, west + chunkSize);
        Vector2 x = new Vector2(north, north + chunkSize);


        wallInfo top = new wallInfo();

        top.leftPoint = new Vector2(x.x,z.y);
        top.rightPoint = new Vector2(x.y,z.y);


        wallInfo bot = new wallInfo();


        bot.leftPoint = new Vector2(x.x,z.x);
        bot.rightPoint = new Vector2(x.y,z.x);

        wallInfo left = new wallInfo();

        left.leftPoint = new Vector2(x.x,z.x);
        left.rightPoint = new Vector2(x.x,z.y);

        wallInfo right = new wallInfo();

        right.leftPoint = new Vector2(x.y,z.x);
        right.rightPoint = new Vector2(x.y,z.y);


        chunkBorderInfo chunkBorderInfo = new chunkBorderInfo();

        chunkBorderInfo.topWall = top;
        chunkBorderInfo.botWall = bot;
        chunkBorderInfo.leftWall = left;
        chunkBorderInfo.rightWall = right;



        return chunkBorderInfo;
    }

// loop over each mesh walls check for mesh neighbors
    void createFinalWallList(){
        foreach(chunkBorderInfo chunkBorderInfo in chunkWalls){
            
            int currWest = chunkBorderInfo.west;
            int currNorth = chunkBorderInfo.north;

            Vector2 northNeighbor = new Vector2(currWest, currNorth - chunkSize);
            Vector2 southNeighbor = new Vector2(currWest, currNorth + chunkSize);

            Vector2 westNeighbor = new Vector2(currWest - chunkSize, currNorth);
            Vector2 eastNeighbor = new Vector2(currWest + chunkSize, currNorth);


            if(!neighborExists(chunkBorderInfo, northNeighbor)){
                finalWalls.Add(chunkBorderInfo.topWall);
            }

            if(!neighborExists(chunkBorderInfo, southNeighbor)){
                finalWalls.Add(chunkBorderInfo.botWall);
            }

            if(!neighborExists(chunkBorderInfo, westNeighbor)){
                finalWalls.Add(chunkBorderInfo.leftWall);
            }

            if(!neighborExists(chunkBorderInfo, eastNeighbor)){
                finalWalls.Add(chunkBorderInfo.rightWall);
            }
        }
    }

    bool neighborExists(chunkBorderInfo currChunk, Vector2 neighborPos){
        foreach(Vector2 pos in chunkPositions){
            if(pos == neighborPos) return true;
        }
        return false;
    }


    
    void createAGXWallBorders(){
        foreach(wallInfo wall in finalWalls){


            GameObject wallGO = new GameObject();
            wallParent.AddChild(wallGO);

            AGXUnity.RigidBody rb = wallGO.AddComponent<AGXUnity.RigidBody>().GetInitialized<AGXUnity.RigidBody>();

            rb.Native.setMotionControl(agx.RigidBody.MotionControl.STATIC);

            AGXUnity.Collide.Box box = wallGO.AddComponent<AGXUnity.Collide.Box>();

            Vector2 p1 = wall.leftPoint;
            Vector2 p2 = wall.rightPoint;

            int x,z;
            if(p1.x == p2.x){
                // vertical wall
                x = wallThickness;
                z = (int) Mathf.Abs(p1.y - p2.y);
            }
            else if(p1.y == p2.y){
                // horizontal Wall
                x = (int) Mathf.Abs(p1.x - p2.x);
                z = wallThickness;
            }
            else{
                // wtf is this wall
                Debug.LogWarning("Chunk Wall Is Not Vertical Or Horizontal");
                continue;
            }

            Vector2 rbPos = Vector2.Lerp(p1,p2,.5f);
            Vector3 depthPos = new Vector3(rbPos.x,0,rbPos.y);

            rb.Native.setPosition(depthPos.ToHandedVec3());

            Vector3 boxHalfExtents = new Vector3(x / 2, wallHeight / 2, z / 2);

            box.HalfExtents = boxHalfExtents;
            
            agxCollide.Geometry geo = new agxCollide.Geometry(box.NativeShape);

            rb.Native.add(geo);

            GetSimulation().add(rb.Native);
        }
    }
}



// contains the infomration to create the 4 walls around a mesh
struct chunkBorderInfo{
    public wallInfo topWall;
    public wallInfo botWall;
    public wallInfo leftWall;
    public wallInfo rightWall;

    public int west;
    public int north;

}

    // these contains the min/max x and z components for the wall creation
struct wallInfo{
    public Vector2 leftPoint;
    public Vector2 rightPoint;
}
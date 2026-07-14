# NARW Simulation Documentation

This markdown file contains documentation on Dany's work on the NARW summer 26 project. This will include information on both the systems I have engineered, as well as systems that I have used and worked with, such as Algoryx (AGX).

Information contained will include design ideas, as well as how to operate the system.

**Last Update: 07/06/26**

## Document Layout

This document will be comprised into 2 major sections. Those of which being 

1. Systems I created and how they operate, and why they are made the way they are.
2. Systems I have **not** created but had to work with. (mainly AGX)


## My Systems

### Organization / Layout

In terms of my pipeline, there are 2 main components:

1. **Bathymetry**

    "Bathymetry is the study of underwater depth of ocean floors (seabed topography), river floors, or lake floors. In other words, bathymetry is the underwater equivalent to hypsometry or topography." (Wikipedia)

2. **Backscatter**
   
    "In physics, backscatter (or backscattering) is the reflection of waves, particles, or signals back to the direction from which they came."

Each of these 2 Main Components is then subdivided into 2 tasks that the pipeline handles, I like to call these the **Preprocess** and **Runtime** parts:

1. **Preprocess:** 
   
   Reading in real data in the form of a geoTIFF, applying some preprocessing, and store the preprocessed data into a binary file for the next step of the pipeline to make use of.
2. **Runtime** 
   
   Reading in the Binary File that the previous section, read it in and apply it to the runtime environment.


The main reason for needing 2 different sections is due to the fact of having expensive operations that can take several minutes. Rather than processing each time, we can preprocess once and cache the result. **NOTE** Runtime operations, like the name implies should be ran in realtime. If any operation in the runtime section is too slow, it should be moved to the preprocess step. 


### Bathymetry Preprocess Step

The Bathymetry Preprocess step contains 3 steps, those being 

1. **Reading:** Read in geoTIFF data from the CHS dataset (https://data.chs-shc.ca/dashboard/map). Each chunk of bathymetry is converted to a ```DepthDataRecord```, The definition can be found in ```./Assets/Scripts/Bathymetry/DepthDataRecord.cs```. This contains a reference to a ```GeoTiffData```, which can be found at ```./Assets/Scripts/Util/GeoTiffData.cs```. This contains all necessary and important information from reading in the geoTiff. At this stage we also determine the chunk positions relative to each other. This is done in the following manner:
    1. Find the minimum X and Z coordinate of the chunks. 
    2. Use the minimum X and Z as an origin point. 
   
2. **Patching:** Real data is messy. Real data is not defined everywhere. This is the key motivation for this section of the pipeline. There are many points in the Bathymetry that are undefined, and if not processed, would break the plausiblity of the environment, as they lead in spikes in the data. How the pipeline handles missing data points is an algorithm called Inverse Distance Weighting (IDW).
    Essentially, we take the nearest X (typically 4 is chosen) known points and take a weighted average of them. We use $1 / dP$, where $dP$  is the distance to the unknown point. 
    - Note that instead of adding the exact depth at a certain point, the system adds multiple octaves of perlin noise to break up regularity. To ensure that our new interpolated point still goes through our control points (the known points), we mask the noise based on the distance to a known point. If we are within X units of the known point, do not add any noise, enforcing control point crossover, and ensuring a smooth continuous surface.
3. **Writing:** After Reading and Patching, we can then write the processed data into a binary (.bytes) file for runtime usage. The binary file is organized like this:    
   
    | Field Name | Data Type | Size (Bytes) |
    | :--- | :--- | :--- |
    | Width | `float` | 4 |
    | Height | `float` | 4 |
    | chunkPosition X | `float` | 4 |
    | chunkPosition Y | `float` | 4 |
    | chunkGlobalStartPosition X | `float` | 4 |
    | chunkGlobalStartPosition Y | `float` | 4 |
    | pixelScale X | `double` | 8 |
    | pixelScale Y | `double` | 8 |
    | dataCount | `int` | 4 |
    | dataPoints | `List<float>` | 4 * dataCount |

### Bathymetry Runtime Step

The Bathymetry runtime step involves the construction of a mesh and displaying chunks of data. IT is relatively simple and follows a conventional data process for mesh construction.

This step as mentioned previously will use the data from the process step to display. 

First, we read in the binary file containing the preprocessed data. The organization of this binary file can be found in the previous section. These Binary files can and should be represent using the ```DepthDataRecord``` struct

Then for each DepthDataRecord, we first construct a unity mesh. Then we position it using the relative chunks position, as well as the chunk size which is defined in the ScriptableObject ```./Assets/Scripts/ProcessingSettings.cs```. We then parent this game object to a set Gameobject parent.


### Backscatter Preprocess Step

This is where things get complicated, as this is where I faced some roadblocks. 


Generally, the backscatter (BS) pipeline contains 5 steps.

Those being:

1. **Reading:**
2. **Simple Preprocessing**
3. **Cropping**
4. **Projecting**
5. **Writing to Binary**


#### Reading

Reading involves reading in the BS data from the CHS dataset and converting to a ```List<GeoTiffData>```. The reading is comprised of 2 main files that need to be read in.
1. GeoTiff file
2. JSON file

The GeoTiff data contains the raw Backscatter intensity values, however it is missing some key data that is needed for the full picture, such as min and max of the intensity values. This is needed to be passed along to the tiff reader, as it assumes a range of values to clip/enforce values to stay within. 

#### Simple Preprocessing

After the files are read in and processed into a ```List<GeoTiffData>```, we only grab the first file. (THIS WILL BE CHANGED). The reason for this was to simplify the processing. The Bay of Fundy (where the backscatter originated from, was one massive backscatter file, so assuming one file, while not good practice, was feasible for the data, and simplified the preprocessing).

In this master file, we normalize the values from the range defined in the JSON file, to a 0-1 range. This step ensures the data is easier to work with mapping directly to intensity values, where 1.0 represents the max and 0.0 represents the minimum, no matter what data is processed.

#### Cropping 

Before continuing, we should understand the key motivation and purpose of the next two steps. The key motivation for this step, while it is to reduce the computation time, the key insight is that the Bathymetry data is in Geographical Latitude and Longitude, and the backscatter is in UTM, two different coordinate spaces. We need a way to spatially relate these two datasets. They need to be brought into the same coordinate space. This can be done using projections. we should also note that projecting is an **expensive** operation, especially on large datasets. This will be gone into more detail in the next section, **Projection**.

Now armed with information of why we need to project datasets, and that it is computationally expensive, we should then decide which coordinate space we should unify both datasets into. After some initial thoughts and experimentation, it made more sense to normalize the backscatter into geographical latitude and longitude. 

**Why?**

While this may seem counter intuitive, why would I pick to normalize into geographical coordinates rather than use UTM. After all, UTM measures in **METERS**, and lat/long is in degrees. Surely working with meters would be easier than working with degrees, especially since we are using the Unity engine, where 1 unit = 1 meter. This too was my initial thought process. So I commenced converting the Bathymetry data into UTM, and this is where complications and my assumptions from earlier in the project came back around. Earlier in the project when handling the Bathymetry files, I noted that the chunks of bathymetry are 10KM^2 of data, and each file is 10 "arbitrary units" apart. For example, a file would be named 4510N06500W, and the file next to it would be 4510N06510W, note how the files are 10 units W apart. This way I could map the bathymetry data onto a flat surface. Later on, I came to learn that these arbitrary units were degrees of Latitude and Longitude, so my assumption was **warping** the space, especially stretching the space east, west wise, using a projection called the Equirectangular projection. When converting into UTM, now I was using a projection that is designed to cause the least local warping possible. However instead of the warping the space by stretching east west, it would reduce this effect, causing chunks to be not evenly sized chunks.  so now each chunk, rather than being arbitrarily placed 10KM apart, they were uniquely unevenly spaced, respecting the curvature of the Earth. Some chunks were 7.8KM apart, others 7.9. At this point, I decided that converting to UTM was **feasible**, but it would be like running into a brick wall. If I wanted to continue down this path, I would likely need to **redesign** my entire system from the ground up, to account for uneven chunk sizes. After giving that some thought as it would increase accuracy of the environment, it proved to be complicated. Hence, the decision was taken, I will continue to make my assumption of .1 Degree of Lat/Long = 10KM, and I will convert backscatter into Lat/Long.

**Back to the pipeline**

After the 0-1 Normalization is complete, we begin processing and chunking the backscatter data. As mentioned above, the Bay of Fundy BS data from the CHS dataset is one massive dataset. To reduce the computation time, one realization and key finding needed is that instead of preprocessing the entire dataset, only preprocess the parts that are needed, so what is needed, what parts of this entire file do I need? 

I need only the parts that I will be made into mesh chunks. So, this first step of chunking, is grabbing the bathymetry data and projecting them into UTM.
Then from here, I am able to determine where in the massive master BS array each chunk would be using the resolution of the data (spatial distance between points). 

I then only save these cropped BS chunks. Which could then be projected. 

Overall instead of projecting roughly ~30 x 30 Chunks of data, we only process the number of chunks wanted, typically only around 5. (However because the clipping requires projecting into UTM space, it is really 2X Projections, where X is the number of chunks we want to show) Which is much less than ~900. 

#### Projection

This is the final preprocessing step for the Backscatter pipeline (excluding the writing to a binary file, as that is trivial).

Projecting from one coordinate space into another is a complex and expensive operation. 

For our purposes, we need to handle **UTM** <=> **Geographical Lat/Long**. As well as being able to project, we should be able to **upsample** as we project to maintain a uniform resolution across datasets.

Projection comes with 2 distinct steps. A forward and backward pass. The reason for having 2 distinct steps is to ensure points defined uniformly across a grid, effectivly combating projection warping on individual points. For example, say you have a list of data points in lat/long which are spaced 10 meters apart (If this doesn't make sense, read the why section in the Cropping step). when Projecting, we would like to keep points to be evenly spaced apart. However, when projecting, points do not end up exactly in the sapce spot, this is due to warping. We first do a **forward pass**, where we convert all of our known points into the new coordinate space, ignoring the warping effect. Then afterwards, we do a **backward pass**. In this backward pass, instead of looping over each point that was just projected, we loop over each **TARGET** point. The target points are the uniform spaced points that we define based on the resolution. foreach target point, we interpolate the nearest X neighbors together, essentially taking an average for our target point. It is worth noting, that we upsample in the backward pass, based on the target resolution.

We then have our chunked, upsampled, projected backscatter data (what a mouthful) that is ready to be writing into a binary file. 


#### Writing To Binary

The process of writing to a binary file is trivial. It follows the following structure:

| Field Name | Data Type | Size (Bytes) |
| :--- | :--- | :--- |
| Width | `float` | 4 |
| Height | `float` | 4 |
| dataCount | `int` | 4 |
| dataPoints | `List<float
The backscatter runtime step includes the mapping of a flat mud texture to the bathymetry driven mesh, then the inclusion of procededurally generated boulders. While this approach sacrifices some bathymetry realism, we can achieve greater plausibility. 

Generally the process follows these steps:

1. Read in Processed Backscatter
2. Read in current meshes data
3. Sample some Pseudorandom numbers to lerp between some defined properties.

Step 1 and 2 are relatively trivial as it consits of reading in the processed work that we have completed before. The layout of backscatter binary file can be found in the section above.


#### Pseudorandom Properties

This section is not too difficult/complex to understand, however it is important to have a record of. 

Moving on to the system.

First, we define a min and max scale, and min and max number of boulders.

After having read in the backscatter, we take the average of the entire chunk's bathymetry. The reason we do this is for a few reasons, but the main being for simplicity sake. As I had observed within the data, the Backscatter values do not tend to deviate much from the average, and so taking an average of the entire chunk would not destroy much detail, while simplifying further usage. 

We then use this average to lerp between the bottom number of clumps and the top. The idea is that higher backscatter values means rouger terrain, which can equate to more boulders, or more clumps of boulders.

To destory the self similiarity bgetween boulders, we sample some pseduo random numbers that drive the boulder positioning and scale. 

We first decide where to place a clump of boulders. Then after which, we decide how many boulders should be in this clump, this is completly random, however we pick between a min and max. Then for each boulder, we sample a random number for the scale, and for the deviation from the clump spawn point. These numbers are all exposed to the user to allow for greater control of the system. 

We should note that for position, we use the chunks bounds to determine where it should be placed, to ensure the boulders stay within the chunks space. This enforcemnet of position is done after having decided where a boulder should be placed with some deviation. 

**Procederaul Boulders** 

How are the boulders procederally generated? 

They are created in a shadergraph, Unity's visual shader creation tool. I find these tools quite easy to work with, and having a visual output as you move through the creation is quite handy.

The goal is destroy the self similiarity between different rocks. 

The generation consists of 2 steps:
* Vertex Displacement
* Color Picking


**Vertex Displacement**

I use perlin ridge noise. More can be found here: https://thebookofshaders.com/13/ (note, this website is very cool!, and I would recommend checking it out!)

The idea of using this type of noise is to introduce the ridges and roughness that boulders typically have due to erosion. We displace the vertices using this. 


**Color Picking**

We define a target color. Say around a grey. Then for each boulder, we should deviate slightly from this defined color. We can pick a (Pseudo)random unit vector in 3D space. Then we can allow this vector to be scaled by a certain small value. From there, we can add this small deviation into the color, producing a color that is plausible, but slightly differnt boulder to boulder.


### Miscealous

In this section, I will conver all miscelanous files, scripts, or other bits of information that would be useful to understand the systems I have built. 

This section will be covered in the following manner:

Each file/script/component will have its own header which will be titled with its path relative to the scripts directory (in the Assets directory). For example ```Backscatter/BackscatterReader.cs```

#### Scene Util Direcotry

In the scene Util Directory, as the name implies, there are utilities that handle and work with Unity Scenes. This includes the following files:

* ```MeshWaterPosition.cs```
* ```PrefabGridInstaniate.cs```
* ```RopeActivator.cs```
* ```SceneSwitcher.cs```
* ```TrawlLineSpawner.cs```

**MeshWaterPosition.cs**
<br>
This file positions water boxes, which is a prefab, according to the chunks of data that is being renderered. It registers these water chunks with the AGX WindAndWaterManager. Note, that the water chunks can be reloaded when setting the boolean variable ```spawnWater``` to ```true```. 


**PrefabGridInstaniate.cs**
<br>
This file is used for creating many prefabs in a grid. So say for example, if you would like to spawn in 10 ropes in a grid with equal spacing, this file would be a good fit. 

Note that many of the variables and function names in this class are named around ropes. This is due to when first creating the class, only forseeing its use case for spawning ropes, however it has proven useful on numerous occasions. 


**RopeActivator.cs**
<br>
This file is simply an object toggler. If you press P, it will activate and deactive and a chosen object. 

This file like the previous is named around ropes as that is what I had forseen it being used for, however it is likely this could be useful for other purposes.


**SceneSwitcher.cs**
<br>
This file can be used for switching between a list of scenes. This was mainly used for the demo. 

Note that, the list of scenes is of the type ```string```. Where the string should be the NAME of the scene itself.


Also note that the scenes must be registered with the unity BUILD. This can be achieved in the following manner:

file >> Build Profiles >> Open Scene List >> Add Open Scenes. 

For this to work, the scenes must be open in the hierarchy. 


**TrawlLineSpawner.cs**
<br>
This file is used for the spawning of trawl lines, like the name sugests. This handles the dyanamic creation of AGX ropes in between the buoy, and traps.

We are able to control the minimum and maximum numebr of trpas spawned per trawl, as well as, the spacing, and depth (or y position) of the trawl line. 

The script randomly picks a value between the min and max to spawn for the number of traps on a trawl line. 

This script will eventually be expanded in to spawn numerous trawl lines across a certain given chunk(s). 

#### Noise.cs

the noise class allows for the inclusion/addition of noise into either depth data, or into other use cases. 

In the case of adding it to depth data (```addNoiseToDepth()```), the noise class will also mask/not add noise to points that are too close to control points. 

The control points in this case are the points that are defined in our height map. Therefore, this function must be ran after KNN, or after some kind of distance search. 

Alternatively, fBm coupled with perlin noise can be called, using 
```fBmNoise()```

## Systems I have worked with

This section will act as a reference for all the knowledge I have on different systems, how they operate, how to use them, and other importnat infomration relating to the system.


### Algoryx for Unity (AGX)

Algoryx is the physics engine driving the simulation. AGX appears to be more predictable than Obi (our previous system). Once everything is setup properly, the system works. However sometimes the system, especially when under heavy load, in particular with many contact points with a rope, can become unstable. 

#### Important Links
Here are some important links for learning, which is where I got most of my information from (alongside reading source code).

Algoryx for Unity Documentation: https://us.download.algoryx.se/AGXUnity/documentation/current/index.html
<br>
Algoryx Developer Guide: ```./Assets/AGXUnity/DeveloperGuide.md``` 


#### General AGX System Architecture

AGX, in our case is being used as a plugin for Unity. It contains different components that can be attached to game objects. It appears that once an AGX component interacts with a game object, that game object gets locked off from runtime changes that does not go through their system.

This is because AGX keeps a track of all gameobjects separately into a "**native**"  system. 

From what I have seen there are 2 sides to AGX scripting. The term refers to any scripting where you are interacting or handling AGX objects. 

1. AGXUnity.XYZ
2. agx.XYZ

I have found this confusing. There are typically 2 copies of for example a Mesh, AGXUnity.Collide.Mesh, and agxCollide.Mesh. After diving into the source for both it appears that the distinction is the following:

anything with AGXUnity, interacts with unity side things, in the case of mesh, AGXUnity will create a mesh from a Unity Mesh. On the other hand, anything that does not contain **Unity**, namely **agx**, rather interacts with some precompiled DLL file. These agx namepsace files are decompiled using Swig. AGX hides away the key logic for most of their physics behind precompiled DLL Files that were turned into DLL from their C++ main engine. It appears they used a tool called **Swig** to complete this task https://github.com/swig/swig.


#### Wires Vs Cables 

Four our purposes of creation of fishing lines, either wires or cables are plausible choices. After  some intial testing of differences between the two, they apepar to work very similiarly. I noticed some performance gains in using wires rather than cables.

The documentation also states that it is possible to cut and merge different wires during runtime, which could be help drive realism in the simulation, as fishing gear may break under heavy tension.

The documnetation also states that cables have a fixed resoultion vs wires have dynamic resolution. In this case resolution is referring to how many segmnents are in the rope. Having dyanmic resolution allows for points that could allow for entanglement, where cables might struggle in a similar situation. 

However cables allow for modeling of torsion and plasticity that wires do not. I do not believe that these would be useful for our simulation. 

More can be found here: "https://www.algoryx.se/documentation/complete/agx/tags/latest/doc/UserManual/source/agxcable.html"


#### Solver Settings

I have tested numerous different solver settings. The 2 goals that drove the experimentation was

1. Realtime Playback
2. Stable Simulation

A sub goal of keeping memory usage as minimal as possible was kept in mind as well.

The most important discovery in the solver settings is the setting **Real Time Rendering**.

When set to 1, the solver, when under heavy simulation, sacrifices FPS in a traditional sense, lowering the FPS.
When set to 0, the solver when under heavy simulation, slows down the simulation, attempting to keep FPS higher.

I have found that setting this parameter to 0 allows us to keep realtime playback. Without it, its very possible to have 1-2 FPS under heavy load.

Another important note is number of threads. I have noiticed that changing this value did not effect performance at all. Unsure if it is a bug, but either way, I kept this on 4, as this is noted as the max in the documentation.  


#### HydroDyanamics (Water)

Creation and Management of hydrodynamics has proven to be quite easy. The main things to keep in mind is that the hydrodynamics expects all objects that are water to be udner the same object. As well as that, Density is the driving force for how buoyant or not an object is. This can be found within the shape material. (Less Dense Objects float more)

#### Current Development Struggles

As the above has mentioned, working with AGX has been quite tricky. The documentation on the Unity side of the system is sparse. And while looking through the source code, we have to dig through source that has multiple defintions (for example, multiple meshs), it is easy to find functions that sound like they would work, but after giving them a try, it does not work. 

As well as that, due to the fact of a section of the code being decompiled, it is harder to read and make out the purpose, and often times is hard to call certain functions due to parameters that are unclear. 

I have found that AGX struggles heavily with collisions with planes. Our mesh collider for the whale uses a trimesh (triangle mesh). When interacting with many wires, often the wires will clip through the model. The solver will then overcompensate with large forces, shooting the whale off. Often in these cases, performance of the simulation severely degrades, in terms of FPS and Memory. These situations MUST be avoided at all costs. As when this happens it is possible for agx to use over 30GBs of RAM, and to drop the fps from over 60 to less than 1, and in some cases crash. 



## References

https://en.wikipedia.org/wiki/Bathymetry
https://en.wikipedia.org/wiki/Backscatter
https://en.wikipedia.org/wiki/Equirectangular_projection
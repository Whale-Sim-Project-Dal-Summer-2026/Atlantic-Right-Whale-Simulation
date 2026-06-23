
using DataSources; 
using Unity;
using System;


//-- ROADMAP NOTES--


// ARCH
//1. animation is preprocessed from data source into a series of whale states
//2. then the animation or list of whale states are saved into a binary file
//3. then the binary file is loaded using a background thread in large chunks and fed into the driver
//4. the driver then applies the current whale state to the whale model in the scene
//5. once the chunk is nearly finished, the next chunk is loaded in the background and fed into the driver
//6. repeat until the end of the animation is reached



// to make the timestep scrubbing work properly, the driver needs to be able to get the whale state at a specific timestep, 
// and also get the next whale state from the current timestep. the chunk loading should be able to do those tasks.

// FIXED TIMESTEP OF 0.02 

// EVERY 5 FIXED UPDATES A NEW FRAME OR SNAPCHOT OF THE WHALE IS SWITCHED TO (lerp between the two states for the 5 fixed updates).
// count the updates and every 5 switch frames 

// chunkloading for a time step will work by dividing the current time by 0.1 and getting the floor to then get the index of the frame in the file
// then dividne by chunk size to get which chunk its in
//then modulo to get the offset inside the chunk and then load the chunk and get the frame at the offset. 
// then contiune the animation
// 

// all logic will be the same so there can be an AGX based dirver and a pure unity based driver

//SAME THING WITH THE MULTIPLE DATA SOURCES!!!

public class WhaleDriver
{
    DataSource dataSource; 
    
}
using AnimationDataStructs;
using System;
using System.IO;


namespace AnimationDataStorageManager {
    public class DataStorageManager{

    // Fixed counts for each section to offset the data correctly when writing
    int bodyLength_Count;
    int mouth_Count;
    int leftFin_Count;
    int rightFin_count; 
    int root_Count;
    int head_Count;
    int tailStartIndex;
  

    //byte sizes for getting offsets
    const int NUM_BYTES_LOCALROTATION = 4 * sizeof(float);
    const int NUM_BYTES_GLOBALANIMATION = 8 * sizeof(float);

    

    public DataStorageManager(WhaleBlueprint blueprint) {
        // init from the blueprint
        bodyLength_Count = blueprint.BodyLengthCount;
        mouth_Count = blueprint.MouthCount;
        leftFin_Count = blueprint.LeftFinCount;
        rightFin_count = blueprint.RightFinCount;
        root_Count = blueprint.RootCount;
        head_Count = blueprint.HeadCount;
    }

    public void SaveWhaleAnimationData(WhaleState[] whaleStates, string filePath) {
        // Write the whale animation data to a binary file
        using (FileStream stream = new FileStream(filePath, FileMode.Create))
        {
            using (BinaryWriter writer = new BinaryWriter(stream)) {
                
                // Write total number of frames (could be used to check if the file is correct?)
                writer.Write((int)whaleStates.Length); 

                // go through each whale state and write the data
                for (int i =0; i < whaleStates.Length; i++) {
                    WhaleState state = whaleStates[i];
                
                    if (state.BodyLength.Length > 0) WriteLocalRotationList(writer, state.BodyLength);
                    if (state.Mouth.Length > 0) WriteLocalRotationList(writer, state.Mouth);
                    if (state.LeftFin.Length > 0) WriteLocalRotationList(writer, state.LeftFin);
                    if (state.RightFin.Length > 0) WriteLocalRotationList(writer, state.RightFin);
                    if (head_Count > 0) WriteLocalRotation(writer, state.Head);
                    WriteBodyBlock(writer, state.Root);
                    

                }
            }
        }
    }
    public WhaleState[] LoadAllWhaleAnimationData(string filePath) {
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                using (BinaryReader reader = new BinaryReader(stream)) {
                    
                    int totalFrames = reader.ReadInt32(); 

                    WhaleState[] states = new WhaleState[totalFrames];

                    for (int i = 0; i < totalFrames; i++) {
                    
                        WhaleState state = new WhaleState(new WhaleBlueprint(bodyLength_Count, mouth_Count, leftFin_Count, rightFin_count, root_Count, head_Count, tailStartIndex));

                        // Populate the data from the stream.
                        if (state.BodyLength.Length > 0) ReadLocalRotationList(reader, state.BodyLength);
                        if (state.Mouth.Length > 0) ReadLocalRotationList(reader, state.Mouth);
                        if (state.LeftFin.Length > 0) ReadLocalRotationList(reader, state.LeftFin);
                        if (state.RightFin.Length > 0) ReadLocalRotationList(reader, state.RightFin);
                        if (head_Count > 0 ) state.Head = ReadLocalRotation(reader, state.Head);
                        ReadBodyBlock(reader, state.Root);
                        

                        states[i] = state;
                    }

                    return states;
                }
            }
        }
    public WhaleState LoadWhaleStateAtIndex(string filePath, int frameIndex) {
        using (FileStream stream = new FileStream(filePath, FileMode.Open)){
            using (BinaryReader reader = new BinaryReader(stream)) {

                int totalFrames = reader.ReadInt32();

                if (frameIndex < 0 || frameIndex >= totalFrames)
                    throw new ArgumentOutOfRangeException(nameof(frameIndex),
                    $"Frame {frameIndex} is out of range \nFile has {totalFrames} frames.");

                // Gets number of bytes for each local rotation sections
                int bytesForLocalRotation = (bodyLength_Count + mouth_Count + leftFin_Count + rightFin_count+ head_Count) * NUM_BYTES_LOCALROTATION;
                // gets number of bytes for the main body section
                int bytesForGlobalAnimation = root_Count  * NUM_BYTES_GLOBALANIMATION;
                // combine to get total bytes per frame
                int bytesPerFrame = bytesForLocalRotation + bytesForGlobalAnimation;
                // skip the frame count header then jump ahead frameIndex frames
                long offset = sizeof(int) + (long)frameIndex * bytesPerFrame;
                stream.Seek(offset, SeekOrigin.Begin);

                WhaleState state = new WhaleState(new WhaleBlueprint(bodyLength_Count, mouth_Count, leftFin_Count, rightFin_count, root_Count, head_Count, tailStartIndex));

                if (state.BodyLength.Length > 0) ReadLocalRotationList(reader, state.BodyLength);
                if (state.Mouth.Length > 0) ReadLocalRotationList(reader, state.Mouth);
                if (state.LeftFin.Length > 0) ReadLocalRotationList(reader, state.LeftFin);
                if (state.RightFin.Length > 0) ReadLocalRotationList(reader, state.RightFin);
                if (head_Count>0) state.Head = ReadLocalRotation(reader, state.Head);
                ReadBodyBlock(reader, state.Root);
                

                return state;
            }
        }   
    }
    
    public WhaleState[] LoadWhaleStatesInRange(string filePath, int startFrame, int count) {
        using (FileStream stream = new FileStream(filePath, FileMode.Open)) {
            using (BinaryReader reader = new BinaryReader(stream)) {

                int totalFrames = reader.ReadInt32();

                // Clamp to valid range
                startFrame = Math.Max(0, Math.Min(startFrame, totalFrames - 1));
                count = Math.Min(count, totalFrames - startFrame);

                // Gets number of bytes for each local rotation sections
                int bytesForLocalRotation = (bodyLength_Count + mouth_Count + leftFin_Count + rightFin_count + head_Count) * NUM_BYTES_LOCALROTATION;
                // gets number of bytes for the main body section
                int bytesForGlobalAnimation = root_Count * NUM_BYTES_GLOBALANIMATION;
                // combine to get total bytes per frame
                int bytesPerFrame = bytesForLocalRotation + bytesForGlobalAnimation;

                // skip the frame count header then jump ahead frameIndex frames
                long offset = sizeof(int) + (long)startFrame * bytesPerFrame;
                stream.Seek(offset, SeekOrigin.Begin);

                WhaleState[] states = new WhaleState[count];
                for (int i = 0; i < count; i++) {
                   WhaleState state = new WhaleState(new WhaleBlueprint(bodyLength_Count, mouth_Count, leftFin_Count, rightFin_count, root_Count, head_Count, tailStartIndex));


                    if (state.BodyLength.Length > 0) ReadLocalRotationList(reader, state.BodyLength);
                    if (state.Mouth.Length > 0) ReadLocalRotationList(reader, state.Mouth);
                    if (state.LeftFin.Length > 0) ReadLocalRotationList(reader, state.LeftFin);
                    if (state.RightFin.Length > 0) ReadLocalRotationList(reader, state.RightFin);
                    if (head_Count >0) state.Head = ReadLocalRotation(reader, state.Head);
                    state.Root = ReadBodyBlock(reader, state.Root);


                    states[i] = state;
                }

                return states;
            }   
        }
    }
    
    private void ReadLocalRotationList(BinaryReader reader, LocalRotation_AnimationData[] data) {
            for (int i = 0; i < data.Length; i++) {
                data[i].Rotation.x = (float)reader.ReadSingle();
                data[i].Rotation.y =(float)reader.ReadSingle();
                data[i].Rotation.z = (float)reader.ReadSingle();
                data[i].Rotation.w = (float)reader.ReadSingle();
            }
        }
    private LocalRotation_AnimationData ReadLocalRotation(BinaryReader reader, LocalRotation_AnimationData data) {
            data.Rotation.x = (float)reader.ReadSingle();
            data.Rotation.y =(float)reader.ReadSingle();
            data.Rotation.z = (float)reader.ReadSingle();
            data.Rotation.w = (float)reader.ReadSingle();
            return data;
        }
    

    private Global_AnimationData ReadBodyBlock(BinaryReader reader, Global_AnimationData data) {
            data.Position.x = (float)reader.ReadSingle();
            data.Position.y = (float)reader.ReadSingle();
            data.Position.z = (float)reader.ReadSingle();
            data.Rotation.x = (float)reader.ReadSingle();
            data.Rotation.y = (float)reader.ReadSingle();
            data.Rotation.z = (float)reader.ReadSingle();
            data.Rotation.w = (float)reader.ReadSingle();
            data.Speed = (float)reader.ReadSingle();

            return data;
    }
        
    public int ReadTotalFrameCount(string filePath) {
        using (FileStream stream = new FileStream(filePath, FileMode.Open))
        using (BinaryReader reader = new BinaryReader(stream))
            return reader.ReadInt32();
    }
    private void WriteLocalRotationList(BinaryWriter writer, LocalRotation_AnimationData[] data) {
        foreach (var item in data) {
            // 
            writer.Write(item.Rotation.x); 
            writer.Write(item.Rotation.y);
            writer.Write(item.Rotation.z);
            writer.Write(item.Rotation.w);
        }
    }
    
    private void WriteLocalRotation(BinaryWriter writer, LocalRotation_AnimationData data) {
            writer.Write(data.Rotation.x); 
            writer.Write(data.Rotation.y);
            writer.Write(data.Rotation.z);
            writer.Write(data.Rotation.w);
    
    }

    private void WriteBodyBlock(BinaryWriter writer, Global_AnimationData data) {
        // write position as 3 floats (12 bytes)
        writer.Write(data.Position.x);
        writer.Write(data.Position.y);
        writer.Write(data.Position.z);
        // write rotadata as quaternion (4 floats, 16 bytes)
        writer.Write(data.Rotation.x);
        writer.Write(data.Rotation.y);
        writer.Write(data.Rotation.z);
        writer.Write(data.Rotation.w);
        writer.Write(data.Speed);
        
    }
    
    
}

}
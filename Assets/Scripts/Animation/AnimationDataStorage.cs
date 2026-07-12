using AnimationDataStructs;
using System;
using System.IO;


namespace AnimationDataStorageManager {
    public class DataStorageManager{

    // Fixed counts for each section to offset the data correctly when writing
    int tail_Count;
    int mouth_Count;
    int leftFin_Count;
    int rightFin_count; 
    int mainBody_Count;

    //byte sizes for getting offsets
    const int NUM_BYTES_LOCALROTATION = 4 * sizeof(float);
    const int NUM_BYTES_GLOBALANIMATION = 8 * sizeof(float);

    

    public DataStorageManager(WhaleBlueprint blueprint) {
        // init from the blueprint
        tail_Count = blueprint.TailCount;
        mouth_Count = blueprint.MouthCount;
        leftFin_Count = blueprint.LeftFinCount;
        rightFin_count = blueprint.RightFinCount;
        mainBody_Count = blueprint.MainBodyCount;
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
                
                    if (state.Tail.Length > 0) WriteLocalRotation(writer, state.Tail);
                    if (state.Mouth.Length > 0) WriteLocalRotation(writer, state.Mouth);
                    if (state.LeftFin.Length > 0) WriteLocalRotation(writer, state.LeftFin);
                    if (state.RightFin.Length > 0) WriteLocalRotation(writer, state.RightFin);
                    WriteBodyBlock(writer, state.MainBody);

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
                    
                        WhaleState state = new WhaleState(new WhaleBlueprint(tail_Count, mouth_Count, leftFin_Count, rightFin_count, mainBody_Count));

                        // Populate the data from the stream.
                        if (state.Tail.Length > 0) ReadLocalRotation(reader, state.Tail);
                        if (state.Mouth.Length > 0) ReadLocalRotation(reader, state.Mouth);
                        if (state.LeftFin.Length > 0) ReadLocalRotation(reader, state.LeftFin);
                        if (state.RightFin.Length > 0) ReadLocalRotation(reader, state.RightFin);
                        ReadBodyBlock(reader, state.MainBody);

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
                int bytesForLocalRotation = (tail_Count + mouth_Count + leftFin_Count + rightFin_count) * NUM_BYTES_LOCALROTATION;
                // gets number of bytes for the main body section
                int bytesForGlobalAnimation = mainBody_Count * NUM_BYTES_GLOBALANIMATION;
                // combine to get total bytes per frame
                int bytesPerFrame = bytesForLocalRotation + bytesForGlobalAnimation;
                // skip the frame count header then jump ahead frameIndex frames
                long offset = sizeof(int) + (long)frameIndex * bytesPerFrame;
                stream.Seek(offset, SeekOrigin.Begin);

                WhaleState state = new WhaleState(new WhaleBlueprint(tail_Count, mouth_Count, leftFin_Count, rightFin_count, mainBody_Count));
                if (state.Tail.Length > 0) ReadLocalRotation(reader, state.Tail);
                if (state.Mouth.Length > 0) ReadLocalRotation(reader, state.Mouth);
                if (state.LeftFin.Length > 0) ReadLocalRotation(reader, state.LeftFin);
                if (state.RightFin.Length > 0) ReadLocalRotation(reader, state.RightFin);
                ReadBodyBlock(reader, state.MainBody);

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
                int bytesForLocalRotation = (tail_Count + mouth_Count + leftFin_Count + rightFin_count) * NUM_BYTES_LOCALROTATION;
                // gets number of bytes for the main body section
                int bytesForGlobalAnimation = mainBody_Count * NUM_BYTES_GLOBALANIMATION;
                // combine to get total bytes per frame
                int bytesPerFrame = bytesForLocalRotation + bytesForGlobalAnimation;

                // skip the frame count header then jump ahead frameIndex frames
                long offset = sizeof(int) + (long)startFrame * bytesPerFrame;
                stream.Seek(offset, SeekOrigin.Begin);

                WhaleState[] states = new WhaleState[count];
                for (int i = 0; i < count; i++) {
                    WhaleState state = new WhaleState(new WhaleBlueprint(tail_Count, mouth_Count, leftFin_Count, rightFin_count, mainBody_Count));

                    if (state.Tail.Length > 0) ReadLocalRotation(reader, state.Tail);
                    if (state.Mouth.Length > 0) ReadLocalRotation(reader, state.Mouth);
                    if (state.LeftFin.Length > 0) ReadLocalRotation(reader, state.LeftFin);
                    if (state.RightFin.Length > 0) ReadLocalRotation(reader, state.RightFin);
                    state.MainBody = ReadBodyBlock(reader, state.MainBody);

                    states[i] = state;
                }

                return states;
            }   
        }
    }
    
    private void ReadLocalRotation(BinaryReader reader, LocalRotation_AnimationData[] data) {
            for (int i = 0; i < data.Length; i++) {
                data[i].Rotation.x = (float)reader.ReadSingle();
                data[i].Rotation.y =(float)reader.ReadSingle();
                data[i].Rotation.z = (float)reader.ReadSingle();
                data[i].Rotation.w = (float)reader.ReadSingle();
            }
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
    private void WriteLocalRotation(BinaryWriter writer, LocalRotation_AnimationData[] data) {
        foreach (var item in data) {
            // 
            writer.Write(item.Rotation.x); 
            writer.Write(item.Rotation.y);
            writer.Write(item.Rotation.z);
            writer.Write(item.Rotation.w);
        }
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
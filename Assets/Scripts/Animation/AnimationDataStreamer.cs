using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks; 
using AnimationDataStructs;
using AnimationDataStorageManager;
using UnityEngine;

// trying to follow a producer consumer pattern for now but this might change
// this could be bad for finetune scrubbing but we will see as it gets implemented

public class WhaleAnimationStreamer{

    private readonly DataStorageManager storageManager;
    private readonly string filePath;
    private readonly int batchSize;
    private readonly int refillThreshold;

    private readonly ConcurrentQueue<WhaleState> queue = new ConcurrentQueue<WhaleState>();

    private int nextFrameIndex = 0;
    private readonly int totalFrames;

    // 0 means not loading, 1 means loading (used this its what the interlocked class uses for bools)
    private int _isLoading = 0;

    // checks if the streamer has finished loading all frames and the queue is empty
    public bool IsExhausted => nextFrameIndex >= totalFrames && queue.IsEmpty;

    // converts the int to a bool for easier checking if its loading or not
    public bool IsLoading => _isLoading == 1;

    // returns the number of whale states currently buffered in the queue
    public int Buffered => queue.Count;

    public WhaleAnimationStreamer(DataStorageManager storageIn, string filePathIn, int batchSizeIn = 500, int refillThresholdIn = 100) {
        storageManager = storageIn;
        filePath = filePathIn;
        batchSize = batchSizeIn;
        refillThreshold = refillThresholdIn;

        // Get the total number of frames in the file
        totalFrames = storageManager.ReadTotalFrameCount(filePath);

        //Load right away
        TriggerLoad(); 
    }

    // dequeue the next whale state (if it exists) and trigger a load if its running out
    public bool TryGetNextState(out WhaleState state) {
        bool success = queue.TryDequeue(out state);

        if (queue.Count < refillThreshold)
            TriggerLoad();

        return success;
    }

    public void SeekTo(int targetFrame) {

        // clear queue
        while (queue.TryDequeue(out _)) { }

        // set the next frame index to the target frame
        nextFrameIndex =  targetFrame;

        // lok the loading flag
        Interlocked.Exchange(ref _isLoading, 0);

        TriggerLoad();

    }


    private void TriggerLoad() {
        if (nextFrameIndex >= totalFrames) return;

        // uses interlocked to make sure only one load is happening at a time 
        if (Interlocked.CompareExchange(ref _isLoading, 1, 0) != 0) return;

        int startFrame = nextFrameIndex;
        int count = Math.Min(batchSize, totalFrames - startFrame);
        nextFrameIndex += count;

        // run the loading in a separate task to not block main thread
        Task.Run(() => {
            try {
                WhaleState[] batch = storageManager.LoadWhaleStatesInRange(filePath, startFrame, count);
                foreach (WhaleState s in batch) {
                    queue.Enqueue(s);
                }
            }
            catch (Exception e) {
                Debug.LogError($"Failed to load whale data: {e.Message}");
            }
            finally {
                //clear lock
                Interlocked.Exchange(ref _isLoading, 0);
            }
        });
    }

}
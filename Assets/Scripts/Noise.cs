using UnityEngine;

public class Noise
{

// this is the min distance away that noise can commence at. (closer than this distance to a known point, there is no noise)
    byte distanceMask = 2;
    
    ProcessingSettings settings;

    public Noise(ProcessingSettings settings)
    {
        this.settings = settings;
    }
    public float addNoiseToDepth(float depth, Vector2 pos, int[] size, float distance)
    {

        byte mask = (byte)(distance < distanceMask ? 0 : 1);

        if(mask == 0){
            return depth;
        }
        float frequency = settings.noiseFrequency;
        float amplitude = settings.noiseAmplitude;

        float lacunarity = 2.2f;
        float persistence = .55f;

        float x = pos.x / size[0];
        float y = pos.y / size[1];

        float accumulatedNoise = fBmNoise(x,y,frequency,amplitude,lacunarity,persistence,8);
        
        return depth + accumulatedNoise;
    }

// pos[1] / size[1]
    public float fBmNoise(float x, float y, float startFreq, float startAmp, float lacunarity, float persistence, byte numOctaves)
    {
        float accumulatedNoise = 0f;
        float frequency = startFreq;
        float amplitude = startAmp;

        for(int i = 0; i < numOctaves; i++)
        {

            float normalizedX = x * frequency;
            float normalizedY = y * frequency;
            float noise = ((Mathf.PerlinNoise(normalizedX, normalizedY) * 2) - 1) * amplitude;

            accumulatedNoise += noise;

            amplitude *= persistence;
            frequency *= lacunarity;
            
        }

        return accumulatedNoise;
    }


}
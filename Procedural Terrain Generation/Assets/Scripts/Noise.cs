using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise 
{
    public static float[,] GenerateNoiseMap(NoiseData noiseData, Vector2 offset)
    {
        float[,] noiseMap = new float[noiseData.length + 1, noiseData.length + 1];

        System.Random prng = new System.Random(noiseData.randomizerSeed);
        Vector2[] layerOffsets = new Vector2[noiseData.numOfLayers];

        float amplitude = 1;
        float frequency;
        float noiseHeight;
        float maxPossibleHeight = 0;

        for (int i = 0; i < noiseData.numOfLayers; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetZ = prng.Next(-100000, 100000) + offset.y;
            layerOffsets[i] = new Vector2(offsetX, offsetZ);

            maxPossibleHeight += amplitude;
            amplitude *= noiseData.persistence;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfLength = noiseData.length / 2;

        for (int x = 0; x <= noiseData.length; x++)
        {
            for (int z = 0; z <= noiseData.length; z++)
            {
                amplitude = 1;
                frequency = 1;
                noiseHeight = 0;

                for (int i = 0; i < noiseData.numOfLayers; i++)
                {
                    float sampleX = (x- halfLength + layerOffsets[i].x) / noiseData.scale * frequency;
                    float sampleZ = (z- halfLength + layerOffsets[i].y) / noiseData.scale * frequency;

                    float perlinVal = Mathf.PerlinNoise(sampleX, sampleZ) * 2 - 1;
                    noiseHeight += perlinVal * amplitude;

                    amplitude *= noiseData.persistence;
                    frequency *= noiseData.lacunarity;
                }

                if(noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if(noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                noiseMap[x, z] = noiseHeight;
            }
        }
        for (int x = 0; x <= noiseData.length; x++)
        {
            for (int z = 0; z <= noiseData.length; z++)
            {
                float normalizedHeight = (noiseMap[x, z] + 1) / (2f * maxPossibleHeight *1.75f);
                noiseMap[x, z] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
            }
        }
        return noiseMap;
    }
}
[System.Serializable]
public class NoiseData
{
    public int length;
    public int numOfLayers;
    public float scale;
    public AnimationCurve heightMultiplier;
    public float persistence;
    public float lacunarity;
    public int randomizerSeed;

    public NoiseData(NoiseData noiseData)
    {
        this.length = noiseData.length;
        this.numOfLayers = noiseData.numOfLayers;
        this.scale = noiseData.scale;
        this.persistence = noiseData.persistence;
        this.lacunarity = noiseData.lacunarity;
    }

}

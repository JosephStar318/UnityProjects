using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise 
{
    public static float[,] GenerateNoiseMap(NoiseData noiseData)
    {
        float[,] noiseMap = new float[noiseData.width + 1, noiseData.length + 1];

        System.Random prng = new System.Random(noiseData.randomizerSeed);
        Vector2[] layerOffsets = new Vector2[noiseData.numOfLayers];
        for (int i = 0; i < noiseData.numOfLayers; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + noiseData.offset.x;
            float offsetZ = prng.Next(-100000, 100000) + noiseData.offset.y;
            layerOffsets[i] = new Vector2(offsetX, offsetZ);
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = noiseData.width / 2;
        float halfLength = noiseData.length / 2;

        for (int x = 0; x <= noiseData.width; x++)
        {
            for (int z = 0; z <= noiseData.length; z++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < noiseData.numOfLayers; i++)
                {
                    float sampleX = (x-halfWidth + layerOffsets[i].x) / noiseData.scale * frequency;
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
        for (int x = 0; x <= noiseData.width; x++)
        {
            for (int z = 0; z <= noiseData.length; z++)
            {
                noiseMap[x, z] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, z]);
            }
        }


        return noiseMap;
    }
}
[System.Serializable]
public class NoiseData
{
    public int length;
    public int width;
    public int numOfLayers;
    public float scale;
    public AnimationCurve heightMultiplier;
    public float persistence;
    public float lacunarity;
    public int randomizerSeed;
    public Vector2 offset;

    public NoiseData(int length, int width, int numOfLayers, float scale, float persistence, float lacunarity, Vector2 offset)
    {
        this.length = length;
        this.width = width;
        this.numOfLayers = numOfLayers;
        this.scale = scale;
        this.persistence = persistence;
        this.lacunarity = lacunarity;
        this.offset = offset;
    }
}

using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using System;

public class Chunk
{
    public GameObject chunkObject;
    public Mesh mesh;
    private NoiseData _noiseData;
    private Vector3[] vertices;
    private int[] triangles;

    private float _resolution;
    private int _chunkLength;
    private int _chunkWidth;

    public static Queue<ThreadInfo<Chunk>> ThreadInfoQueue = new Queue<ThreadInfo<Chunk>>();

    public Chunk(GameObject obj, float cellSize, int chunkLength, int chunkWidth, NoiseData noiseData)
    {
        _resolution = cellSize;
        _chunkLength = chunkLength;
        _chunkWidth = chunkWidth;
        _noiseData = noiseData;
        chunkObject = obj;

        mesh = new Mesh();
        vertices = new Vector3[(_chunkLength + 1) * (_chunkWidth + 1)];
        triangles = new int[_chunkLength * _chunkWidth * 6];
    }
    public void UpdateThread(Action<Chunk> callback)
    {
        ThreadStart threadStart = delegate
        {
            UpdateChunk(callback);
        };
        new Thread(threadStart).Start();
    }
    private void UpdateChunk(Action<Chunk> callback)
    {
        lock(ThreadInfoQueue)
        {
            int tris = 0;
            int verts = 0;
            float vertexOffset = _resolution * 0.5f;

            float[,] noiseMap = Noise.GenerateNoiseMap(_noiseData);

            for (int x = 0; x <= _chunkWidth; x++)
            {
                for (int z = 0; z <= _chunkLength; z++)
                {
                    vertices[verts] = new Vector3((x * _resolution) - vertexOffset, _noiseData.heightMultiplier.Evaluate(noiseMap[x, z]), (z * _resolution) - vertexOffset);
                    verts++;
                }
            }
            verts = 0;

            for (int x = 0; x < _chunkWidth; x++)
            {
                for (int z = 0; z < _chunkLength; z++)
                {
                    triangles[tris] = verts;
                    triangles[tris + 1] = triangles[tris + 4] = verts + 1;
                    triangles[tris + 2] = triangles[tris + 3] = verts + (_chunkLength + 1);
                    triangles[tris + 5] = verts + (_chunkLength + 1) + 1;
                    verts++;
                    tris += 6;
                }
                verts++;
            }

            ThreadInfoQueue.Enqueue(new ThreadInfo<Chunk>(callback, this));
        }
    }
    public void UpdateChunk(float resolution, int chunkLength, int chunkWidth, NoiseData noiseData)
    {
        _resolution = resolution;
        _chunkLength = chunkLength;
        _chunkWidth = chunkWidth;
        _noiseData = noiseData;

        vertices = new Vector3[(_chunkLength + 1) * (_chunkWidth + 1)];
        triangles = new int[_chunkLength * _chunkWidth * 6];
    }
    public int[] GetTriangles()
    {
        return triangles;
    }
    public Vector3[] GetVertices()
    {
        return vertices;
    }
}

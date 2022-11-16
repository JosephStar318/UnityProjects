using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using System;

public class Chunk
{
    //private Noise noise = new Noise();
    public GameObject chunkObject;
    public Mesh mesh;
    private NoiseData _noiseData;
    private Vector3[] vertices;
    private Vector3[] borderedVertices;
    private Vector2 _chunkOffset;
    private int[] triangles;
    private int[] borderedTriangles;

    private float _resolution;
    private int _chunkLength;
    private int _borderedChunkLength;

    public static Queue<ThreadInfo<Chunk>> ThreadInfoQueue = new Queue<ThreadInfo<Chunk>>();

    public Chunk(GameObject obj, float cellSize, int chunkLength, NoiseData noiseData, Vector3 chunkOffset)
    {
        _resolution = cellSize;
        _chunkLength = chunkLength;
        _noiseData = noiseData;
        chunkObject = obj;
        _borderedChunkLength = chunkLength + 2;
        _chunkOffset = chunkOffset;
        mesh = new Mesh();

        AdjustMesh();
    }

    private void AdjustMesh()
    {
        vertices = new Vector3[(_chunkLength + 1) * (_chunkLength + 1)];
        triangles = new int[_chunkLength * _chunkLength * 6];

        borderedVertices = new Vector3[(_borderedChunkLength + 1) * (_borderedChunkLength + 1)];
        borderedTriangles = new int[_borderedChunkLength * _borderedChunkLength * 6];
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
            int vert = 0;
            int borderedTris = 0;
            float vertexOffset = _resolution * 0.5f;
            float[,] noiseMap = Noise.GenerateNoiseMap(_noiseData, _chunkOffset);
            bool isBorder;
            for (int x = 0; x <= _borderedChunkLength; x++)
            {
                for (int z = 0; z <= _borderedChunkLength; z++)
                {
                    isBorder = x == 0 || z == 0 || x == _borderedChunkLength || z == _borderedChunkLength;
                    if (!isBorder)
                    {
                        vertices[vert++] = new Vector3((x * _resolution) - vertexOffset, _noiseData.heightMultiplier.Evaluate(noiseMap[x, z]), (z * _resolution) - vertexOffset);
                    }
                    borderedVertices[x * (_borderedChunkLength + 1) + z] = new Vector3((x * _resolution) - vertexOffset, _noiseData.heightMultiplier.Evaluate(noiseMap[x, z]), (z * _resolution) - vertexOffset);
                }
            }
            vert = 0;
            for (int x = 0; x < _borderedChunkLength; x++)
            {
                for (int z = 0; z < _borderedChunkLength; z++)
                {
                    isBorder = x == 0 || z == 0 || x == _borderedChunkLength - 1 || z == _borderedChunkLength - 1;

                    if (!isBorder)
                    {
                        triangles[tris] = vert;
                        triangles[tris + 1] = triangles[tris + 4] = vert + 1;
                        triangles[tris + 2] = triangles[tris + 3] = vert + (_chunkLength) + 1;
                        triangles[tris + 5] = vert + (_chunkLength + 1) + 1;
                        tris += 6;
                        vert++;
                    }
                    borderedTriangles[borderedTris] = x * (_borderedChunkLength + 1) + z;
                    borderedTriangles[borderedTris + 1] = borderedTriangles[borderedTris + 4] = x * (_borderedChunkLength + 1) + z + 1;
                    borderedTriangles[borderedTris + 2] = borderedTriangles[borderedTris + 3] = (x + 1) * (_borderedChunkLength + 1) + z;
                    borderedTriangles[borderedTris + 5] = (x + 1) * (_borderedChunkLength + 1) + z + 1;
                    borderedTris += 6;
                }
                if (!(x == 0 || x == _borderedChunkLength)) vert++;
            }

            ThreadInfoQueue.Enqueue(new ThreadInfo<Chunk>(callback, this));
        }
    }
    public void UpdateChunkInEditor(float resolution, int chunkLength, NoiseData noiseData)
    {
        _resolution = resolution;
        _chunkLength = chunkLength;
        _noiseData = noiseData;
        _borderedChunkLength = chunkLength + 2;

        AdjustMesh();

        int tris = 0;
        int vert = 0;
        int borderedTris = 0;
        float vertexOffset = _resolution * 0.5f;

        float[,] noiseMap = Noise.GenerateNoiseMap(_noiseData, _chunkOffset);
        bool isBorder;
        for (int x = 0; x <= _borderedChunkLength; x++)
        {
            for (int z = 0; z <= _borderedChunkLength; z++)
            {
                isBorder = x == 0 || z == 0 || x == _borderedChunkLength || z == _borderedChunkLength;
                if(!isBorder)
                {
                    vertices[vert++] = new Vector3((x * _resolution) - vertexOffset, _noiseData.heightMultiplier.Evaluate(noiseMap[x, z]), (z * _resolution) - vertexOffset);
                }
                borderedVertices[x * (_borderedChunkLength + 1) + z] = new Vector3((x * _resolution) - vertexOffset, _noiseData.heightMultiplier.Evaluate(noiseMap[x, z]), (z * _resolution) - vertexOffset);
            }
        }
        vert = 0;
        for (int x = 0; x < _borderedChunkLength; x++)
        {
            for (int z = 0; z < _borderedChunkLength; z++)
            {
                isBorder = x == 0 || z == 0 || x == _borderedChunkLength - 1 || z == _borderedChunkLength - 1;
                
                if(!isBorder)
                {
                    triangles[tris] = vert;
                    triangles[tris + 1] = triangles[tris + 4] = vert + 1;
                    triangles[tris + 2] = triangles[tris + 3] = vert + (_chunkLength) + 1;
                    triangles[tris + 5] = vert + (_chunkLength + 1) + 1;
                    tris += 6;
                    vert++;
                }
                borderedTriangles[borderedTris] = x * (_borderedChunkLength + 1) + z;
                borderedTriangles[borderedTris + 1] = borderedTriangles[borderedTris + 4] = x * (_borderedChunkLength + 1) + z + 1;
                borderedTriangles[borderedTris + 2] = borderedTriangles[borderedTris + 3] = (x + 1) * (_borderedChunkLength + 1) + z;
                borderedTriangles[borderedTris + 5] = (x + 1) * (_borderedChunkLength + 1) + z + 1;
                borderedTris += 6;
            }
            if(!(x == 0 || x == _borderedChunkLength)) vert++;
        }
    }
    
    private Vector3[] GenerateBorderlessNormalMap(Vector3[] oldNormalMap, int length)
    {
        Vector3[] normalMap = new Vector3[(length-1) * (length-1)];
        int counter = 0;
        bool isBorder;
        for (int i = 0; i <= length; i++)
        {
            for (int j = 0; j <= length; j++)
            {
                isBorder = i == 0 || j == 0 || i == length || j == length;
                if(!isBorder)
                    normalMap[counter++] = oldNormalMap[i * (length + 1) + j];
            }
        }
        return normalMap;
    }
    public Mesh GetMesh()
    {
        Mesh tempMesh = new Mesh();
        tempMesh.SetVertices(borderedVertices);
        tempMesh.triangles = borderedTriangles;
        tempMesh.RecalculateNormals();

        mesh.SetVertices(vertices);
        mesh.triangles = triangles;
        mesh.SetNormals(GenerateBorderlessNormalMap(tempMesh.normals, _borderedChunkLength));

        return mesh;
    }
}

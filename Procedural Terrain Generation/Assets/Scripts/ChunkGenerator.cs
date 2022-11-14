using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    public float resolution = 1;
    public int chunkLength = 1;
    public int chunkWidth = 1;
    public int chunkSize = 1;
    public Material material;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    public NoiseData noiseAttributes;
    Chunk[,] chunks;

    void Start()
    {
        CreateChunks();
    }
    public void CreateChunks()
    {
        chunks = new Chunk[chunkSize, chunkSize];
        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                CreateChunk(x,z);
            }
        }
    }
    public void UpdateChunks()
    {
        if(chunks != null)
        {
            foreach (Chunk chunk in chunks)
            {
                if (chunk != null)
                {
                    chunk.UpdateChunk(resolution, chunkLength, chunkWidth, noiseAttributes);
                    meshFilter.sharedMesh.vertices = chunk.GetVertices();
                    meshFilter.sharedMesh.triangles = chunk.GetTriangles();
                    meshFilter.sharedMesh.RecalculateNormals();
                    meshCollider.sharedMesh = meshFilter.sharedMesh;
                }
            }
        }
        
    }
    private void CreateChunk(int x, int z)
    {
        Vector3 chunkOffset = new Vector3(x * chunkLength * resolution, 0, z * chunkLength * resolution);

        chunks[x,z] = new Chunk(resolution, chunkLength, chunkWidth, noiseAttributes);

        GameObject newChunk = new GameObject("Chunk");
        newChunk.transform.position = chunkOffset;
        meshFilter = newChunk.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = chunks[x, z].mesh;

        meshCollider = newChunk.AddComponent<MeshCollider>();
        newChunk.AddComponent<MeshRenderer>().material = material;

        meshFilter.sharedMesh.vertices = chunks[x,z].GetVertices();
        meshFilter.sharedMesh.triangles = chunks[x, z].GetTriangles();
        meshFilter.sharedMesh.RecalculateNormals();
        meshCollider.sharedMesh = meshFilter.sharedMesh;
    }

    private void OnValidate()
    {
        //always keep the same as chunk sizes
        noiseAttributes.width = chunkWidth;
        noiseAttributes.length = chunkLength;
    }

}



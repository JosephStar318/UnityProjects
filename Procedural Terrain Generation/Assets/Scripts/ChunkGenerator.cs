using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    public float resolution = 1;
    [Range(0,1)] public float noiseMultiplier;
    public int chunkLength = 1;
    public int chunkSize = 1;
    public Material material;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    Chunk[,] chunks;

    void Start()
    {
        chunks = new Chunk[chunkSize, chunkSize];
        CreateChunks();
    }
    private void CreateChunks()
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                CreateChunk(x,z);
            }
        }
    }
    private void CreateChunk(int x, int z)
    {
        Vector3 chunkOffset = new Vector3(x * chunkLength * resolution, 0, z * chunkLength * resolution);

        chunks[x,z] = new Chunk(resolution, chunkLength, noiseMultiplier);

        GameObject newChunk = Instantiate(new GameObject(), chunkOffset, Quaternion.identity);
        meshFilter = newChunk.AddComponent<MeshFilter>();
        meshCollider = newChunk.AddComponent<MeshCollider>();
        newChunk.AddComponent<MeshRenderer>().material = material;

        meshFilter.mesh.vertices = chunks[x,z].GetVertices();
        meshFilter.mesh.triangles = chunks[x, z].GetTriangles();
        meshFilter.mesh.RecalculateNormals();
        meshCollider.sharedMesh = meshFilter.mesh;
    }
    void Update()
    {
        //foreach (Chunk chunk in chunks)
        //{
        //    chunk.UpdateChunk(resolution, chunkLength, noiseMultiplier);
        //    meshFilter.mesh.vertices = chunk.GetVertices();
        //    meshFilter.mesh.triangles = chunk.GetTriangles();
        //    meshFilter.mesh.RecalculateNormals();
        //    meshCollider.sharedMesh = meshFilter.mesh;
        //}
    }

}

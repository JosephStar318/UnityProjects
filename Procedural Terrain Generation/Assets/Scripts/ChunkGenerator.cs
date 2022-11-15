using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    public float resolution = 1;
    public int chunkLength = 1;
    public int chunkWidth = 1;
    public int chunkRenderRadius = 1;
    public bool autoGenerate = false;

    public Material material;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    public NoiseData noiseAttributes;
    public Transform player;
    private TerrainData terrainData = new TerrainData();
    private Vector2 lastSpawnPos;
    
    void Start()
    {
        //load player position before start if needed
        GenerateChunks();
    }

    public void GenerateChunks()
    {
        int spawnIndexX = Mathf.FloorToInt(player.position.x / chunkWidth);
        int spawnIndexZ = Mathf.FloorToInt(player.position.z / chunkLength);

        if (terrainData.GetChunkCount() == 0)
        {
            //createchunk by chunkRenderRadius around the player
            lastSpawnPos = new Vector2(spawnIndexX, spawnIndexZ);

            for (int i = -chunkRenderRadius; i <= chunkRenderRadius; i++)
            {
                for (int j = -chunkRenderRadius; j <= chunkRenderRadius; j++)
                {

                    CreateChunk(spawnIndexX + i, spawnIndexZ + j);
                }
            }
        }
        else
        {
            //generate necessary chunks
            //terrainData.GetActiveChunkCount() != Mathf.Pow((chunkRenderRadius * 2 + 1), 2)
            if (Vector2.Distance(lastSpawnPos, new Vector2(spawnIndexX, spawnIndexZ)) > 0)
            {
                terrainData.UnloadChunks();
                for (int i = -chunkRenderRadius; i <= chunkRenderRadius; i++)
                {
                    for (int j = -chunkRenderRadius; j <= chunkRenderRadius; j++)
                    {
                        Vector2 pos = new Vector2(spawnIndexX + i, spawnIndexZ + j);
                        lastSpawnPos = new Vector2(spawnIndexX, spawnIndexZ);

                        if (terrainData.FindChunk(pos) == true)
                        {
                            terrainData.LoadChunk(pos);
                        }
                        else
                        {
                            CreateChunk(spawnIndexX + i, spawnIndexZ + j);
                        }
                    }
                }
            }
        }
        
    }

    public void UpdateChunks()
    {
        if (terrainData != null)
        {
            foreach (var chunk in terrainData.GetChunks())
            {
                chunk.UpdateChunk(resolution, chunkLength, chunkWidth, noiseAttributes);
                chunk.UpdateThread(ApplyMesh);
            }
        }

    }
    private void CreateChunk(int x, int z)
    {
        Vector3 chunkOffset = new Vector3(x * chunkWidth * resolution, 0, z * chunkLength * resolution);

        GameObject chunkObject = new GameObject("Chunk");
        chunkObject.transform.parent = transform;

        Chunk newChunk  = new Chunk(chunkObject, resolution, chunkLength, chunkWidth, noiseAttributes);

        terrainData.AddChunk(new Vector2(x, z), newChunk);
        newChunk.chunkObject.transform.position = chunkOffset;
        newChunk.UpdateThread(ApplyMesh);
    }
    private void ApplyMesh(Chunk chunk)
    {
        meshFilter = chunk.chunkObject.AddComponent<MeshFilter>();
        meshCollider = chunk.chunkObject.AddComponent<MeshCollider>();
        chunk.chunkObject.AddComponent<MeshRenderer>().material = material;

        chunk.mesh.vertices = chunk.GetVertices();
        chunk.mesh.triangles = chunk.GetTriangles();
        chunk.mesh.RecalculateNormals();
        meshFilter.sharedMesh = chunk.mesh;
        meshCollider.sharedMesh = meshFilter.sharedMesh;
    }

    private void OnValidate()
    {
        //always keep the same as chunk sizes
        noiseAttributes.width = chunkWidth;
        noiseAttributes.length = chunkLength;
    }
    private void Update()
    {
        GenerateChunks();
        if(Chunk.ThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < Chunk.ThreadInfoQueue.Count; i++)
            {
                ThreadInfo<Chunk> threadInfo = Chunk.ThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.param);
            }
        }
    }

}



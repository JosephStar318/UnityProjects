using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    public float resolution = 1;
    public int chunkLength = 1;
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
        int spawnIndexX = Mathf.FloorToInt(player.position.x / chunkLength);
        int spawnIndexZ = Mathf.FloorToInt(player.position.z / chunkLength);

        if (terrainData.GetChunkCount() == 0)
        {
            //createchunk by chunkRenderRadius around the player
            lastSpawnPos = new Vector2(spawnIndexX, spawnIndexZ);

            for (int i = -chunkRenderRadius; i <= chunkRenderRadius; i++)
            {
                for (int j = -chunkRenderRadius; j <= chunkRenderRadius; j++)
                {
                    //for spherical radius 
                    if (i * i + j * j < chunkRenderRadius * chunkRenderRadius)
                        CreateChunk(spawnIndexX + i, spawnIndexZ + j);
                }
            }
        }
        else
        {
            //generate necessary chunks
            if (Vector2.Distance(lastSpawnPos, new Vector2(spawnIndexX, spawnIndexZ)) > 0 || terrainData.GetActiveChunkCount() != Mathf.Pow((chunkRenderRadius * 2 + 1), 2))
            {
                terrainData.UnloadChunks();
                for (int i = -chunkRenderRadius; i <= chunkRenderRadius; i++)
                {
                    for (int j = -chunkRenderRadius; j <= chunkRenderRadius; j++)
                    {
                        //for spherical radius 
                        if (i * i + j * j < chunkRenderRadius * chunkRenderRadius)
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
    }
    public void GenerateChunksInEditor()
    {
        int spawnIndexX = Mathf.FloorToInt(player.position.x / chunkLength);
        int spawnIndexZ = Mathf.FloorToInt(player.position.z / chunkLength);

        if (terrainData.GetChunkCount() == 0)
        {
            //createchunk by chunkRenderRadius around the player
            lastSpawnPos = new Vector2(spawnIndexX, spawnIndexZ);

            for (int i = -chunkRenderRadius; i <= chunkRenderRadius; i++)
            {
                for (int j = -chunkRenderRadius; j <= chunkRenderRadius; j++)
                {
                    //for spherical radius 
                    if (i*i + j*j < chunkRenderRadius*chunkRenderRadius)
                        CreateChunkInEditor(spawnIndexX + i, spawnIndexZ + j);
                }
            }
        }
        else
        {
            if (Vector2.Distance(lastSpawnPos, new Vector2(spawnIndexX, spawnIndexZ)) > 0 || terrainData.GetActiveChunkCount() != Mathf.Pow((chunkRenderRadius * 2 + 1), 2))
            {
                terrainData.UnloadChunks();
                for (int i = -chunkRenderRadius; i <= chunkRenderRadius; i++)
                {
                    for (int j = -chunkRenderRadius; j <= chunkRenderRadius; j++)
                    {
                        //for spherical radius 
                        if (i * i + j * j < chunkRenderRadius * chunkRenderRadius)
                        {
                            Vector2 pos = new Vector2(spawnIndexX + i, spawnIndexZ + j);
                            lastSpawnPos = new Vector2(spawnIndexX, spawnIndexZ);

                            if (terrainData.FindChunk(pos) == true)
                            {
                                terrainData.LoadChunk(pos);
                            }
                            else
                            {
                                CreateChunkInEditor(spawnIndexX + i, spawnIndexZ + j);
                            }
                        }
                    }
                }
            }
        }
    }

    public void UpdateChunksInEditor()
    {
        if (terrainData != null)
        {
            foreach (var chunk in terrainData.GetChunks())
            {
                if(chunk != null)
                {
                    chunk.UpdateChunkInEditor(resolution, chunkLength, noiseAttributes);

                    meshFilter = chunk.chunkObject.GetComponent<MeshFilter>();
                    meshCollider = chunk.chunkObject.GetComponent<MeshCollider>();

                    meshFilter.sharedMesh = chunk.GetMesh();
                    meshCollider.sharedMesh = meshFilter.sharedMesh;
                }
            }
        }
    }
    public void CreateChunkInEditor(int x, int z)
    {
        Vector2 chunkOffset = new Vector2(x * chunkLength * resolution, z * chunkLength * resolution);

        GameObject chunkObject = new GameObject("Chunk");
        chunkObject.transform.parent = transform;

        Chunk newChunk = new Chunk(chunkObject, resolution, chunkLength, noiseAttributes, chunkOffset);

        terrainData.AddChunk(new Vector2(x, z), newChunk);
        newChunk.chunkObject.transform.position = new Vector3(chunkOffset.x,0,chunkOffset.y);

        newChunk.UpdateChunkInEditor(resolution, chunkLength, noiseAttributes);

        meshFilter = newChunk.chunkObject.AddComponent<MeshFilter>();
        meshCollider = newChunk.chunkObject.AddComponent<MeshCollider>();
        newChunk.chunkObject.AddComponent<MeshRenderer>().material = material;

        meshFilter.sharedMesh = newChunk.GetMesh();
        meshCollider.sharedMesh = meshFilter.sharedMesh;
    }
    private void CreateChunk(int x, int z)
    {
        Vector2 chunkOffset = new Vector2(x * chunkLength * resolution, z * chunkLength * resolution);

        GameObject chunkObject = new GameObject("Chunk");
        chunkObject.transform.parent = transform;

        Chunk newChunk = new Chunk(chunkObject, resolution, chunkLength, noiseAttributes, chunkOffset);

        terrainData.AddChunk(new Vector2(x, z), newChunk);
        newChunk.chunkObject.transform.position = new Vector3(chunkOffset.x, 0, chunkOffset.y);
        newChunk.UpdateThread(ApplyMesh);
    }
    private void ApplyMesh(Chunk chunk)
    {
        meshFilter = chunk.chunkObject.AddComponent<MeshFilter>();
        meshCollider = chunk.chunkObject.AddComponent<MeshCollider>();
        chunk.chunkObject.AddComponent<MeshRenderer>().material = material;

        meshFilter.sharedMesh = chunk.GetMesh();
        meshCollider.sharedMesh = meshFilter.sharedMesh;
    }
    private void OnValidate()
    {
        //always keep the same as bordered chunk sizes
        noiseAttributes.length = chunkLength + 2;
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



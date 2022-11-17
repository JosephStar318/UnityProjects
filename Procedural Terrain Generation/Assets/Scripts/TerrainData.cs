using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainData
{
    private Dictionary<Vector2, Chunk> chunkDictionary = new Dictionary<Vector2, Chunk>();
    private int activeChunkCount;
    
    public bool AddChunk(Vector2 chunkPos, Chunk chunk)
    {
        if(chunkDictionary.TryAdd(chunkPos, chunk))
        {
            activeChunkCount++;
            return true;
        }
        else
        {
            Debug.Log("Could not added chunk to dictionary. There may be a chunk occupying the same position.");
            return false;
        }
    }
    public bool RemoveChunk(Vector2 chunkPos)
    {
        if(chunkDictionary.Remove(chunkPos))
        {
            activeChunkCount--;
            Debug.Log("Chunk Successfully removed");
            return true;
        }
        else
        {
            Debug.Log("There is no chunk in that position");
            return false;
        }
    }
    public Dictionary<Vector2,Chunk>.ValueCollection GetChunks()
    {
        return chunkDictionary.Values;
    }
    public int GetChunkCount()
    {
        return chunkDictionary.Count;
    }
    public int GetActiveChunkCount()
    {
        return activeChunkCount;
    }

    public void UnloadChunks()
    {
        activeChunkCount = 0;
        foreach (var chunkPair in chunkDictionary)
        {
            chunkPair.Value.chunkObject.SetActive(false);
        }
    }
    public bool LoadChunk(Vector2 position)
    {
        if(chunkDictionary.TryGetValue(position,out Chunk chunk))
        {
            chunk.chunkObject.SetActive(true);
            activeChunkCount++;
            return true;
        }
        else
        {
            Debug.Log("Could not load chunk at specified position");
            return false;
        }
    }

    public bool FindChunk(Vector2 position)
    {
        return chunkDictionary.ContainsKey(position);
    }
}

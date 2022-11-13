using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    private float _resolution;
    private int _chunkLength;


    public Chunk(float cellSize, int chunkLength, float terrainNoise)
    {
        _resolution = cellSize;
        _chunkLength = chunkLength;

        mesh = new Mesh();

        vertices = new Vector3[(chunkLength + 1)*(chunkLength + 1)];
        triangles = new int[chunkLength *  chunkLength* 6];

        UpdateChunk(terrainNoise);

        mesh.vertices = vertices;
        mesh.triangles = triangles;

    }
    private void UpdateChunk(float terrainNoise)
    {
        int tris = 0;
        int verts = 0;
        float vertexOffset = _resolution * 0.5f;

        for (int x = 0; x <= _chunkLength; x++)
        {
            for (int z = 0; z <= _chunkLength; z++)
            {
                float y = Mathf.PerlinNoise(x * terrainNoise, z * terrainNoise);
                vertices[verts] = new Vector3((x * _resolution) - vertexOffset, y, (z * _resolution) - vertexOffset);
                verts++;
            }
        }
        verts = 0;

        for (int x = 0; x < _chunkLength; x++)
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

    }
    public void UpdateChunk(float resolution, int chunkLength,float terrainNoise)
    {
        _resolution = resolution;
        _chunkLength = chunkLength;

        vertices = new Vector3[(chunkLength + 1) * (chunkLength + 1)];
        triangles = new int[chunkLength * chunkLength * 6];

        UpdateChunk(terrainNoise);

        mesh.vertices = vertices;
        mesh.triangles = triangles;
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

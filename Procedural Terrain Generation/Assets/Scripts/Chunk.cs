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
    private int _chunkWidth;


    public Chunk(float cellSize, int chunkLength, int chunkWidth, NoiseData noiseData)
    {
        _resolution = cellSize;
        _chunkLength = chunkLength;
        _chunkWidth = chunkWidth;

        mesh = new Mesh();

        vertices = new Vector3[(chunkLength + 1)*(chunkWidth + 1)];
        triangles = new int[chunkLength * chunkWidth * 6];

        UpdateChunk(noiseData);

        mesh.vertices = vertices;
        mesh.triangles = triangles;

    }
    private void UpdateChunk(NoiseData noiseData)
    {
        int tris = 0;
        int verts = 0;
        float vertexOffset = _resolution * 0.5f;

        float[,] noiseMap = Noise.GenerateNoiseMap(noiseData);


        for (int x = 0; x <= _chunkWidth; x++)
        {
            for (int z = 0; z <= _chunkLength; z++)
            {
                vertices[verts] = new Vector3((x * _resolution) - vertexOffset, noiseData.heightMultiplier.Evaluate(noiseMap[x, z]), (z * _resolution) - vertexOffset);
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

    }
    public void UpdateChunk(float resolution, int chunkLength, int chunkWidth, NoiseData noiseData)
    {
        _resolution = resolution;
        _chunkLength = chunkLength;
        _chunkWidth = chunkWidth;

        vertices = new Vector3[(chunkLength + 1) * (chunkWidth + 1)];
        triangles = new int[chunkLength * chunkWidth * 6];

        UpdateChunk(noiseData);

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

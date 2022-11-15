using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ChunkGenerator))]
public class ChunkGenerationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ChunkGenerator chunkGenerator = (ChunkGenerator)target;
        if(DrawDefaultInspector())
        {
            chunkGenerator.UpdateChunks();
        }
        if(chunkGenerator.autoGenerate == true)
        {
            chunkGenerator.GenerateChunks();
        }
        if(GUILayout.Button("Generate Chunks"))
        {
            chunkGenerator.GenerateChunks();
        }
    }
    
}

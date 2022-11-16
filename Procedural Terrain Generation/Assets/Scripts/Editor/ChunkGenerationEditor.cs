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
            chunkGenerator.UpdateChunksInEditor();
        }
        if(chunkGenerator.autoGenerate == true)
        {
            chunkGenerator.GenerateChunksInEditor();
        }
        if(GUILayout.Button("Generate Chunks"))
        {
            chunkGenerator.GenerateChunksInEditor();
        }
    }
    
}

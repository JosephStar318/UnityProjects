using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlockSpawnManager : MonoBehaviour
{
    public List<GameObject> blocks;
    public int rows;
    public int columns;
    public float spawnOffsetX;
    public float spawnOffsetY;
    private int blockTypeCount;
    private float gapX;
    private float gapY;
    // Start is called before the first frame update
    void Start()
    {
        blockTypeCount = blocks.Count;
        gapX = blocks[0].transform.localScale.x*2.2f;
        gapY = blocks[0].transform.localScale.y*2.5f;
        SpawnBlocks();
    }

    private void SpawnBlocks()
    {
        int randomType;

        for(int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                randomType = Random.Range(0, blockTypeCount);
                Vector3 spawnPos = new Vector3(spawnOffsetX + j * gapX, spawnOffsetY + i * gapY,-5);
                Instantiate(blocks[randomType], spawnPos, Quaternion.identity);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

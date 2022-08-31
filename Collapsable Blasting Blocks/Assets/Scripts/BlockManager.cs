using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlockManager : MonoBehaviour
{
    public List<GameObject> prefabBlocks;
    
    private Block[,] blockMatrix;
    private bool[,] occurenceMatrix;

    private int groupIDCounter = 0;

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
        blockMatrix = new Block[rows, columns];
        occurenceMatrix = new bool[rows, columns];

        SpawnBlocks();
        CheckGroups();
    }
    
    private void CheckGroups()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if(occurenceMatrix[i,j] == false)
                {
                    BlockGroup bg = new BlockGroup();
                    ClusterCheck(i,j, bg);
                    bg.blocks.Add(blockMatrix[i, j]);
                    bg.GroupID = groupIDCounter++;
                    BlockGroup.blockGroups.Add(bg);

                }
            }
        }
    }

    /*
     Look for clusters and store them in groups.
     */
    private void ClusterCheck(int i, int j,BlockGroup bg)
    {
        //matrix limits
        if(i+1 < rows)
        {
            //first look up
            if (blockMatrix[i, j].blockType == blockMatrix[i + 1, j].blockType)
            {
                occurenceMatrix[i, j] = true;
                occurenceMatrix[i + 1, j] = true;
                bg.blocks.Add(blockMatrix[i + 1, j]);
                ClusterCheck(i + 1, j,bg);
            }
        }
        //matrix limits
        if (j+1 < columns)
        {
            //then look right
            if (blockMatrix[i, j].blockType == blockMatrix[i, j + 1].blockType)
            {
                occurenceMatrix[i, j] = true;
                occurenceMatrix[i, j + 1] = true;
                bg.blocks.Add(blockMatrix[i, j + 1]);
                ClusterCheck(i, j + 1,bg);
            }
        }


    }

    private void SpawnBlocks()
    {
        int randomType;

        blockTypeCount = prefabBlocks.Count;
        gapX = prefabBlocks[0].transform.localScale.x * 2.2f;
        gapY = prefabBlocks[0].transform.localScale.y * 2.5f;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                randomType = Random.Range(0, blockTypeCount);
                Vector3 spawnPos = new Vector3(spawnOffsetX + j * gapX, spawnOffsetY + i * gapY,-5);
                Instantiate(prefabBlocks[randomType], spawnPos, Quaternion.identity);

                blockMatrix[i, j] = new Block(
                    prefabBlocks[randomType].GetComponent<BlockComponent>().blockType,
                    prefabBlocks[randomType].GetComponent<BlockComponent>().blockState,
                    prefabBlocks[randomType]
                    );
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        foreach (BlockGroup item in BlockGroup.blockGroups)
        {
            Debug.Log("\n Group: " + item.GroupID + " " + item.blocks.Count);
        }
    }
    private void OnMouseDown()
    {
        
    }
}

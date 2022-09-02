using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlockManager : MonoBehaviour
{
    public static BlockManager instance;

    public List<GameObject> prefabBlocks;
    private Block[,] blockMatrix;
    private bool[,] occurenceMatrix;

    public bool isBlasted = false;
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
        instance = this;
        blockMatrix = new Block[rows*2, columns];
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
                    ChanceBlockState(bg);

                }
            }
        }
    }

    private void ChanceBlockState(BlockGroup bg)
    {
        if(bg.blocks.Count > 4)
        {
            bg.blocks.ForEach(block => block.BlockState = BlockState.First);
        }
        else if(bg.blocks.Count > 4 && bg.blocks.Count < 6)
        {
            bg.blocks.ForEach(block => block.BlockState = BlockState.Second);
        }
        else if (bg.blocks.Count > 6)
        {
            bg.blocks.ForEach(block => block.BlockState = BlockState.Third);
        }
    }

    /*
     Look for clusters and store them in groups.
     */
    private void ClusterCheck(int i, int j, BlockGroup bg)
    {
        
        //matrix limits
        if(i+1 < rows && occurenceMatrix[i + 1, j] == false)
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
        if (j+1 < columns && occurenceMatrix[i, j + 1] == false)
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
        //matrix limits
        if (i - 1 >= 0 && occurenceMatrix[i - 1, j] == false)
        {
            //then look down
            if (blockMatrix[i, j].blockType == blockMatrix[i - 1, j].blockType)
            {
                occurenceMatrix[i, j] = true;
                occurenceMatrix[i - 1, j] = true;
                bg.blocks.Add(blockMatrix[i - 1, j]);
                ClusterCheck(i - 1, j, bg);
            }
        }
        //matrix limits
        if (j - 1 >= 0 && occurenceMatrix[i, j - 1] == false)
        {
            //then look left
            if (blockMatrix[i, j].blockType == blockMatrix[i, j - 1].blockType)
            {
                occurenceMatrix[i, j] = true;
                occurenceMatrix[i, j - 1] = true;
                bg.blocks.Add(blockMatrix[i, j - 1]);
                ClusterCheck(i, j - 1, bg);
            }
        }
    }

    private void SpawnBlocks()
    {
        int randomType;

        blockTypeCount = prefabBlocks.Count;
        gapX = prefabBlocks[0].transform.localScale.x * 2.2f;
        gapY = prefabBlocks[0].transform.localScale.y * 2.5f;

        for (int i = 0; i < rows*2; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                randomType = Random.Range(0, blockTypeCount);
                Vector3 spawnPos = new Vector3(spawnOffsetX + j * gapX, spawnOffsetY + i * gapY,-5);
                GameObject instance = Instantiate(prefabBlocks[randomType], spawnPos, Quaternion.identity);
                blockMatrix[i, j] = new Block(
                    prefabBlocks[randomType].GetComponent<BlockComponent>().blockType,
                    prefabBlocks[randomType].GetComponent<BlockComponent>().blockState,
                    instance
                    );
            }
        }

    }
    public void CollapseBlocks()
    {
        //check if null spaces for each column from bottom to top
        for (int j = 0; j < columns; j++)
        {
            int nullCount = 0;
            for (int i = 0; i < rows*2; i++)
            {
                if (blockMatrix[i, j].block == null)
                {
                    nullCount++;
                }
                else
                {
                    if(nullCount>0)
                    {
                        //swap elements
                        Block temp = blockMatrix[i-nullCount, j];
                        blockMatrix[i- nullCount, j] = blockMatrix[i, j];
                        blockMatrix[i, j] = temp;
                    }
                }
            }
        }
        SpawnSpareBlocks();
        BlockGroup.blockGroups.Clear();
        System.Array.Clear(occurenceMatrix,0,occurenceMatrix.Length);
        CheckGroups();
    }

    private void SpawnSpareBlocks()
    {
        int randomType;

        for (int i = rows; i < rows*2; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if(blockMatrix[i,j].block == null)
                {
                    randomType = Random.Range(0, blockTypeCount);
                    Vector3 spawnPos = new Vector3(spawnOffsetX + j * gapX, spawnOffsetY + i * gapY, -5);
                    GameObject instance = Instantiate(prefabBlocks[randomType], spawnPos, Quaternion.identity);
                    blockMatrix[i, j] = new Block(
                    prefabBlocks[randomType].GetComponent<BlockComponent>().blockType,
                    prefabBlocks[randomType].GetComponent<BlockComponent>().blockState,
                    instance
                    );
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isBlasted)
        {
            //make a delay to catch up with destroy metod
            Invoke(nameof(CollapseBlocks), 2);
            isBlasted = false;
        }
    }
}

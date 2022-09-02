using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Block
{
    public BlockType blockType;
    private BlockState blockState;
    public GameObject block;

    public BlockState BlockState 
    {
        get => blockState; 
        set
        {
            if(BlockState == BlockState.First)
            {
                if(blockType == BlockType.Blue)
                {
                    //change sprite
                }
            }
        } 
    }

    public Block()
    {

    }
    public Block(BlockType blockType, BlockState blockState, GameObject block)
    {
        this.blockType = blockType;
        this.BlockState = blockState;
        this.block = block;
    }
}

public enum BlockType
{
    Blue = 0,
    Green,
    Pink,
    Purple,
    Red,
    Yellow
}
public enum BlockState
{
    Default = 0,
    First,
    Second,
    Third
}

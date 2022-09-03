using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Block : MonoBehaviour
{
    public List<Sprite> sprites;

    public BlockType blockType;
    private BlockState blockState;
    public int parentGroupID;
    public BlockState BlockState 
    {
        get => blockState; 
        set
        {
            blockState = value;
            GetComponent<SpriteRenderer>().sprite = sprites[(int)blockState];
        } 
    }
    private void OnMouseDown()
    {
        if (BlockManager.instance.isBlasted == false)
        {
            //destroy blocks
            foreach (var item in BlockGroup.blockGroups)
            {
                if (item.blocks.Count > 1)
                {
                    if (item.GroupID == parentGroupID)
                    {
                        BlockManager.instance.isBlasted = true;
                        item.blocks.ForEach(block => Destroy(block.gameObject));
                    }
                }
            }
        }
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockComponent : MonoBehaviour
{
    public BlockType blockType;
    public BlockState blockState;
    public int parentGroupID;

    private void OnMouseDown()
    {
        if(BlockManager.instance.isBlasted == false)
        {
            //destroy blocks
            foreach (var item in BlockGroup.blockGroups)
            {
                if (item.GroupID == parentGroupID)
                {
                    item.blocks.ForEach(i => Destroy(i.block));
                }
            }
            BlockManager.instance.isBlasted = true;
            
        }
    }


}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGroup
{
    public static List<BlockGroup> blockGroups = new List<BlockGroup>();
    public List<Block> blocks = new List<Block>();
    private int groupID;

    public int GroupID 
    { 
        get => groupID;
        set 
        { 
            groupID = value;
            //whenever group id set, each child's parentGroupID will be set as well
            blocks.ForEach(block => block.parentGroupID = value);
        }
    }
}

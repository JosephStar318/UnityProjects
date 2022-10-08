using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class LevelData
{
    public int levelID;
    public int levelTargetScore;
    public float levelSpeed;
    public LevelData(int levelID, int levelTargetScore, float levelSpeed)
    {
        this.levelID = levelID;
        this.levelTargetScore = levelTargetScore;
        this.levelSpeed = levelSpeed;
    }

}

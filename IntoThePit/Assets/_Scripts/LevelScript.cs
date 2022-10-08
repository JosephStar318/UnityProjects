using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScript : MonoBehaviour
{
   [SerializeField] private LevelData levelData;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        GetComponentInChildren<PitScript>().SetTargetScore(levelData.levelTargetScore);
    }
    private void FixedUpdate()
    {
        if(GameManager.Instance.isFinished == false)
        {
            rb.MovePosition(new Vector3(rb.position.x, rb.position.y, rb.position.z - levelData.levelSpeed));
        }
    }
    public LevelData GetLevelData()
    {
        return levelData;
    }
    public void SetLevelData(int id, int levelTargetScore, float levelSpeed)
    {
        levelData.levelID = id;
        levelData.levelTargetScore = levelTargetScore;
        levelData.levelSpeed = levelSpeed;
        GetComponentInChildren<PitScript>().SetTargetScore(levelTargetScore);
    }
}

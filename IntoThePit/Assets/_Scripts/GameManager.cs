using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int currentLevel;

    public List<GameObject> levelPrefabs;
    public List<GameObject> levelList = new List<GameObject>();
    public GameObject lastLevel;
    public int CurrentLevel
    {
        get => currentLevel;
        set
        {
            currentLevel = value;
            LoadRandomLevel();
        }
    }

    private void OnEnable() => PitScript.OnLevelPassed += OnLevelPassed;
    private void OnDisable() => PitScript.OnLevelPassed -= OnLevelPassed;

    void Start()
    {
        currentLevel = 1;
    }
    void Update()
    {
        //clear if there is certain amount of level reached
        if(levelList.Count>10)
        {
            Destroy(levelList[0]);
            levelList.RemoveAt(0);
        }
    }
    private void OnLevelPassed()
    {
        CurrentLevel++;
    }
    private void LoadRandomLevel()
    {
        int randomIndex = Random.Range(0, levelPrefabs.Count);
        Vector3 spawnPos = lastLevel.transform.GetChild(0).position;
        Debug.Log(spawnPos);
        lastLevel = Instantiate(levelPrefabs[randomIndex], spawnPos, Quaternion.identity);
        levelList.Add(lastLevel);
    }
}

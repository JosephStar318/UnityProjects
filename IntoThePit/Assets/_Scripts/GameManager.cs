using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private int currentLevel;

    public List<GameObject> levelPrefabs;
    public List<GameObject> levelList = new List<GameObject>();
    public GameObject lastLevel;
    public bool isFinished;
    public bool isLevelPassed;
    public int CurrentLevel
    {
        get => currentLevel;
        set
        {
            currentLevel = value;
            LoadRandomLevel();
        }
    }
    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        PitScript.OnLevelPassed += OnLevelPassed;
        CollectorScript.OnFinished += OnLevelFinished;
    }
    private void OnDisable()
    {
        PitScript.OnLevelPassed -= OnLevelPassed;
        CollectorScript.OnFinished -= OnLevelFinished;
    }

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
        isLevelPassed = true;
        CurrentLevel++;
    }
    private void OnLevelFinished(GameObject level)
    {
        isFinished = true;
        StartCoroutine(WaitForObjectsToFall());
    }

    private IEnumerator WaitForObjectsToFall()
    {
        yield return new WaitForSeconds(3);
        if(isLevelPassed == true)
        {
            isLevelPassed = false;
            isFinished = false;
            UpdateHUD();
        }
        else
        {
            GameOverScreen();
        }

    }

    private void UpdateHUD()
    {
        //Change the level counter on the screen

    }

    private void GameOverScreen()
    {
        //show game over screen

    }

    private void LoadRandomLevel()
    {
        int randomIndex = Random.Range(0, levelPrefabs.Count);
        Vector3 spawnPos = lastLevel.transform.GetChild(0).position;
        lastLevel = Instantiate(levelPrefabs[randomIndex], spawnPos, Quaternion.identity);
        levelList.Add(lastLevel);
    }
}

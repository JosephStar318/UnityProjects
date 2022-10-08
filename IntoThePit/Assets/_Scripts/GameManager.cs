using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private List<LevelData> levelDataList = new List<LevelData>();
    
    private AudioSource audioSource;
    public AudioClip checkpointSound;

    [SerializeField] private List<GameObject> levelList = new List<GameObject>();
    [SerializeField] private int currentLevelCount;
    [SerializeField] private List<GameObject> levelPrefabs;
    private GameObject lastLevel;

    public GameObject startScreen;
    public GameObject pauseScreen;
    public GameObject endScreen;
    public GameObject HUD;
    public GameObject collector;

    public float objectTimer;

    public bool isFinished;
    public bool isLevelPassed;
    public bool isGamePaused = true;
    public bool isGameStarted;
    public int CurrentLevelCount
    {
        get => currentLevelCount;
        set
        {
            currentLevelCount = value;
            CreateRandomLevel();
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
        audioSource = GetComponent<AudioSource>();
        InitializeGameLevels();
        if (int.TryParse(FileHandler.ReadFile("current_level"),out int result))
        {
            for (int i = 1; i <= result; i++)
            {
                CurrentLevelCount = result;
            }
        }
    }
    void Update()
    {
        //Wait for user to tap to screen
        if (!isGameStarted && Input.touchCount == 1)
        {
            isGameStarted = true;
            startScreen.SetActive(false);
            HUD.SetActive(true);
            ReplayLevel();
        }
        //clear levels if there is certain amount of level reached
        if (levelList.Count>15)
        {
            Destroy(levelList[0]);
            levelList.RemoveAt(0);
        }
        //wait for objects to fall into the pit
        if (isFinished && !isGamePaused)
        {
            objectTimer += Time.deltaTime;
            if(objectTimer > 2)
            {
                if (isLevelPassed == true)
                {
                    isLevelPassed = false;
                    isGamePaused = true;
                    pauseScreen.SetActive(true);
                    HUDAnimation();
                }
                else
                {
                    endScreen.SetActive(true);
                    isGamePaused = true;
                }
            }
        }
    }
    private void OnLevelPassed()
    {
        audioSource.PlayOneShot(checkpointSound);
        collector.GetComponentInChildren<Animator>().CrossFade("Flip", 0);
        isLevelPassed = true;
        CurrentLevelCount++;
    }
    private void OnLevelFinished()
    {
        objectTimer = 0;
        isFinished = true;
    }

    private void HUDAnimation()
    {
        HUD.GetComponent<Animation>().Play();
    }

    private void CreateRandomLevel()
    {
        int randomIndex = Random.Range(0, levelPrefabs.Count);
        Vector3 spawnPos = lastLevel.transform.GetChild(0).position;
        lastLevel = Instantiate(levelPrefabs[randomIndex], spawnPos, Quaternion.identity);
        levelList.Add(lastLevel);
        lastLevel.GetComponent<LevelScript>().SetLevelData(randomIndex, 4, 0.3f);

        SaveGameProgress();
    }
    private void CreateLevels()
    {
        foreach (GameObject level in levelList)
        {
            Destroy(level.gameObject);
        }
        levelList.Clear();
        Vector3 spawnPos = new Vector3(0f, 0f, 0f);
        for (int i = 0; i < levelDataList.Count; i++)
        {
            lastLevel = Instantiate(levelPrefabs[levelDataList[i].levelID], spawnPos, Quaternion.identity);
            //set level data from saved levels
            lastLevel.GetComponent<LevelScript>().SetLevelData(levelDataList[i].levelID, levelDataList[i].levelTargetScore, levelDataList[i].levelSpeed);
            //child is end position game object
            spawnPos = lastLevel.transform.GetChild(0).position;
            levelList.Add(lastLevel);
        }
    }

    private void LoadCheckpoint(int levelIndex)
    {
        levelDataList = FileHandler.ReadJson<LevelData>("level_data");
        CreateLevels();
        //move collector to current unfinished level
        MoveCollector(levelList[levelIndex - 1].transform.position);
        endScreen.SetActive(false);
        HUD.SetActive(true);
        isFinished = false;
        isGamePaused = false;
    }
    private void LoadNewGame()
    {
        CreateLevels();
        //move collector to current unfinished level
        MoveCollector(levelList[0].transform.position);
        currentLevelCount = 1;
        endScreen.SetActive(false);
        HUD.SetActive(true);
        isFinished = false;
        isGamePaused = false;
    }

    private void MoveCollector(Vector3 spawnPos)
    {
        collector.transform.position = spawnPos + new Vector3(0f,0.5f,5f);
    }

    private void InitializeGameLevels()
    {
        currentLevelCount = 1;
        lastLevel = levelList[levelList.Count - 1];
        foreach (GameObject obj in levelList)
        {
            levelDataList.Add(obj.GetComponent<LevelScript>().GetLevelData());
        }
        if (levelDataList.Count > 10)
        {
            // only keep first 10 element
            levelDataList.RemoveRange(10, levelDataList.Count - 10);
        }
        FileHandler.WriteJson<LevelData>("level_data", levelDataList);
    }
    private void SaveGameProgress()
    {
        levelDataList.Add(lastLevel.GetComponent<LevelScript>().GetLevelData());
        FileHandler.WriteJson<LevelData>("level_data", levelDataList);
        FileHandler.WriteFile("current_level", currentLevelCount.ToString());
    }

    public void NewGame()
    {
        LoadNewGame();
        HUD.GetComponent<HUD>().UpdateLevelIndicator();
    }

    public void ContinueGame()
    {
        isFinished = false;
        isGamePaused = false;
        pauseScreen.SetActive(false);
    }

    public void ReplayLevel()
    {
        if (currentLevelCount > 1)
        {
            LoadCheckpoint(currentLevelCount);
        }
        else
        {
            NewGame();
        }
    }


    public void ExitGame()
    {
        Application.Quit();
    }
}

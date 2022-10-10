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
    public float speedModifier;

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
        LevelScript.OnLevelDestroyed += OnLevelDestroyed;
    }

    private void OnDisable()
    {
        PitScript.OnLevelPassed -= OnLevelPassed;
        CollectorScript.OnFinished -= OnLevelFinished;
        LevelScript.OnLevelDestroyed -= OnLevelDestroyed;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        speedModifier = 0.02f;
        if (int.TryParse(FileHandler.ReadFile("current_level"),out int result))
        {
            currentLevelCount = result;
        }
        else
        {
            currentLevelCount = 1;
        }
        InitializeGameLevels();
    }
    void Update()
    {
        //Wait for user to tap to screen
        if (!isGameStarted && Input.touchCount == 1)
        {
            isGameStarted = true;
            startScreen.SetActive(false);
            ContinueGame();
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

    private void OnLevelDestroyed()
    {
        levelList.RemoveAt(0);
        levelDataList.RemoveAt(0);
        FileHandler.WriteJson<LevelData>("level_data", levelDataList);
        FileHandler.WriteFile("current_level", currentLevelCount.ToString());
    }
    private void InitializeGameLevels()
    {
        lastLevel = levelList[levelList.Count - 1];
        if (currentLevelCount > 1)  LoadCheckpoint();
        else                        LoadNewGame();
    }

    private void CreateRandomLevel()
    {
        int randomIndex = Random.Range(0, levelPrefabs.Count);
        Vector3 spawnPos = lastLevel.transform.GetChild(0).position;
        float lastLevelSpeed = lastLevel.GetComponent<LevelScript>().GetLevelSpeed();

        lastLevel = Instantiate(levelPrefabs[randomIndex], spawnPos, Quaternion.identity);
        lastLevel.GetComponent<LevelScript>().SetLevelSpeed(lastLevelSpeed);
        levelList.Add(lastLevel);

        SaveGameProgress();
    }

    //creates levels based on level data list
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

    private void SaveGameProgress()
    {
        levelDataList.Add(lastLevel.GetComponent<LevelScript>().GetLevelData());
        FileHandler.WriteJson<LevelData>("level_data", levelDataList);
        FileHandler.WriteFile("current_level", currentLevelCount.ToString());
    }

    public void LoadNewGame()
    {
        foreach (GameObject obj in levelList)
        {
            levelDataList.Add(obj.GetComponent<LevelScript>().GetLevelData());
        }
        FileHandler.WriteJson<LevelData>("level_data", levelDataList);
        CreateLevels();
    }
    private void LoadCheckpoint()
    {
        levelDataList = FileHandler.ReadJson<LevelData>("level_data");
        CreateLevels();
    }

    public void ContinueButton()
    {
        if (currentLevelCount > 10)
        {
            for (int i = 0; i < levelDataList.Count; i++)
            {
                levelDataList[i] = levelList[i].GetComponent<LevelScript>().IncreaseLevelSpeed(speedModifier);
            }
        }
        ContinueGame();
    }
    public void ContinueGame()
    {
        endScreen.SetActive(false);
        pauseScreen.SetActive(false);
        HUD.SetActive(true);
        isFinished = false;
        isGamePaused = false;
    }

    public void ReplayLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RestartGame()
    {
        currentLevelCount = 1;
        FileHandler.WriteFile("current_level", currentLevelCount.ToString());
        FileHandler.WriteJson<LevelData>("level_data", levelDataList);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private void HUDAnimation()
    {
        HUD.GetComponent<Animation>().Play();
    }
    private void OnApplicationQuit()
    {
        //if player quits the game after passed the level
        if(pauseScreen.activeInHierarchy)
        {
            levelDataList.RemoveAt(0);
            FileHandler.WriteJson<LevelData>("level_data", levelDataList);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

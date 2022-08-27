using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
public class LevelController : MonoBehaviour
{
    [Header("Level Parameters")]
    public float fallHeightLimit;
    public float maxLevelHight;
    public GameObject player;

    private NavigationManager navigationManager = new NavigationManager();
    private string activeCheckPoint;
    private string nextCheckPoint;
    private int stageLevel = 1;
    private bool isGameWin = false;
    public static bool isPaused = false;
    private MyCharacterController playerController;
    [SerializeField] public GameObject pauseMenu;
    [SerializeField] public GameObject checkpointNotification;
    [SerializeField] public Light checkpointLight;

    // Start is called before the first frame update
    public void Start()
    {
        playerController = player.GetComponent<MyCharacterController>();
        navigationManager.AddWayPoint("Start", player.transform.position);
        navigationManager.AddWayPoint("Stage1", new Vector3(0.19f, 1.5f, 23.33f));
        navigationManager.AddWayPoint("Stage2", new Vector3(20.75f, 7.4f ,20.87f));
        navigationManager.AddWayPoint("Stage3", new Vector3(20.32f, 16.07f, -3.77f));
        navigationManager.AddWayPoint("Stage4", new Vector3(20.48f, 19.61f, -38.45f));
        activeCheckPoint = "Start";
        nextCheckPoint = "Stage" + stageLevel++;
        checkpointLight.transform.position = navigationManager.GetWaypointLocation(nextCheckPoint) + new Vector3(0,10,0);
        Cursor.visible = false;
    }
    

    // Update is called once per frame
    public void Update()
    {
        FallDetect();
        CheckPointCheck();
        UserPrompt();
        if (isGameWin == true)
        {
            //do something
        }
    }
    public void UserPrompt()
    {
        
        //Pause menu
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(!isPaused)
            {
                Time.timeScale = 0f;
                playerController.enabled = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                pauseMenu.SetActive(true);
                isPaused = true;
            }
            else
            {
                Time.timeScale = 1f;
                playerController.enabled = true;
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = false;
                pauseMenu.SetActive(false);
                isPaused = false;
            }
        }
        //

    }
    private void FallDetect()
    {
        if(player.transform.position.y < fallHeightLimit)
        {
            navigationManager.Teleport(player, activeCheckPoint);
        }
    }
    private void CheckPointCheck()
    {
        Vector3 waypointPosition = navigationManager.GetWaypointLocation(nextCheckPoint);
        Vector3 boxSize = new Vector3(2, 2, 2);
        
        if (Physics.CheckBox(waypointPosition,boxSize,transform.rotation,LayerMask.GetMask("Player")) == true)
        {
            
            activeCheckPoint = nextCheckPoint;
            nextCheckPoint = "Stage" + stageLevel++;
            ShowNotification();
            
            Debug.Log("Waypoint set");
            if (nextCheckPoint == "Stage5")
            {
                isGameWin = true;
            }
            else
            {
                checkpointLight.transform.position = navigationManager.GetWaypointLocation(nextCheckPoint) + new Vector3(0, 10, 0);
            }
        }
        
    }
    private void ShowNotification()
    {
        checkpointNotification.SetActive(true);
        Invoke(nameof(HideNotification), 5);
    }
    private void HideNotification()
    {
        checkpointNotification.SetActive(false);
    }
}

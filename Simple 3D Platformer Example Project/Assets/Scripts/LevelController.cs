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
    [SerializeField] public GameObject pauseMenu;
    // Start is called before the first frame update
    public void Start()
    {
        navigationManager.AddWayPoint("Start", player.transform.position);
        navigationManager.AddWayPoint("Stage1", new Vector3(0.3138357f, 1.5f, 22.97127f));
        //navigationManager.AddWayPoint("Stage1", player.transform.position);
        //navigationManager.AddWayPoint("Stage2", player.transform.position);
        //navigationManager.AddWayPoint("Stage3", player.transform.position);
        //navigationManager.AddWayPoint("Stage4", player.transform.position);
        activeCheckPoint = "Start";
        nextCheckPoint = "Stage" + stageLevel++;

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
                player.GetComponent<MyCharacterController>().enabled = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                pauseMenu.SetActive(true);
                isPaused = true;
            }
            else
            {
                Time.timeScale = 1f;
                player.GetComponent<MyCharacterController>().enabled = true;
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
        if (player.transform.position.y < fallHeightLimit)
        {
            navigationManager.Teleport(player, activeCheckPoint);
        }
    }
    private void CheckPointCheck()
    {
        Vector3 waypointPosition = navigationManager.GetWaypointLocation(nextCheckPoint);
        Vector3 boxSize = new Vector3(5, 5, 5);
        
        if (Physics.CheckBox(waypointPosition,boxSize,transform.rotation,LayerMask.GetMask("Player")) == true)
        {
            activeCheckPoint = nextCheckPoint;
            nextCheckPoint = "Stage" + stageLevel++;
            Debug.Log("Waypoint set");
            if (nextCheckPoint == "Stage4")
            {
                isGameWin = true;
            }
        }
        
    }
}

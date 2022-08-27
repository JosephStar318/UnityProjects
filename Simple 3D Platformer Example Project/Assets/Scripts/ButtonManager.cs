using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.UI;
public class ButtonManager : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        if(LevelController.isPaused)
        {
            UnPause();
        }
        SceneManager.LoadScene(sceneName);
    }
    public void QuitGame()
    {
        //todo prompt will be added
        Application.Quit();
    }
    public void Update()
    {
        
    }
    public void UnPause()
    {
        Time.timeScale = 1f;
        GameObject.FindGameObjectWithTag("Player").GetComponent<MyCharacterController>().enabled = true;
        GameObject.Find("Pause Menu").SetActive(false);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        LevelController.isPaused = false;

    }
    public void ChangeFOV(Camera camera)
    {
        camera.fieldOfView = GameObject.Find("FOVBar").GetComponent<Slider>().value;
        GameObject.FindGameObjectWithTag("Player").GetComponent<MyCharacterController>().defaultFOV = camera.fieldOfView;
    }
}

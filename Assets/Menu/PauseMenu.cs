using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
    
    public bool gameIsPaused = false, settingsMenuIsOn = false, controlsMenuIsOn = false;
    public GameObject pauseMenuUI, settingsMenuUI, controlsMenuUI, zeph;
    
    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
        settingsMenuUI.SetActive(settingsMenuIsOn);
        controlsMenuUI.SetActive(controlsMenuIsOn);
    }

    public void Pause()
    {
        gameIsPaused = true;
        pauseMenuUI.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        zeph.GetComponent<Zeph>().enabled = false;
    }

    public void Resume()
    {
        gameIsPaused = false;
        pauseMenuUI.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        zeph.GetComponent<Zeph>().enabled = true;
    }

    public void LoadMenu()
    {
        Debug.Log("à faire");
        //SceneManager.LoadScene("MainMenu");
    }

    public void LoadControls()
    {
        controlsMenuIsOn = !controlsMenuIsOn;
        if (settingsMenuIsOn)
        {
            controlsMenuIsOn = false;
        }
    }


    public void LoadSettings()
    {
        settingsMenuIsOn = !settingsMenuIsOn;
        if (controlsMenuIsOn)
        {
            settingsMenuIsOn = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WelcomeMenu : MonoBehaviour {
    public bool creditsMenuIsOn = false;
    public GameObject welcomeMenuUI;
    public GameObject creditsMenuUI;
    public GameObject welcomeCam;
    public GameObject zephyr;
    public GameObject zephyrIdle;

    private void FixedUpdate () {
        creditsMenuUI.SetActive(creditsMenuIsOn);
    }

    public void Play()
    {
        StartCoroutine(PlayEnum());
    }

    private IEnumerator PlayEnum()
    {
        welcomeCam.GetComponent<Animator>().enabled = true;
        welcomeMenuUI.SetActive(false);
        yield return new WaitForSeconds(10);
        welcomeCam.SetActive(false);
        zephyrIdle.SetActive(false);
        zephyr.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void LoadCredits()
    {
        creditsMenuIsOn = !creditsMenuIsOn;
    }
}

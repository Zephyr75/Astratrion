using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {
    public GameObject music, sfx;
    private AudioSource[] musicTab, sfxTab;
    public Slider musicSlider,sfxSlider;

    private void Start()
    {
        musicTab = music.GetComponentsInChildren<AudioSource>();
        sfxTab = sfx.GetComponentsInChildren<AudioSource>();
    }

    public void Music()
    {
        foreach (AudioSource source in musicTab)
        {
            source.volume = musicSlider.value;
        }
    }

    public void Sfx()
    {
        foreach (AudioSource source in sfxTab)
        {
            source.volume = sfxSlider.value;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionsScript : MonoBehaviour
{
    public Slider Music;
    public Slider SFX;
    public AudioMixer MusicMixer;
    public AudioMixer SFXMixer;

    // Start is called before the first frame update
    void Start()
    {
        Music.value = SaveScript.MusicVol;
        SFX.value = SaveScript.SFXVol;
        MusicMixer.SetFloat("MusicLevel", SaveScript.MusicVol);
        SFXMixer.SetFloat("SFXLevel", SaveScript.SFXVol);
    }

    public void DifficultyEasy()
    {
        SaveScript.DifficultyAmt = 3.0f;
    }

    public void DifficultyStandard()
    {
        SaveScript.DifficultyAmt = 2.0f;
    }

    public void DifficultyExpert()
    {
        SaveScript.DifficultyAmt = 1.0f;
    }

    public void MusicVolume()
    {
        MusicMixer.SetFloat("MusicLevel", Music.value);
        SaveScript.MusicVol = Music.value;
    }

    public void SFXVolume()
    {
        SFXMixer.SetFloat("SFXLevel", SFX.value);
        SaveScript.SFXVol = SFX.value;
    }
}

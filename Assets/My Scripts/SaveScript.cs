using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SaveScript : MonoBehaviour
{
    public static float Player1Health = 1.0f;
    public static float Player2Health = 1.0f;
    public static float Player1Timer = 2.0f;
    public static float Player2Timer = 2.0f;
    public static bool TimeOut = false;
    public static bool Player1Mode = true;
    public static int Player1Wins = 0;
    public static int Player2Wins = 0;
    public static int Round = 0;
    public static string P1Select;
    public static string P2Select;
    public static GameObject Player1Load;
    public static GameObject Player2Load;
    public static int LevelNumber = 1;
    public static float DifficultyAmt = 1.0f;
    public static bool P1Reacting = false;
    public static bool P2Reacting = false;
    private int FrameRateTarget = 120;
    public AudioMixer MusicMixer;
    public AudioMixer SFXMixer;
    public static float MusicVol = 0;
    public static float SFXVol = 2;


    // Start is called before the first frame update
    void Start()
    {
        Player1Health = 1.0f;
        Player2Health = 1.0f;
        P1Reacting = false;
        P2Reacting = false;

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = FrameRateTarget;

        MusicMixer.SetFloat("MusicLevel", MusicVol);
        SFXMixer.SetFloat("SFXLevel", SFXVol);

    }

    // Update is called once per frame
    void Update()
    {
        if(Application.targetFrameRate != FrameRateTarget)
        {
            Application.targetFrameRate = FrameRateTarget;
        }
        Debug.Log("Music Volume = " + MusicVol);
        Debug.Log("SFX Volume = " + SFXVol);
    }
}

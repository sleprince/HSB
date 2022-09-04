using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseWin : MonoBehaviour
{
    public GameObject WinText;
    public GameObject LoseText;
    public GameObject Player1WinText;
    public GameObject Player2WinText;
    public AudioSource MyPlayer;
    public AudioClip LoseAudio;
    public AudioClip Player1WindAudio;
    public AudioClip Player2WindAudio;
    public float PauseTime = 1.0f;

    private int Scene = 3;

    // Start is called before the first frame update
    void Start()
    {
        SaveScript.TimeOut = false;
        WinText.gameObject.SetActive(false);
        LoseText.gameObject.SetActive(false);
        Player1WinText.gameObject.SetActive(false);
        Player2WinText.gameObject.SetActive(false);
        StartCoroutine(WinSet());
    }

    IEnumerator WinSet()
    {
        yield return new WaitForSeconds(0.4f);
        if(SaveScript.Player1Health > SaveScript.Player2Health)
        {
            if (SaveScript.Player1Mode == true)
            {
                WinText.gameObject.SetActive(true);
                MyPlayer.Play();
                SaveScript.Player1Wins++;
            }
            else if (SaveScript.Player1Mode == false)
            {
                Player1WinText.gameObject.SetActive(true);
                MyPlayer.clip = Player1WindAudio;
                MyPlayer.Play();
                SaveScript.Player1Wins++;
            }
        }
        else if (SaveScript.Player2Health > SaveScript.Player1Health)
        {
            if (SaveScript.Player1Mode == true)
            {
                LoseText.gameObject.SetActive(true);
                MyPlayer.clip = LoseAudio;
                MyPlayer.Play();
                SaveScript.Player2Wins++;
            }
            else if (SaveScript.Player1Mode == false)
            {
                Player2WinText.gameObject.SetActive(true);
                MyPlayer.clip = Player2WindAudio;
                MyPlayer.Play();
                SaveScript.Player2Wins++;
            }
        }
        yield return new WaitForSeconds(PauseTime);
        SceneManager.LoadScene(Scene);


    }

}

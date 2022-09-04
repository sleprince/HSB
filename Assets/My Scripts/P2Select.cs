using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class P2Select : MonoBehaviour
{
    public int MaxIcons = 6;
    public int IconsPerRow = 3;
    public int MaxRows = 2;

    public GameObject EveP2;
    public GameObject MorakP2;
    public GameObject MariaP2;
    public GameObject ElyP2;
    public GameObject SynthP2;
    public GameObject VanguardP2;

    public GameObject EveP2Text;
    public GameObject MorakP2Text;
    public GameObject MariaP2Text;
    public GameObject ElyP2Text;
    public GameObject SynthP2Text;
    public GameObject VanguardP2Text;

    public TextMeshProUGUI Player2Name;

    public string CharacterSelectionP2;

    private int IconNumber = 1;
    private int RowNumber = 1;
    private float PauseTime = 1.0f;
    public float TimerMax = 0.6f;
    private bool TimeCountDown = false;
    private bool ChangeCharacter = false;
    private AudioSource MyPlayer;

    public int Scene = 1;

    // Start is called before the first frame update
    void Start()
    {
        ChangeCharacter = true;
        Time.timeScale = 0.6f;
        MyPlayer = GetComponent<AudioSource>();
        PauseTime = TimerMax;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(SaveScript.P1Select);
        if (ChangeCharacter == true)
        {
            if (IconNumber == 1)
            {
                SwitchOff();
                EveP2.gameObject.SetActive(true);
                EveP2Text.gameObject.SetActive(true);
                Player2Name.text = "Eve";
                CharacterSelectionP2 = "EveP2";
                ChangeCharacter = false;
            }
            if (IconNumber == 2)
            {
                SwitchOff();
                MorakP2.gameObject.SetActive(true);
                MorakP2Text.gameObject.SetActive(true);
                Player2Name.text = "Morak";
                CharacterSelectionP2 = "MorakP2";
                ChangeCharacter = false;
            }
            if (IconNumber == 3)
            {
                SwitchOff();
                MariaP2.gameObject.SetActive(true);
                MariaP2Text.gameObject.SetActive(true);
                Player2Name.text = "Maria";
                CharacterSelectionP2 = "MariaP2";
                ChangeCharacter = false;
            }
            if (IconNumber == 4)
            {
                SwitchOff();
                ElyP2.gameObject.SetActive(true);
                ElyP2Text.gameObject.SetActive(true);
                Player2Name.text = "Ely";
                CharacterSelectionP2 = "ElyP2";
                ChangeCharacter = false;
            }
            if (IconNumber == 5)
            {
                SwitchOff();
                SynthP2.gameObject.SetActive(true);
                SynthP2Text.gameObject.SetActive(true);
                Player2Name.text = "Synth";
                CharacterSelectionP2 = "SynthP2";
                ChangeCharacter = false;
            }
            if (IconNumber == 6)
            {
                SwitchOff();
                VanguardP2.gameObject.SetActive(true);
                VanguardP2Text.gameObject.SetActive(true);
                Player2Name.text = "Vanguard";
                CharacterSelectionP2 = "VanguardP2";
                ChangeCharacter = false;
            }
        }

        if(Input.GetButtonDown("Fire1P2"))
        {
            SaveScript.P2Select = CharacterSelectionP2;
            MyPlayer.Play();
            SceneManager.LoadScene(Scene);
        }

        if(TimeCountDown == true)
        {
            if(PauseTime > 0.1f)
            {
                PauseTime -= 1.0f * Time.deltaTime;
            }
            if(PauseTime <= 0.1f)
            {
                PauseTime = TimerMax;
                TimeCountDown = false;
            }
        }

        if(Input.GetAxis("HorizontalP2") > 0)
        {
            if(PauseTime == TimerMax)
            {
                if(IconNumber < IconsPerRow * RowNumber)
                {
                    IconNumber++;
                    ChangeCharacter = true;
                    TimeCountDown = true;
                }
            }
        }
        if (Input.GetAxis("HorizontalP2") < 0)
        {
            if (PauseTime == TimerMax)
            {
                if (IconNumber > IconsPerRow * (RowNumber - 1) +1)
                {
                    IconNumber--;
                    ChangeCharacter = true;
                    TimeCountDown = true;
                }
            }
        }
        if (Input.GetAxis("VerticalP2") < 0)
        {
            if (PauseTime == TimerMax)
            {
                if (RowNumber < MaxRows)
                {
                    IconNumber += IconsPerRow;
                    RowNumber++;
                    ChangeCharacter = true;
                    TimeCountDown = true;
                }
            }
        }
        if (Input.GetAxis("VerticalP2") > 0)
        {
            if (PauseTime == TimerMax)
            {
                if (RowNumber > 1)
                {
                    IconNumber -= IconsPerRow;
                    RowNumber--;
                    ChangeCharacter = true;
                    TimeCountDown = true;
                }
            }
        }
    }

    void SwitchOff()
    {
        EveP2.gameObject.SetActive(false);
        MorakP2.gameObject.SetActive(false);
        MariaP2.gameObject.SetActive(false);
        ElyP2.gameObject.SetActive(false);
        SynthP2.gameObject.SetActive(false);
        VanguardP2.gameObject.SetActive(false);
        EveP2Text.gameObject.SetActive(false);
        MorakP2Text.gameObject.SetActive(false);
        MariaP2Text.gameObject.SetActive(false);
        ElyP2Text.gameObject.SetActive(false);
        SynthP2Text.gameObject.SetActive(false);
        VanguardP2Text.gameObject.SetActive(false);
    }
}

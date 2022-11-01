using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Levels : MonoBehaviour {

    public bool solo;
    public int numberOfUsers;
    public List<LevelBase> players = new List<LevelBase>(); //the list with all our levels.
    
    //the list were we hold anything we need to know for each separate Level,
    //for now, it's their id and their corresponding prefab
    public List<LevelBase> LevelList = new List<LevelBase>();

    //we use this function to find Levels from their id
	public LevelBase returnLevelWithID(int id)
    {
        LevelBase retVal = null;

        for (int i = 0; i < LevelList.Count; i++)
        {
            if(string.Equals(LevelList[i].levelId,id))
            {
                retVal = LevelList[i];
                break;
            }
        }

        return retVal;
    }

    //we use this one to return the player from his created Level, states
    public LevelBase returnPlayerFromStates(StateManager states)
    {
        LevelBase retVal = null;

        for (int i = 0; i < players.Count; i++)
        {
            if(players[i].playerStates == states)
            {
                retVal = players[i];
                break;
            }
        }

        return retVal;
    }

    public LevelBase returnOppositePlater(LevelBase pl)
    {
        LevelBase retVal = null;

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] != pl)
            {
                retVal = players[i];
                break;
            }
        }

        return retVal;
    }

    public int ReturnLevelInt(GameObject prefab)
    {
        int retVal = 0;

        for (int i = 0; i < LevelList.Count; i++)
        {
            if(LevelList[i].prefab == prefab)
            {
                retVal = i;
                break;
            }
        }

        return retVal;
    }

    public static Levels instance;
    public static Levels GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

}

[System.Serializable]
public class LevelBase
{
    public int levelId;
    public GameObject prefab;
    public Sprite icon;

    public Text LevelName;
    //public AudioClip SelectSound;

    //public AudioClip[] CharSounds;




    public string playerId;
    public string inputId;
    //public PlayerType playerType;
    public bool hasCharacter;
    public GameObject playerPrefab;
    public StateManager playerStates;
    public int score;

    //for use with passing over selected CharName to the level
    public Text CharName;
    public int CharNum;
    //for use with passing over selected character's sounds to the level
    public AudioClip[] CharSounds;






    public AudioClip[] HitSounds;

    public int charId;



    public AudioClip SelectSound;


}


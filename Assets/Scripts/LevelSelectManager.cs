using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    public int numberOfPlayers = 1;
    public List<LevelInterfaces> LevInterfaces = new List<LevelInterfaces>();

    public List<PlayerInterfaces2> plInterfaces = new List<PlayerInterfaces2>();

    int maxRow;
    int maxCollumn;
    List<PotraitInfo> potraitList = new List<PotraitInfo>();

    public GameObject potraitCanvas; // the canvas that holds all the potraits

    //bool loadLevel; //if we are loading the level  
    public bool bothPlayersSelected; //whether both players chosen and the level will then be loaded.


    public static int LevelName;

    Levels levels;

    CharacterManager charManager;

    GameObject potraitPrefab;

    #region Singleton
    public static LevelSelectManager instance;
    public static LevelSelectManager GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;
    }
    #endregion

    void Start()
    {
        //we start by getting the reference to the character manager
        levels = Levels.GetInstance();
        numberOfPlayers = levels.numberOfUsers;

        potraitPrefab = Resources.Load("potraitPrefab") as GameObject;
        CreatePotraits();

       // levels.solo = (numberOfPlayers == 1);

    }

    void CreatePotraits()
    {
        GridLayoutGroup group = potraitCanvas.GetComponent<GridLayoutGroup>();

        maxRow = group.constraintCount;
        int x = 0;
        int y = 0;

        for (int i = 0; i < levels.LevelList.Count; i++)
        {
            LevelBase c = levels.LevelList[i];

            GameObject go = Instantiate(potraitPrefab) as GameObject;
            go.transform.SetParent(potraitCanvas.transform);

            PotraitInfo p = go.GetComponent<PotraitInfo>();
            p.img.sprite = c.icon;
            p.characterId = c.levelId;
            p.posX = x;
            p.posY = y;
            potraitList.Add(p);

            if (x < maxRow - 1)
            {
                x++;
            }
            else
            {
                x = 0;
                y++;
            }

            maxCollumn = y;
        }
    }

    void Update()
    {

            for (int i = 0; i < plInterfaces.Count; i++)
            {
                //bringing LevelBase info into each player interface as a new LevelBase.
                // plInterfaces[i].LevelBase = levels.LevelList[i];

       


                        //bringing playerbase info into each player interface as a new playerbase.
                       // plInterfaces[i].playerBase = charManager.players[i];


                        //plInterfaces[i].LevelBase.CharNum = null;
                        //plInterfaces[i].CharNum = null;

                        HandleSelectorPosition(plInterfaces[i]);
                        HandleSelectScreenInput(plInterfaces[i], levels.players[i].inputId);
                        HandleCharacterPreview(plInterfaces[0]);

                    //  }

                        //trying to play chosen character sound.
                        //plInterfaces[i].SelectSound.play();
                        //charManager.characterList.SelectSound.play();


           }



        if (bothPlayersSelected)
        {
            Debug.Log("loading");
            StartCoroutine("LoadLevel"); //and start the coroutine to load the level
            //loadLevel = true;
            bothPlayersSelected = false;
        }
        else
        {
            if (Input.GetButtonDown("A") || Input.GetButtonDown("A1"))
            {
                bothPlayersSelected = true;
                LoadLevel();
            }

        }
    }

    void HandleSelectScreenInput(PlayerInterfaces2 pl, string playerId)
    {
        #region Grid Navigation

        /*To navigate in the grid
         * we simply change the active x and y to select what entry is active
         * we also smooth out the input so if the user keeps pressing the button
         * it won't switch more than once over half a second
         */

        float vertical = Input.GetAxis("Vertical" + playerId);

        if (vertical != 0)
        {
            if (!pl.hitInputOnce)
            {
                if (vertical > 0)
                {
                    pl.activeY = (pl.activeY > 0) ? pl.activeY - 1 : maxCollumn;
                }
                else
                {
                    pl.activeY = (pl.activeY < maxCollumn) ? pl.activeY + 1 : 0;
                }

                pl.hitInputOnce = true;
            }
        }

        float horizontal = Input.GetAxis("Horizontal" + playerId);

        if (horizontal != 0)
        {
            if (!pl.hitInputOnce)
            {
                if (horizontal > 0)
                {
                    pl.activeX = (pl.activeX > 0) ? pl.activeX - 1 : maxRow - 1;
                }
                else
                {
                    pl.activeX = (pl.activeX < maxRow - 1) ? pl.activeX + 1 : 0;
                }

                pl.timerToReset = 0;
                pl.hitInputOnce = true;
            }
        }

        if (vertical == 0 && horizontal == 0)
        {
            pl.hitInputOnce = false;
        }

        if (pl.hitInputOnce)
        {
            pl.timerToReset += Time.deltaTime;

            if (pl.timerToReset > 0.8f)
            {
                pl.hitInputOnce = false;
                pl.timerToReset = 0;
            }
        }

        #endregion


        levels.players[0].hasCharacter = true;
    }
    

    void LoadLevel()
    {

        MySceneManager.GetInstance().RequestLevelLoad(SceneType.prog, "level_1");

    }

    void HandleSelectorPosition(PlayerInterfaces2 pl)
    {
        pl.selector.SetActive(true); //enable the selector

        PotraitInfo pi = ReturnPotrait(pl.activeX, pl.activeY);//

        if (pi != null)
        {
            pl.activePotrait = pi; //find the active potrait

            //bringing the select sound for which character is being hovered over to the player interface.
           // pl.SelectSound = CharacterManager.GetInstance().returnCharacterWithID(pi.characterId).SelectSound;

            //and place the selector over it's position
            Vector2 selectorPosition = pl.activePotrait.transform.localPosition;

            selectorPosition = selectorPosition + new Vector2(potraitCanvas.transform.localPosition.x
                , potraitCanvas.transform.localPosition.y);

            pl.selector.transform.localPosition = selectorPosition;
        }
    }

    void HandleCharacterPreview(PlayerInterfaces2 pl)
    {

        //if the previews potrait we had, is not the same as the active one we have
        //that means we changed characters
        if (pl.previewPotrait != pl.activePotrait)
        {
            if (pl.createdCharacter != null)//delete the one we have now if we do have one
            {
                Destroy(pl.createdCharacter);
            }


            //defining the half scale.
            var HalfScale = new Vector3(.25f, .25f, .25f);

            //and create another one
            GameObject go = Instantiate(
                //character gets made with this function
                levels.returnLevelWithID(pl.activePotrait.characterId).prefab,
                //where you could maybe change the scale
                pl.charVisPos.position,
                Quaternion.identity) as GameObject;

            //bringing the character ID number over from the Character list
            pl.CharNum = pl.activePotrait.characterId;
            //pl.CharName is the property we want to bring in to the level to show their name under the health bar.
            pl.CharName = CharacterManager.GetInstance().returnCharacterWithID(pl.activePotrait.characterId).CharName;



            pl.createdCharacter = go;
            //putting character back in the place where it's meant to spawn
            //go.transform.position = pl.charVisPos.position;
           go.transform.localScale = HalfScale;

            pl.previewPotrait = pl.activePotrait;
        }



    }


    PotraitInfo ReturnPotrait(int x, int y)
    {
        PotraitInfo r = null;
        for (int i = 0; i < potraitList.Count; i++)
        {
            if (potraitList[i].posX == x && potraitList[i].posY == y)
            {
                r = potraitList[i];
            }
        }

        return r;
    }

    [System.Serializable]
    public class LevelInterfaces
    {
        public PotraitInfo activePotrait; //the current active potrait for player 1
        public PotraitInfo previewPotrait;
        public GameObject selector; //the select indicator for player1
        public Transform charVisPos; //the visualization position for player 1
        public GameObject createdCharacter; // the created character for player 1

        //public AudioClip SelectSound; //select sound for the chosen character
        public int LevelNum; //character id number for the chosen character
        public Text LevelName; //character name for the chosen character
        //public AudioClip[] CharSounds; //character sounds for the chosen character


        public int activeX;//the active X and Y entries for player 1
        public int activeY;

        //variables for smoothing out input
        public bool hitInputOnce;
        public float timerToReset;

        //public PlayerBase playerBase;
        //bringing LevelBase info into each player interface as a new LevelBase.
        //public LevelBase LevelBase;

        //public LevelBase LevelBase;
    }

    [System.Serializable]
    public class PlayerInterfaces2
    {
        public PotraitInfo activePotrait; //the current active potrait for player 1
        public PotraitInfo previewPotrait;
        public GameObject selector; //the select indicator for player1
        public Transform charVisPos; //the visualization position for player 1
        public GameObject createdCharacter; // the created character for player 1

        public AudioClip SelectSound; //select sound for the chosen character
        public int CharNum; //character id number for the chosen character
        public Text CharName; //character name for the chosen character
        public AudioClip[] CharSounds; //character sounds for the chosen character


        public int activeX;//the active X and Y entries for player 1
        public int activeY;

        //variables for smoothing out input
        public bool hitInputOnce;
        public float timerToReset;

        public PlayerBase playerBase;
        //bringing LevelBase info into each player interface as a new LevelBase.
        public LevelBase LevelBase;


    }

}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SelectScreenManager : MonoBehaviour
{
    public int numberOfPlayers = 1;
    public List<PlayerInterfaces> plInterfaces = new List<PlayerInterfaces>();
    int maxRow;
    int maxCollumn;
    List<PotraitInfo> potraitList = new List<PotraitInfo>();

    public GameObject potraitCanvas; // the canvas that holds all the potraits
   
    bool loadLevel; //if we are loading the level  
    public bool bothPlayersSelected; //whether both players chosen and the level will then be loaded.


    public static int CharacterName;

    CharacterManager charManager;

    GameObject potraitPrefab;

    #region Singleton
    public static SelectScreenManager instance;
    public static SelectScreenManager GetInstance()
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
        charManager = CharacterManager.GetInstance();
        numberOfPlayers = charManager.numberOfUsers;

        potraitPrefab = Resources.Load("potraitPrefab") as GameObject;
        CreatePotraits();

        charManager.solo = (numberOfPlayers == 1);

    }

    void CreatePotraits()
    {
        GridLayoutGroup group = potraitCanvas.GetComponent<GridLayoutGroup>();

        maxRow = group.constraintCount;
        int x = 0;
        int y = 0;

        for (int i = 0; i < charManager.characterList.Count; i++)
        {
            CharacterBase c = charManager.characterList[i];

            GameObject go = Instantiate(potraitPrefab) as GameObject;
            go.transform.SetParent(potraitCanvas.transform);

            PotraitInfo p = go.GetComponent<PotraitInfo>();
            p.img.sprite = c.icon;
            p.characterId = c.charId;
            p.posX = x;
            p.posY = y;
            potraitList.Add(p);

            if(x < maxRow-1)
            {
                x++;
            }
            else
            {
                x=0;
                y++;
            }

            maxCollumn = y;
        }
    }

    void Update()
    {
        if (!loadLevel)
        {
            for (int i = 0; i < plInterfaces.Count; i++)
            {
                //bringing characterbase info into each player interface as a new characterbase.
               // plInterfaces[i].characterBase = charManager.characterList[i];

                if (i < numberOfPlayers)
                {
                    if (Input.GetButtonUp("B" + charManager.players[i].inputId))
                    {
                        plInterfaces[i].playerBase.hasCharacter = false;
                    }

                    if (!charManager.players[i].hasCharacter)
                    {
                        //bringing playerbase info into each player interface as a new playerbase.
                        plInterfaces[i].playerBase = charManager.players[i];

                        
                        //plInterfaces[i].characterBase.CharNum = null;
                        //plInterfaces[i].CharNum = null;

                        HandleSelectorPosition(plInterfaces[i]);
                        HandleSelectScreenInput(plInterfaces[i], charManager.players[i].inputId);
                        HandleCharacterPreview(plInterfaces[i]);
                    }
                }
                else
                {
                    charManager.players[i].hasCharacter = true;
                    //trying to play chosen character sound.
                    //plInterfaces[i].SelectSound.play();
                    //charManager.characterList.SelectSound.play();
                }
            }
           
        }

        if(bothPlayersSelected)
        {
            Debug.Log("loading");
            StartCoroutine("LoadLevel"); //and start the coroutine to load the level
            loadLevel = true;
            bothPlayersSelected = false;
        }
        else
        {
            if(charManager.players[0].hasCharacter 
                && charManager.players[1].hasCharacter)
            {
                bothPlayersSelected = true;
            }
           
        }
    }
  
    void HandleSelectScreenInput(PlayerInterfaces pl, string playerId)
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
                    pl.activeX = (pl.activeX > 0) ? pl.activeX - 1 : maxRow-1;
                }
                else
                {
                    pl.activeX = (pl.activeX < maxRow-1) ? pl.activeX + 1 : 0;
                }

                pl.timerToReset = 0;
                pl.hitInputOnce = true;
            }
        }
       
        if(vertical == 0 && horizontal == 0)
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

        //if the user presses space, he has selected a character
        if (Input.GetButtonUp("A" + playerId))
        {
            //make a reaction on the character, because why not
            pl.createdCharacter.GetComponentInChildren<Animator>().Play("Kick");

            //play the relevant select sound
            Completed.SoundManager.instance.RandomizeSfx(pl.SelectSound);


            //pass the character to the character manager so that we know what prefab to create in the level
            pl.playerBase.playerPrefab =
               charManager.returnCharacterWithID(pl.activePotrait.characterId).prefab;

            //pass the character name to the character manager so that we can use it in the level.
            pl.playerBase.CharName =
                charManager.returnCharacterWithID(pl.activePotrait.characterId).CharName;

            //pass the character sounds to the character manager so that we can use them in the level.
            pl.playerBase.CharSounds =
    charManager.returnCharacterWithID(pl.activePotrait.characterId).CharSounds;


            //trying to make global character identification.
            if (pl.createdCharacter.name == "Fighter Object(Clone)")
            {
                CharacterName = 1;
                //Debug.Log(CharacterName);
            }


            pl.playerBase.hasCharacter = true;
        }
    }

    IEnumerator LoadLevel()
    {
        //if any of the players is an AI, then assign a random character to the prefab
        for (int i = 0; i < charManager.players.Count; i++)
        {
            if(charManager.players[i].playerType == PlayerBase.PlayerType.ai)
            {
                if(charManager.players[i].playerPrefab == null)
                {
                    int ranValue = Random.Range(0, potraitList.Count);

                    charManager.players[i].playerPrefab = 
                        charManager.returnCharacterWithID(potraitList[ranValue].characterId).prefab;

                    Debug.Log(potraitList[ranValue].characterId);
                }
            }
        }

        yield return new WaitForSeconds(2);//after 2 seconds load the level

        if (charManager.solo)
        {
            MySceneManager.GetInstance().CreateProgression();
            MySceneManager.GetInstance().LoadNextOnProgression();
        }
        else
        {
            //here is where I should change it, to loading level select screen for 2Player mode.
            MySceneManager.GetInstance().RequestLevelLoad(SceneType.prog, "level_1");
        }

    }

    void HandleSelectorPosition(PlayerInterfaces pl)
    {
        pl.selector.SetActive(true); //enable the selector

        PotraitInfo pi = ReturnPotrait(pl.activeX, pl.activeY);//

        if (pi != null)
        {
            pl.activePotrait = pi; //find the active potrait

            //bringing the select sound for which character is being hovered over to the player interface.
            pl.SelectSound = CharacterManager.GetInstance().returnCharacterWithID(pi.characterId).SelectSound;

            //and place the selector over it's position
            Vector2 selectorPosition = pl.activePotrait.transform.localPosition;

            selectorPosition = selectorPosition + new Vector2(potraitCanvas.transform.localPosition.x
                , potraitCanvas.transform.localPosition.y);

            pl.selector.transform.localPosition = selectorPosition;
        }
    }

    void HandleCharacterPreview(PlayerInterfaces pl)
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
                CharacterManager.GetInstance().returnCharacterWithID(pl.activePotrait.characterId).prefab,
                //where you could maybe change the scale
                pl.charVisPos.position,
                Quaternion.identity) as GameObject;

            //bringing the character ID number over from the Character list
            pl.CharNum = pl.activePotrait.characterId;
            //pl.CharName is the property we want to bring in to the level to show their name under the health bar.
            pl.CharName = CharacterManager.GetInstance().returnCharacterWithID(pl.activePotrait.characterId).CharName;



            pl.createdCharacter = go;
            //putting character back in the place where it's meant to spawn
            go.transform.position = pl.charVisPos.position;
            go.transform.localScale = HalfScale;

            pl.previewPotrait = pl.activePotrait;

            if(!string.Equals(pl.playerBase.playerId, charManager.players[0].playerId))
            {
                pl.createdCharacter.GetComponent<StateManager>().lookRight = false;
            }
        }
    }


    PotraitInfo ReturnPotrait(int x, int y)
    {
        PotraitInfo r = null;
        for (int i = 0; i < potraitList.Count; i++)
        {
            if(potraitList[i].posX == x && potraitList[i].posY == y)
            {
                r = potraitList[i];
            }
        }

        return r;
    }

    [System.Serializable]
    public class PlayerInterfaces
    {
        public PotraitInfo activePotrait; //the current active potrait for player 1
        public PotraitInfo previewPotrait;
        public GameObject selector; //the select indicator for player1
        public Transform charVisPos; //the visualization position for player 1
        public GameObject createdCharacter; // the created character for player 1

        public AudioClip SelectSound; //select sound for the chosen character
        public Text CharNum; //character id number for the chosen character
        public Text CharName; //character name for the chosen character
        public AudioClipArray[] CharSounds; //character sounds for the chosen character


        public int activeX;//the active X and Y entries for player 1
        public int activeY;

        //variables for smoothing out input
        public bool hitInputOnce;
        public float timerToReset;

        public PlayerBase playerBase;
        //bringing characterbase info into each player interface as a new characterbase.
        public CharacterBase characterBase;


    }

}

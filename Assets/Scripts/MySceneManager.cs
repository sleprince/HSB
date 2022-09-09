using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour {

    public int progressionStages = 5;
    public List<string> levels = new List<string>();
    public List<MainScenes> mainScenes = new List<MainScenes>();

    bool waitToLoad;
    public int progIndex;
    public List<SoloProgression> progression = new List<SoloProgression>();

    CharacterManager chm;

    void Start()
    {
        chm = CharacterManager.GetInstance();
    }

    public void CreateProgression()
    {
        progression.Clear();
        progIndex = 0;

        List<int> usedCharacters = new List<int>();

        int playerInt = chm.ReturnCharacterInt(chm.players[0].playerPrefab);
        usedCharacters.Add(playerInt);

        if(progressionStages > chm.characterList.Count-1)
        {
            progressionStages = chm.characterList.Count - 2;
        }

        for (int i = 0; i < progressionStages; i++)
        {
            SoloProgression s = new SoloProgression();

            int levelInt = Random.Range(0, levels.Count);
            s.levelID = levels[levelInt];

            int charInt = UniqueRandomInt(usedCharacters, 0, chm.characterList.Count);
            s.charId = chm.characterList[charInt].charId;
            usedCharacters.Add(charInt);
            progression.Add(s);
        }
    }

    public void LoadNextOnProgression()
    {
        string targetId = "";
        SceneType sceneType = SceneType.prog;

        if (progIndex > progression.Count-1)
        {
            targetId = "intro";
            sceneType = SceneType.main;
        }
        else
        {
            targetId = progression[progIndex].levelID;

            chm.players[1].playerPrefab = 
                chm.returnCharacterWithID(progression[progIndex].charId).prefab;
            
            progIndex++;         
        }

        RequestLevelLoad(sceneType, targetId);
    }

    int UniqueRandomInt(List<int> l, int min, int max)
    {
        int retVal = Random.Range(min, max);

        while (l.Contains(retVal))
        {
            retVal = Random.Range(min, max);           
        }

        return retVal;
    }

    public void RequestLevelLoad(SceneType st, string level)
    {
        if (!waitToLoad)
        {
            string targetId = ""; 
            switch (st)
            {
                case SceneType.main:
                    targetId = ReturnMainScene(level).levelId;           
                    break;
                case SceneType.prog:
                    targetId = level;
                    break;
            }

            StartCoroutine(LoadScene(level));
            waitToLoad = true;
        }

    }

    IEnumerator LoadScene(string levelid)
    {
        yield return SceneManager.LoadSceneAsync(levelid, LoadSceneMode.Single);
        waitToLoad = false;
    }

    MainScenes ReturnMainScene(string level)
    {
        MainScenes r = null;

        for (int i = 0; i < mainScenes.Count; i++)
        {
            if(mainScenes[i].levelId == level)
            {
                r = mainScenes[i];
                break;
            }
        }

        return r;
    }

    public static MySceneManager instance;
    public static MySceneManager GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}


public enum SceneType
{
    main, prog
}

[System.Serializable]
public class SoloProgression
{
    public string charId;
    public string levelID;
}

[System.Serializable]
public class MainScenes
{
    public string levelId;
}

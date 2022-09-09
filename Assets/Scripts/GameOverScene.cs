using UnityEngine;
using System.Collections;

public class GameOverScene : MonoBehaviour {

    public float timer = 3;

	void Start () {
        StartCoroutine("LoadScene");
	}

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(timer);
        MySceneManager.GetInstance().RequestLevelLoad(SceneType.main, "intro");
    }
	
}

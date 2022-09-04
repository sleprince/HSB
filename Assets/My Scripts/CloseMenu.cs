using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CloseMenu : MonoBehaviour
{
    public int SceneNumber = 0;


    public void CloseWindow()
    {
        SceneManager.LoadScene(SceneNumber);
    }
}

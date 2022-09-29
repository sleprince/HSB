using UnityEngine;
using System.Collections;

// System.Serializable lets you change this class in editor
[System.Serializable]
public class AudioClipArray
{
    // your "inner" array
    public AudioClip[] clips;
}

public class MyAudioClipHolder : MonoBehaviour
{
    // the "outer" array
    public AudioClipArray[] clipArrays;
}

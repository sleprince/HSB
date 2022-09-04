using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P2JumpScript : MonoBehaviour
{
    public GameObject Player2;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("P1SpaceDetector"))
        {
            if (Player2Move.FacingLeftP2 == true)
            {
                Player2.transform.Translate(0.8f, 0, 0);
                //Debug.Log("Space Left detected");
            }
            if (Player2Move.FacingRightP2 == true)
            {
                Player2.transform.Translate(-0.8f, 0, 0);
                //Debug.Log("Space Right detected");
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P2MoveRestrict : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("P1Left"))
        {
            Player2Move.WalkRight = false;
        }
        if (other.gameObject.CompareTag("P1Right"))
        {
            Player2Move.WalkLeft = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("P1Left"))
        {
            Player2Move.WalkRight = true;
        }
        if (other.gameObject.CompareTag("P1Right"))
        {
            Player2Move.WalkLeft = true;
        }
    }

}

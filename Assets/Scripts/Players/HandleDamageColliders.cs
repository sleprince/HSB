using UnityEngine;
using System.Collections;

public class HandleDamageColliders : MonoBehaviour {

    //public DCtype dcType;

    public GameObject[] damageCollidersLeft;
    public GameObject[] damageColldersRight;

    public enum DamageType
    {
        light,
        heavy
    }

    public enum DCtype
    {
        bottom,
        up
    }

    StateManager states;

    void Start()
    {
        states = GetComponent<StateManager>();
        CloseColliders();
    }

    public void OpenCollider(DCtype type, float delay, DamageType damageType)
    {
        if (!states.lookRight)
        {
            switch (type)
            {
                case DCtype.bottom:
                    StartCoroutine(OpenCollider(damageCollidersLeft, 0, delay, damageType));
                    break;
                case DCtype.up:
                    StartCoroutine(OpenCollider(damageCollidersLeft, 1, delay, damageType));
                    break;
            }
        }
        else
        {
            switch (type)
            {
                case DCtype.bottom:
                    StartCoroutine(OpenCollider(damageColldersRight, 0, delay, damageType));
                    break;
                case DCtype.up:
                    StartCoroutine(OpenCollider(damageColldersRight, 1, delay, damageType));
                    break;
            }
        }
    }

    IEnumerator OpenCollider(GameObject[] array, int index, float delay, DamageType damageType)
    {
        yield return new WaitForSeconds(delay);
        array[index].SetActive(true);
        array[index].GetComponent<DoDamage>().damageType = damageType;
    }

    public void CloseColliders()
    {
        for (int i = 0; i < damageCollidersLeft.Length; i++)
        {
            damageCollidersLeft[i].SetActive(false);
            damageColldersRight[i].SetActive(false);
        }
    }
}

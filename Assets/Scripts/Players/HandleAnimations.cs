using UnityEngine;
using System.Collections;

public class HandleAnimations : MonoBehaviour
{

    public Animator anim;
    StateManager states;

   // public float attackRate = .3f;
    public AttacksBase[] attacks = new AttacksBase[5];

    void Start()
    {
        states = GetComponent<StateManager>();
        anim = GetComponentInChildren<Animator>();
    }

    void FixedUpdate()
    {
        states.dontMove = anim.GetBool("DontMove");

        anim.SetBool("TakesHit", states.gettingHit);
        anim.SetBool("OnAir", !states.onGround);
        anim.SetBool("Crouch", states.crouch);

        float movement = (states.lookRight) ? states.horizontal : -states.horizontal;
        anim.SetFloat("Movement", movement);

        if (states.vertical < 0)
        {
            states.crouch = true;
        }
        else
        {
            states.crouch = false;
        }

        HandleAttacks();
    }

    void HandleAttacks()
    { //change to for i = 0 to 3
        if (states.canAttack)
        {
            if (states.attack1)
            {
                attacks[0].attack = true;
                anim.SetBool("Attack1", attacks[0].attack);
            }

            if (states.attack2)
            {
                attacks[1].attack = true;
                anim.SetBool("Attack2", attacks[1].attack);
            }

            if (states.attack3)
            {
                attacks[2].attack = true;
                anim.SetBool("Attack3", attacks[2].attack);
            }

            if (states.attack4)
            {
                attacks[3].attack = true;
                anim.SetBool("Attack4", attacks[3].attack);
            }

            if (states.attack5)
            {
                attacks[4].attack = true;
                anim.SetBool("Attack5", attacks[4].attack);
            }

        }

        anim.SetBool("Attack1", attacks[0].attack);
        anim.SetBool("Attack2", attacks[1].attack);
        anim.SetBool("Attack3", attacks[2].attack);
        anim.SetBool("Attack4", attacks[3].attack);
        anim.SetBool("Attack5", attacks[4].attack);

        attacks[0].attack = false;
        attacks[1].attack = false;
        attacks[2].attack = false;
        attacks[3].attack = false;
        attacks[4].attack = false;
    }

    public void JumpAnim()
    {
        anim.SetBool("Attack1", false);
        anim.SetBool("Attack2", false);
        anim.SetBool("Attack3", false);
        anim.SetBool("Attack4", false);
        anim.SetBool("Attack5", false);
        anim.SetBool("Jump", true);
        StartCoroutine(CloseBoolInAnim("Jump"));
    }

    IEnumerator CloseBoolInAnim(string name)
    {
        yield return new WaitForSeconds(0.5f);
        anim.SetBool(name, false);
    }
}

[System.Serializable]
public class AttacksBase
{
    public bool attack;
    //public float attackTimer;
    //public int timesPressed;
}


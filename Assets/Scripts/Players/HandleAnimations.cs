using UnityEngine;
using System.Collections;

public class HandleAnimations : MonoBehaviour
{

    public Animator anim;

    public bool JustPressed;
    public bool IsJumping;

    StateManager states;
    CharacterManager charM;
    HandleMovement Move;

   // public float attackRate = .3f;
    public AttacksBase[] attacks = new AttacksBase[5];

    void Start()
    {
        states = GetComponent<StateManager>();
        anim = GetComponentInChildren<Animator>();
        charM = CharacterManager.GetInstance(); //for sounds
        Move = HandleMovement.GetInstance();
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


        anim.SetBool("Block", states.block);

        if(states.block)
        //stopping the character looking like they've been hit if blocking.
        { anim.SetBool("TakesHit", false); }
        

        HandleAttacks();


    }


    void HandleAttacks()
    { //change to for i = 0 to 3
        if (states.canAttack)
        {
            if (states.attack1)
            {


                if (!JustPressed)
                {
                    attacks[0].attack = true;
                    anim.SetBool("Attack1", attacks[0].attack);
                    //kick sfx

                    //if(!states.crouch)
                    //{
                        if (this.name == "player0") //making both characters able to make non OneShot dounds at once.
                        { Completed.SoundManager.instance.RandomizeSfx(charM.players[0].CharSounds[8]); }
                        if (this.name == "player1")
                        { Completed.SoundManager.instance.RandomizeSfx(charM.players[1].CharSounds[8]); }


                    //}


                    JustPressed = true;
                    StartCoroutine(TimeHater(0.8f));

                }
            }

            if (states.attack2)
            {



                    if (!JustPressed)
                    {
                        attacks[1].attack = true;
                        anim.SetBool("Attack2", attacks[1].attack);
                        //use if JustAnimated here for setbool to make it consistent in same way anims.


                        //kick 2

                        if (!JustPressed && !states.crouch && states.onGround)
                    { 
                                if (this.name == "player0") //making both characters able to make non OneShot dounds at once.
                                { Completed.SoundManager.instance.RandomizeSfx(charM.players[0].CharSounds[9]); }
                                if (this.name == "player1")
                                { Completed.SoundManager.instance.RandomizeSfx(charM.players[1].CharSounds[9]); }

                        }

                        JustPressed = true;
                        StartCoroutine(TimeHater(0.5f));
                    }



            }

            if (states.attack3)
            {


                if (!JustPressed)
                {
                    attacks[2].attack = true;
                    anim.SetBool("Attack3", attacks[2].attack);

                    //punch

                    if (!JustPressed && !states.crouch && states.onGround)
                    { 
                        if (this.name == "player0") //making both characters able to make non OneShot dounds at once.
                        { Completed.SoundManager.instance.RandomizeSfx(charM.players[0].CharSounds[10]); }
                        if (this.name == "player1")
                        { Completed.SoundManager.instance.RandomizeSfx(charM.players[1].CharSounds[10]); }

                    }

                    JustPressed = true;
                    StartCoroutine(TimeHater(0.5f));

                }
            }

            if (states.attack4)
            {
                attacks[3].attack = true;
                anim.SetBool("Attack4", attacks[3].attack);

                if (!AnimatorIsPlaying("Grapple"))
                {
                    //punch2

                    if (!JustPressed && !states.crouch && states.onGround)
                    {
                        if (this.name == "player0") //making both characters able to make non OneShot dounds at once.
                        { Completed.SoundManager.instance.RandomizeSfx(charM.players[0].CharSounds[11]); }
                        if (this.name == "player1")
                        { Completed.SoundManager.instance.RandomizeSfx(charM.players[1].CharSounds[11]); }
                    }

                }
            }

            if (states.attack5)
            {
                attacks[4].attack = true;
                anim.SetBool("Attack5", attacks[4].attack);

                if (!AnimatorIsPlaying("Grapple"))
                {

                    if (!JustPressed && !states.crouch && states.onGround)
                    {    //headbutt sfx
                        if (this.name == "player0") //making both characters able to make non OneShot dounds at once.
                        { Completed.SoundManager.instance.RandomizeSfx(charM.players[0].CharSounds[0]); }
                        if (this.name == "player1")
                        { Completed.SoundManager.instance.RandomizeSfx(charM.players[1].CharSounds[0]); }
                    }

                }
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

    bool AnimatorIsPlaying(string stateName) //bool function to check if the relevant animation is playing.
    {
        return anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0 && anim.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }



    public void JumpAnim()
    {
        anim.SetBool("Attack1", false);
        anim.SetBool("Attack2", false);
        anim.SetBool("Attack3", false);
        anim.SetBool("Attack4", false);
        anim.SetBool("Attack5", false);

        anim.SetBool("Jump", true);
        IsJumping = true;

        StartCoroutine(CloseBoolInAnim("Jump"));
    }

    IEnumerator CloseBoolInAnim(string name)
    {
        yield return new WaitForSeconds(0.5f);
        anim.SetBool(name, false);
    }

    IEnumerator TimeHater(float timer)
    {
        yield return new WaitForSeconds(timer);
        JustPressed = false;
    }
}

[System.Serializable]
public class AttacksBase
{
    public bool attack;
    //public float attackTimer;
    //public int timesPressed;
}


using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StateManager : MonoBehaviour {

    public int health = 100;

    public float horizontal;
    public float vertical;

    public bool block;

    public bool attack1;
    public bool attack2;
    public bool attack3;
    public bool attack4;
    public bool attack5;    

    public bool IsBlocking;    

    public bool crouch;

    public bool canAttack;
    public bool gettingHit;
    public bool currentlyAttacking;

    public bool dontMove;
    public bool onGround;
    public bool lookRight;

    public bool JustBeenHit;
    public bool IsHeavy;

    //audiosource to be able to play audio.
    //public AudioSource source;

    public Slider healthSlider;
    SpriteRenderer sRenderer;

    [HideInInspector]
    public HandleDamageColliders handleDC;
    [HideInInspector]
    public HandleAnimations handleAnim;
    [HideInInspector]
    public HandleMovement handleMovement;

    public GameObject[] movementColliders;

    //audio clip arrays. depricated
    // public AudioClip[] Billy;
    // public AudioClip[] Blazer;

    public AudioClip[] CharSounds;


    CharacterManager charManager;


    ParticleSystem blood;

    void Start()
    {
        handleDC = GetComponent<HandleDamageColliders>();
        handleAnim = GetComponent<HandleAnimations>();
        handleMovement = GetComponent<HandleMovement>();
        sRenderer = GetComponentInChildren<SpriteRenderer>();
        blood = GetComponentInChildren<ParticleSystem>();
        //source = GetComponent<AudioSource>();

        //load all relevant sfx audio clips. depricated
        //Billy = Resources.LoadAll<AudioClip>("Audio/Sounds/BillyNoMates");
        //Blazer = Resources.LoadAll<AudioClip>("Audio/Sounds/Blazer");

        charManager = CharacterManager.GetInstance();

        //doesn't do anything useful.
        var AllHits = "charManager.players[0].HitSounds[0]";

        for (int i = 1; i < 16; i++) 
        {
            AllHits = AllHits + "," + "charManager.players[0].HitSounds[" + i + "]";
        }
        Debug.Log(AllHits);

    }

	void FixedUpdate () {

        sRenderer.flipX = lookRight;

        onGround = isOnGround();

        if(healthSlider != null)
        {
            healthSlider.value =health * 0.01f;
        }

        if (health <= 0)
        {
            if (LevelManager.GetInstance().countdown)
            {
                //stop playing impact sounds

                LevelManager.GetInstance().EndTurnFunction();

                handleAnim.anim.Play("Dead");
                //add victory sounds here

                //play randomized victory sfx, if person who died is not Billy.
               // Debug.Log(handleMovement.Character.name);
               // if (handleMovement.Character.name != "{handleMovement.CharacterNames[0]}")
               // {
               //     Completed.SoundManager.instance.RandomizeSfx(Billy[12], Billy[13], Billy[14]);
               // }

               // if (handleMovement.Character.name == "{handleMovement.CharacterNames[1]}")
                //{
                   // Completed.SoundManager.instance.RandomizeSfx(Blazer[2], Blazer[3], Blazer[4]);
                //}
            }
        }
	}

    bool isOnGround()
    {
        bool retVal = false;

        LayerMask layer = ~(1 << gameObject.layer | 1 << 3);
        retVal = Physics2D.Raycast(transform.position, -Vector2.up, 0.1f, layer);

        return retVal;
    }

    public void ResetStateInputs()
    {
        horizontal = 0;
        vertical = 0;
        attack1 = false;
        attack2 = false;
        attack3 = false;
        attack4 = false;
        attack5 = false;

        block = false;

        //need to add any new attacks here too.
        crouch = false;
        gettingHit = false;
        currentlyAttacking = false;
        dontMove = false;

        JustBeenHit = false;

    }

    public void CloseMovementCollider(int index)
    {
        movementColliders[index].SetActive(false);
    }

    public void OpenMovementCollider(int index)
    {
        movementColliders[index].SetActive(true);
    }

    public void TakeDamage(int damage, HandleDamageColliders.DamageType damageType)
    {
        if (!gettingHit)
        {



                switch (damageType)
                {

                    case HandleDamageColliders.DamageType.light:
                        StartCoroutine(CloseImmortality(0.3f));

                    if (!block)
                    {
                        //play genric hit sound
                        Completed.SoundManager.instance.RandomOnly(charManager.players[0].HitSounds);

                        if (!JustBeenHit)
                        {
                            // light impact sounds here, preceed by light punch sound then short break 0.5 seconds maybe. maybe make them play only every 2 or 3 hits
                            if (this.name == "player0") //making both characters able to make non OneShot dounds at once.
                            { Completed.SoundManager.instance.RandomizeSfx(charManager.players[0].CharSounds[1], charManager.players[0].CharSounds[2]); }
                            if (this.name == "player1")
                            { Completed.SoundManager.instance.RandomizeSfx(charManager.players[1].CharSounds[1], charManager.players[1].CharSounds[2]); }
                            JustBeenHit = true;

                            //3 second pause before more sounds
                            StartCoroutine(CacophanyStopper(3));


                        }



                    }



                    break;

                case HandleDamageColliders.DamageType.heavy:
                        handleMovement.AddVelocityOnCharacter(
                            ((!lookRight) ? Vector3.right * 1 : Vector3.right * -1) + Vector3.up
                            , 0.5f
                            );
                        StartCoroutine(CloseImmortality(1));

                    if (!block)
                    { //play genric hit sound
                        Completed.SoundManager.instance.RandomOnly(charManager.players[0].HitSounds);

                        IsHeavy = true;

                        if (!JustBeenHit)
                        {

                            // heavy impact sounds here
                            if (this.name == "player0") //making both characters able to make non OneShot dounds at once.
                            { Completed.SoundManager.instance.RandomizeSfx(charManager.players[0].CharSounds[3], charManager.players[0].CharSounds[4], CharSounds[5]); }
                            if (this.name == "player1")
                            { Completed.SoundManager.instance.RandomizeSfx(charManager.players[1].CharSounds[3], charManager.players[1].CharSounds[4], CharSounds[5]); }
                            JustBeenHit = true;

                            //3 second pause before more sounds
                            StartCoroutine(CacophanyStopper(3));


                        }


                    }



                        break;

                //case HandleDamageColliders.DamageType.block:

                   // IsBlocking = true;

                   // break;
            }

                if (block)
                {
                    IsBlocking = true;
                    dontMove = true;
                    //blood = null;
                }



                if (IsBlocking)
                {
                damage = damage / 2;
                blood.Emit(0);
                }

                if (blood != null)

                {
                    if (!IsBlocking)//make this and if blood toggle option is on.
                    { blood.Emit(30); }
                }

                if (IsHeavy)
                { damage = damage * 2; }


                health -= damage;

                gettingHit = true;

                JustBeenHit = true;
                IsHeavy = false;

                IsBlocking = false;
                block = false;

                dontMove = false;



            //3 seconds of sounds allowed, might be wrong way around lol, as long as it works, doesn't have to debug on other pc.
            //StartCoroutine(AllowSound(3));

        }
    }

    IEnumerator CloseImmortality(float timer)
    {
        yield return new WaitForSeconds(timer);
        gettingHit = false;
    }

    IEnumerator CacophanyStopper(float timer)
    {
        yield return new WaitForSeconds(timer);
        JustBeenHit = false;
    }

    IEnumerator AllowSound(float timer)
    {
        yield return new WaitForSeconds(timer);
        JustBeenHit = true;
    }

    IEnumerator TimeHater(float timer)
    {
        yield return new WaitForSeconds(timer);
    }
}

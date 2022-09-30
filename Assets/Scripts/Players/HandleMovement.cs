using UnityEngine;
using System.Collections;


public class HandleMovement : MonoBehaviour {

    Rigidbody2D rb;
    StateManager states;
    HandleAnimations anim;
    CharacterManager charM;

    public float acceleration = 30;
    public float airAcceleration = 25;
    public float maxSpeed = 60;
    public float jumpSpeed = 5;
    public float jumpDuration = 5;

    public GameObject Character;

    float actualSpeed;
    bool justJumped;
    bool canVariableJump;
    float jmpTimer;

    //depricated
    //master array containing all the arrays of audioclips.
    //public AudioClipArray[] JumpSounds;
    //potential bug, doesn't work if defining the size of the AudioClipArray here, have to do it in editor.
    //public AudioClipArray[] JumpSounds = new AudioClipArray[6];

    //depricated
    //[HideInInspector]
    //public static string[] CharacterNames = { "Billy", "Blazer", "Chubbernaught", "Headmaster", "Janitor", "Max" };


    void Awake()
    {
        //if I need to do anything in loading.

    }

    void Start () {
        rb = GetComponent<Rigidbody2D>();
        states = GetComponent<StateManager>();
        anim = GetComponent<HandleAnimations>();
        charM = CharacterManager.GetInstance();
        rb.freezeRotation = true;

        //does not work if you define the size of AudioClipArray here either, only in editor.
        //JumpSounds = new AudioClipArray[6];

        //load all relevant sfx audio clips into the AudioClipArray. Depricated.
        //JumpSounds[0].clips = Resources.LoadAll<AudioClip>("Audio/Sounds/BillyNoMates/Jump");
        //JumpSounds[1].clips = Resources.LoadAll<AudioClip>("Audio/Sounds/Blazer/Jump");
        //JumpSounds[2].clips = Resources.LoadAll<AudioClip>("Audio/Sounds/Chubbernaught/Jump");

        //experimenting using variables from other scripts, works.
        // states.attack1 = true;


    }
	
	void FixedUpdate ()
    {
        if (!states.dontMove)
        {
            HorizontalMovement();
            Jump();
        }
    }

    void HorizontalMovement()
    {
        actualSpeed = this.maxSpeed;
        if (states.onGround)
           // if (states.onGround && !states.currentlyAttacking)
        {
            rb.AddForce(new Vector2((states.horizontal * actualSpeed) - rb.velocity.x * this.acceleration, 0));
        }

        //in case there's sliding
        if (states.horizontal == 0 && states.onGround)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    void Jump()
    {



        if (states.vertical > 0)
        {
            if (!justJumped)
            {
                justJumped = true;

                if (states.onGround)
                {
                    //play jump animation.
                    anim.JumpAnim();

                    //play randomized jump sfx depending on player and character.


                    //Completed.SoundManager.instance.RandomizeSfx(charM.players[1].CharSounds[0], charM.players[1].CharSounds[1]);

                    //depricated
                    //if (Character.name == "Chubbernaught")
                    //{
                    //    Completed.SoundManager.instance.RandomizeSfx(JumpSounds[2].clips[0], JumpSounds[2].clips[1]);
                   // }

                    //actually jump, maybe add jump force here.
                    rb.velocity = new Vector3(rb.velocity.x, this.jumpSpeed);
                    jmpTimer = 0;
                    canVariableJump = true;
                }
            }
            else
            {
                if (canVariableJump)
                {
                    jmpTimer += Time.deltaTime;

                    if (jmpTimer < this.jumpDuration / 1000)
                    {
                        rb.velocity = new Vector3(rb.velocity.x, this.jumpSpeed);
                    }
                }
            }
        }
        else
        {
            justJumped = false;
        }
    }

    public void AddVelocityOnCharacter(Vector3 direction, float timer)
    {
        StartCoroutine(AddVelocity(timer, direction));
    }

    IEnumerator AddVelocity(float timer, Vector3 direction)
    {
        float t = 0;

        while(t < timer)
        {
            t += Time.deltaTime;

            rb.velocity = direction;
            yield return null;
        }
    }
}

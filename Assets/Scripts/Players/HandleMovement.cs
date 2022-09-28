using UnityEngine;
using System.Collections;


public class HandleMovement : MonoBehaviour {

    Rigidbody2D rb;
    StateManager states;
    HandleAnimations anim;

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

    //master array containing all the arrays of audioclips.
    public AudioClipArray[] JumpSounds;
    //potential bug, doesn't work if defining the size of the AudioClipArray here, have to do it in editor.
    //public AudioClipArray[] JumpSounds = new AudioClipArray[6];

    [HideInInspector]
    public static string[] CharacterNames = { "Billy", "Blazer", "Chubbernaught", "Headmaster", "Janitor", "Max" };


    void Awake()
    {
        //if I need to do anything in loading.

    }

    void Start () {
        rb = GetComponent<Rigidbody2D>();
        states = GetComponent<StateManager>();
        anim = GetComponent<HandleAnimations>();
        rb.freezeRotation = true;

        //does not work if you define the size of AudioClipArray here either, only in editor.
        //JumpSounds = new AudioClipArray[6];

        //load all relevant sfx audio clips into the AudioClipArray.
        JumpSounds[0].clips = Resources.LoadAll<AudioClip>("Audio/Sounds/BillyNoMates/Jump");
        JumpSounds[1].clips = Resources.LoadAll<AudioClip>("Audio/Sounds/Blazer/Jump");
        JumpSounds[2].clips = Resources.LoadAll<AudioClip>("Audio/Sounds/Chubbernaught/Jump");

        //^ make for loop for this

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

                    //play randomized jump sfx
                    //make for loop for this too
                    if (Character.name == "Billy")
                    {
                        Completed.SoundManager.instance.RandomizeSfx(JumpSounds[0].clips[0], JumpSounds[0].clips[1]);
                    }

                    if (Character.name == "Blazer")
                    {
                        Completed.SoundManager.instance.RandomizeSfx(JumpSounds[1].clips[0], JumpSounds[1].clips[1]);
                    }

                    if (Character.name == "Chubbernaught")
                    {
                        Completed.SoundManager.instance.RandomizeSfx(JumpSounds[2].clips[0], JumpSounds[2].clips[1]);
                    }

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

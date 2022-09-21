using UnityEngine;
using System.Collections;


public class HandleMovement : MonoBehaviour {

    Rigidbody2D rb;
    StateManager states;
    HandleAnimations anim;

    public float acceleration = 30;
    public float airAcceleration = 15;
    public float maxSpeed = 60;
    public float jumpSpeed = 5;
    public float jumpDuration = 5;

    public GameObject Character;

    float actualSpeed;
    bool justJumped;
    bool canVariableJump;
    float jmpTimer;

    //audio clips array.
    static public AudioClip[] billyClips;
    static public AudioClip[] blazerClips;

    public AudioSource audioSource;


    void Start () {
        rb = GetComponent<Rigidbody2D>();
        states = GetComponent<StateManager>();
        anim = GetComponent<HandleAnimations>();
        rb.freezeRotation = true;


        //load all sfx audio clips.
        billyClips = Resources.LoadAll<AudioClip>("Audio/Sounds/BillyNoMates");
        blazerClips = Resources.LoadAll<AudioClip>("Audio/Sounds/Blazer");



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
                    if (Character.name == "Billy")
                    {
                        //var man = new SoundManager("Billy");
                       // audioSource.PlayOneShot(RandomClip());
                        Completed.SoundManager.instance.RandomizeSfx(billyClips[6], billyClips[7]);
                    }

                    if (Character.name == "Blazer")
                    {
                        //audioSource.PlayOneShot(RandomClip());
                        Completed.SoundManager.instance.RandomizeSfx(blazerClips[3]);
                    }



                    //source.PlayOneShot(jump0);
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

    AudioClip RandomClip()
    {
        return billyClips[Random.Range(0, billyClips.Length)];
        //return blazerClips[Random.Range(0, blazerClips.Length)];
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

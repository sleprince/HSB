using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1Actions : MonoBehaviour
{
    public float JumpSpeed = 1.0f;
    public GameObject Player1;
    private Animator Anim;
    private AnimatorStateInfo Player1Layer0;
    private bool HeavyMoving = false;
    private bool HeavyReact = false;
    public float PunchSlideAmt = 2f;
    public float HeavyReactAmt = 4f;
    private AudioSource MyPlayer;
    public AudioClip PunchWoosh;
    public AudioClip KickWoosh;
    public static bool Hits = false;
    public static bool FlyingJumpP1 = false;

    // Start is called before the first frame update
    void Start()
    {
        Anim = GetComponent<Animator>();
        MyPlayer = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (SaveScript.TimeOut == false)
        {
            //Heavy Punch Slide
            if (HeavyMoving == true)
            {
                if (Player1Move.FacingRight == true)
                {
                    Player1.transform.Translate(PunchSlideAmt * Time.deltaTime, 0, 0);
                }
                if (Player1Move.FacingLeft == true)
                {
                    Player1.transform.Translate(-PunchSlideAmt * Time.deltaTime, 0, 0);
                }
            }

            //Heavy React Slide
            if (HeavyReact == true)
            {
                if (Player1Move.FacingRight == true)
                {
                    Player1.transform.Translate(-HeavyReactAmt * Time.deltaTime, 0, 0);
                }
                if (Player1Move.FacingLeft == true)
                {
                    Player1.transform.Translate(HeavyReactAmt * Time.deltaTime, 0, 0);
                }
            }

            //Listen to the Animator
            Player1Layer0 = Anim.GetCurrentAnimatorStateInfo(0);

            //Standing attacks
            if (Player1Layer0.IsTag("Motion") || Player1Layer0.IsTag("Block"))
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    Anim.SetTrigger("LightPunch");
                    Anim.SetTrigger("BlockOff");
                    Hits = false;
                }
                if (Input.GetButtonDown("Fire2"))
                {
                    Anim.SetTrigger("HeavyPunch");
                    Anim.SetTrigger("BlockOff");
                    Hits = false;
                }
                if (Input.GetButtonDown("Fire3"))
                {
                    Anim.SetTrigger("LightKick");
                    Anim.SetTrigger("BlockOff");
                    Hits = false;
                }
                if (Input.GetButtonDown("Jump"))
                {
                    Anim.SetTrigger("HeavyKick");
                    Anim.SetTrigger("BlockOff");
                    Hits = false;
                }
                if (Input.GetButtonDown("Block"))
                {
                    Anim.SetTrigger("BlockOn");
                }
            }

            if (Player1Layer0.IsTag("Block"))
            {
                if (Input.GetButtonUp("Block"))
                {
                    Anim.SetTrigger("BlockOff");
                }
            }


            //Crouching attack
            if (Player1Layer0.IsTag("Crouching"))
            {
                if (Input.GetButtonDown("Fire3"))
                {
                    Anim.SetTrigger("LightKick");
                    Hits = false;
                }
            }

            //Aerial moves
            if (Player1Layer0.IsTag("Jumping"))
            {
                if (Input.GetButtonDown("Jump"))
                {
                    Anim.SetTrigger("HeavyKick");
                    Hits = false;
                }
            }
        }
    }

    public void JumpUp()
    {
        Player1.transform.Translate(0, JumpSpeed * Time.deltaTime, 0);
    }
    public void HeavyMove()
    {
        StartCoroutine(PunchSlide());
    }
    public void HeavyReaction()
    {
        StartCoroutine(HeavySlide());
    }
    public void FlipUp()
    {
        Player1.transform.Translate(0, JumpSpeed * Time.deltaTime, 0);
        FlyingJumpP1 = true;
    }
    public void FlipBack()
    {
        Player1.transform.Translate(0, JumpSpeed * Time.deltaTime, 0);
        FlyingJumpP1 = true;
    }

    public void IdleSpeed()
    {
        FlyingJumpP1 = false;
    }

    public void ResetTime()
    {
        Time.timeScale = 1.0f;
    }


    public void KickWooshSound()
    {
        MyPlayer.clip = KickWoosh;
        MyPlayer.Play();
    }

    public void PunchWooshSound()
    {
        MyPlayer.clip = PunchWoosh;
        MyPlayer.Play();
    }

    public void RandomAttack()
    {

    }

    IEnumerator PunchSlide()
    {
        HeavyMoving = true;
        yield return new WaitForSeconds(0.1f);
        HeavyMoving = false;
    }

    IEnumerator HeavySlide()
    {
        HeavyReact = true;
        yield return new WaitForSeconds(0.3f);
        HeavyReact = false;
    }
}

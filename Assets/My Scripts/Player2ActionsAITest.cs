using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2ActionsAITest : MonoBehaviour
{
    public float JumpSpeed = 10.0f;
    public float JumpDistance = 10.0f;
    public GameObject Player1;
    private Animator Anim;
    private AnimatorStateInfo Player1Layer0;
    private bool HeavyMoving = false;
    private bool HeavyReact = false;
    public float PunchSlideAmt = 4f;
    public float HeavyReactAmt = 4f;
    private AudioSource MyPlayer;
    public AudioClip PunchWoosh;
    public AudioClip KickWoosh;
    public AudioClip Hurt1;
    public AudioClip Hurt2;
    public AudioClip Hurt3;
    public AudioClip Hurt4;
    public float JumpDownWait = 0.5f;
    public static bool IsBlockingAI = false;
    public static bool HitsAI = false;
    public static bool FlyingJumpAI = false;
    public static bool Dazed = false;
    public static float DazedTime = 3.0f;
    private int AttackNumber = 1;
    private bool Attacking = true;
    public float AttackRate = 1.0f;
    private int CrouchAttack = 0;
    


    // Start is called before the first frame update
    void Start()
    {
        Anim = GetComponent<Animator>();
        MyPlayer = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (SaveScript.TimeOut == true)
        {
            Anim.SetBool("Forward", false);
            Anim.SetBool("Backwards", false);
        }
        if (SaveScript.TimeOut == false)
        {
            //Heavy Punch Slide
            if (HeavyMoving == true)
            {
                if (Player2MoveAITest.FacingLeftAI == true)
                {
                    Player1.transform.Translate(PunchSlideAmt * Time.deltaTime, 0, 0);
                }
                if (Player2MoveAITest.FacingRightAI == true)
                {
                    Player1.transform.Translate(-PunchSlideAmt * Time.deltaTime, 0, 0);
                }
            }

            //Heavy React Slide
            if (HeavyReact == true)
            {
                if (Player2MoveAITest.FacingLeftAI == true)
                {
                    Player1.transform.Translate(-HeavyReactAmt * Time.deltaTime, 0, 0);
                }
                if (Player2MoveAITest.FacingRightAI == true)
                {
                    Player1.transform.Translate(HeavyReactAmt * Time.deltaTime, 0, 0);
                }
            }
            // Considerar o Animator
            Player1Layer0 = Anim.GetCurrentAnimatorStateInfo(0);

            // Ataques em pé
            if (Player1Layer0.IsTag("Motion"))
            {
                if (Attacking == true)
                {
                    Attacking = false;
                    if (AttackNumber == 1)
                    {
                        Anim.SetTrigger("LightPunch");
                        HitsAI = false;
                    }
                    if (AttackNumber == 2)
                    {
                        Anim.SetTrigger("HeavyPunch");
                        HitsAI = false;
                    }
                    if (AttackNumber == 3)
                    {
                        Anim.SetTrigger("LightKick");
                        HitsAI = false;
                    }
                    if (AttackNumber == 4)
                    {
                        Anim.SetTrigger("HeavyKick");
                        HitsAI = false;
                    }
                }
                if (Input.GetButtonDown("BlockP2"))
                {
                    Anim.SetTrigger("BlockOn");
                    if (Input.GetAxis("VerticalP2") == 0)
                    {
                        Anim.SetTrigger("CrouchBlockOff");
                    }
                }
                // Defender agachade
                if (Input.GetButtonDown("BlockP2"))
                {
                    if (Input.GetAxis("VerticalP2") < 0)
                    {
                        Anim.SetTrigger("CrouchBlockOn");
                        Anim.SetTrigger("BlockOff");
                    }
                }
            }
            // Se estiver em uma animação de defesa
            if (Player1Layer0.IsTag("Block"))
            {
                IsBlockingAI = true;
                // Se apertar defesa
                if (Input.GetButtonDown("BlockP2"))
                {
                    // Parar de defender se soltar botão defesa
                    if (Input.GetButtonUp("Block"))
                    {
                        Anim.SetTrigger("BlockOff");
                        Anim.SetTrigger("CrouchBlockOff");
                    }
                    //
                    // Defender Agachade
                    if (Input.GetAxis("VerticalP2") < 0)
                    {
                        Anim.SetTrigger("CrouchBlockOn");
                        Anim.SetTrigger("BlockOff");
                    }
                    // Transição de Defender Agachade para Defender Em pé
                    if (Input.GetAxis("VerticalP2") == 0)
                    {
                        Anim.SetTrigger("CrouchBlockOff");
                        Anim.SetTrigger("BlockOn");
                    }
                }
            }
            else
            {
                IsBlockingAI = false;
                Player1Move.WalkRightP1 = true;
                Player1Move.WalkLeftP1 = true;
            }
            {
                // Parar de defender
                if (Input.GetButtonUp("BlockP2"))
                {
                    Anim.SetTrigger("BlockOff");
                    Anim.SetTrigger("CrouchBlockOff");
                }
            }
            // Ataques agachade
            if (Player1Layer0.IsTag("Crouching"))
            {
                CrouchAttack = Random.Range(1, 4);
                Debug.Log("CrouchAttack = " + CrouchAttack);
                if (CrouchAttack == 1)
                {
                    Anim.SetTrigger("LightPunch");
                    HitsAI = false;
                    Anim.SetBool("Crouch", false);
                }
                if (CrouchAttack == 2)
                {
                    Anim.SetTrigger("HeavyPunch");
                    HitsAI = false;
                    Anim.SetBool("Crouch", false);

                }
                if (CrouchAttack == 3)
                {
                    Anim.SetTrigger("LightKick");
                    HitsAI = false;
                    Anim.SetBool("Crouch", false);

                }
                if (CrouchAttack == 4)
                {
                    Anim.SetTrigger("HeavyKick");
                    HitsAI = false;
                    Anim.SetBool("Crouch", false);
                }

                if (Input.GetButtonDown("BlockP2"))
                {
                    Anim.SetTrigger("CrouchBlockOn");
                    Anim.SetTrigger("BlockOff");
                }

            }
            // Ataques aéreos
            if (Player1Layer0.IsTag("Jumping"))
            {
                // Defender agachade
                if (Input.GetButtonDown("BlockP2"))
                {
                    if (Input.GetAxis("VerticalP2") < 0)
                    {
                        Anim.SetTrigger("CrouchBlockOn");
                        Anim.SetTrigger("BlockOff");
                    }
                    else
                    {
                        Anim.SetTrigger("CrouchBlockOff");
                        Anim.SetTrigger("BlockOn");
                    }
                }
                if (Input.GetButtonDown("Fire1P2"))
                {
                    Anim.SetTrigger("LightPunch");
                    HitsAI = false;
                }
                if (Input.GetButtonDown("Fire2P2"))
                {
                    Anim.SetTrigger("HeavyPunch");
                    HitsAI = false;
                }
                if (Input.GetButtonDown("Fire3P2"))
                {
                    Anim.SetTrigger("LightKick");
                    HitsAI = false;
                }
                if (Input.GetButtonDown("Fire4P2"))
                {
                    Anim.SetTrigger("HeavyKick");
                    HitsAI = false;
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

    public void RandomAttack()
    {
            AttackNumber = Random.Range(1, 4);
        StartCoroutine(SetAttacking());

    }
    public void HeavyReaction()
    {
        StartCoroutine(HeavySlide());
    }
    public void BackFlip()
    {
        Player1.transform.Translate(0, JumpSpeed * Time.deltaTime, 0);
        FlyingJumpAI = true;
    }
    public void ForwardFlip()
    {
        Player1.transform.Translate(0, JumpSpeed * Time.deltaTime, 0);
        FlyingJumpAI = true;
    }
    public void IdleSpeed()
    {
        FlyingJumpAI = false;
    }
    public void PunchWooshSound()
    {
        MyPlayer.clip = PunchWoosh;
        MyPlayer.Play();
    }
    public void KickWooshSound()
    {
        MyPlayer.clip = KickWoosh;
        MyPlayer.Play();
    }
    public void HurtSound1()
    {
        MyPlayer.clip = Hurt1;
        MyPlayer.Play();
    }
    public void HurtSound2()
    {
        MyPlayer.clip = Hurt2;
        MyPlayer.Play();
    }
    public void HurtSound3()
    {
        MyPlayer.clip = Hurt3;
        MyPlayer.Play();
    }
    public void HurtSound4()
    {
        MyPlayer.clip = Hurt3;
        MyPlayer.Play();
    }
    IEnumerator PunchSlide()
    {
        HeavyMoving = true;
        yield return new WaitForSeconds(0.005f);
        HeavyMoving = false;
    }
    IEnumerator HeavySlide()
    {
        HeavyReact = true;
        Dazed = true;
        yield return new WaitForSeconds(0.3f);
        HeavyReact = false;
        yield return new WaitForSeconds(DazedTime);
        Dazed = false;
    }
    IEnumerator SetAttacking()
    {
        yield return new WaitForSeconds(AttackRate);
        Attacking = true;
    }

}

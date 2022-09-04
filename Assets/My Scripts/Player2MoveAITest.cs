using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2MoveAITest : MonoBehaviour
{
    private Animator Anim;
    public float WalkSpeed = 0.1f;
    public float JumpSpeed = 0.1f;
    public float JumpMoveSpeed = 0.1f;
    private float MoveSpeed;
    private bool IsJumping = false;
    private bool IsCrouching = false;
    private bool IsBlocking = false;
    private AnimatorStateInfo Player1Layer0;
    private bool CanWalkLeft = true;
    private bool CanWalkRight = true;
    public GameObject Player1;
    public GameObject Opponent;
    private Vector3 OppPosition;
    public static bool FacingLeftAI = true;
    public static bool FacingRightAI = false;
    public static bool WalkLeftAI = true;
    public static bool WalkRightAI = true;
    public AudioClip LightPunch;
    public AudioClip HeavyPunch;
    public AudioClip LightKick;
    public AudioClip HeavyKick;
    public AudioClip LegSweep;
    private AudioSource MyPlayer;
    public GameObject Restrict;
    public GameObject RB;
    public Collider BoxCollider;
    public Collider CapsuleCollider;
    private int Defend = 0;
    private float OppDistance;
    public float AttackDistance = 1.5f;
    private bool MoveAI = true;
    public static bool AttackState = false;

    // Start is called before the first frame update
    void Start()
    {
        FacingLeftAI = false;
        FacingRightAI = true;
        WalkLeftAI = true;
        WalkRightAI = true;
        Opponent = GameObject.Find("Player1");
        OppDistance = Vector3.Distance(Opponent.transform.position, Player1.transform.position);
        Anim = GetComponentInChildren<Animator>();
        StartCoroutine(FaceRight());
        MyPlayer = GetComponentInChildren<AudioSource>();
        MoveSpeed = WalkSpeed;
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
            OppDistance = Vector3.Distance(Opponent.transform.position, Player1.transform.position);
            // Distância percorrida com pulos para esquerda e direita
            if (Player2Actions.FlyingJumpP2 == true)
            {
                WalkSpeed = JumpMoveSpeed;
            }
            else
            {
                // Retornar a velocidade de movimento ao normal
                WalkSpeed = MoveSpeed;
            }

            // Conferir se o personagem está nocauteado
            if (SaveScript.Player2Health <= 0)
            {
                Anim.SetTrigger("Knockout");
                Player1.GetComponent<Player2ActionsAITest>().enabled = false;
                StartCoroutine(KnockedOut());
            }

            // Conferir se o outro personagem está nocauteado
            if (SaveScript.Player1Health <= 0)
            {
                // Animação de Vitória
                Anim.SetTrigger("Victory");
                Player1.GetComponent<Player2ActionsAITest>().enabled = false;
                this.GetComponent<Player2MoveAITest>().enabled = false;
            }

            Debug.Log(WalkLeftAI);
            // Considerar o Animator
            Player1Layer0 = Anim.GetCurrentAnimatorStateInfo(0);

            //Não sair da tela
            Vector3 ScreenBounds = Camera.main.WorldToScreenPoint(this.transform.position);

            if (ScreenBounds.x > Screen.width - 150)
            {
                CanWalkRight = false;
            }
            if (ScreenBounds.x < 150)
            {
                CanWalkLeft = false;
            }
            else if (ScreenBounds.x > 150 && ScreenBounds.x < Screen.width - 150)
            {
                CanWalkRight = true;
                CanWalkLeft = true;
            }

            // Identificar a posição de oponente
            OppPosition = Opponent.transform.position;

            if (Player2ActionsAITest.Dazed == false)
            {
                // Movimentação da IA
                if (OppPosition.x > Player1.transform.position.x)
                {
                    StartCoroutine(FaceLeft());
                    if (Player1Layer0.IsTag("Motion"))
                    {
                        Time.timeScale = 1.0f;
                        Anim.SetBool("CanAttack", false);
                        if (OppDistance > AttackDistance)
                        {
                            if (MoveAI == true)
                            {
                                if (CanWalkRight == true)
                                {
                                    if (WalkRightAI == true)
                                    {
                                        Anim.SetBool("Backwards", true);
                                        Anim.SetBool("Forward", false);
                                        AttackState = false;
                                        transform.Translate(WalkSpeed, 0, 0);
                                    }
                                }
                            }
                        }
                        if (OppDistance < AttackDistance)
                        {
                            if (CanWalkRight == true)
                            {
                                if (MoveAI == true)
                                {
                                    MoveAI = false;
                                    Anim.SetBool("Backwards", false);
                                    Anim.SetBool("Forward", false);
                                    Anim.SetBool("CanAttack", true);
                                    StartCoroutine(ForwardFalse());
                                }
                            }
                        }
                    }
                }
                if (OppPosition.x < Player1.transform.position.x)
                {
                    StartCoroutine(FaceRight());
                    if (Player1Layer0.IsTag("Motion"))
                    {
                        Time.timeScale = 1.0f;
                        Anim.SetBool("CanAttack", false);
                        if (OppDistance > AttackDistance)
                        {
                            if (MoveAI == true)
                            {
                                if (CanWalkLeft == true)
                                {
                                    if (WalkLeftAI == true)
                                    {
                                        Anim.SetBool("Forward", true);
                                        Anim.SetBool("Backwards", false);
                                        AttackState = false;
                                        transform.Translate(-WalkSpeed, 0, 0);
                                    }
                                }
                            }
                        }
                        if (OppDistance < AttackDistance)
                        {
                            if (CanWalkLeft == true)
                            {
                                if (MoveAI == true)
                                {
                                    MoveAI = false;
                                    Anim.SetBool("Backwards", false);
                                    Anim.SetBool("Forward", false);
                                    Anim.SetBool("CanAttack", true);
                                    StartCoroutine(ForwardFalse());
                                }
                            }
                        }
                    }
                }

            }



            // Pulando, Defendendo e Agachando

            if (Defend == 4)
            {
                Anim.SetTrigger("Jump");
                Defend = 0;
            }
            if (Defend == 3)
            {
                IsCrouching = true;
                Anim.SetBool("Crouch", true);
                Defend = 0;
            }
            if (Defend == 2)
            {
                if (IsBlocking == false)
                {
                    IsBlocking = true;
                    Anim.SetTrigger("BlockOn");
                    StartCoroutine(EndBlock());
                }
            }

            // Resets the restrict
            if (Restrict.gameObject.activeInHierarchy == false)
            {
                WalkLeftAI = true;
                WalkRightAI = true;
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    // Sons apanhando
    {
        if (other.gameObject.CompareTag("FistLight"))
        {
            Anim.SetTrigger("HeadHit");
            MyPlayer.clip = LightPunch;
            MyPlayer.Play();
            Defend = 3;
            //Defend = Random.Range(0, 5);
            Debug.Log("Defend =" + Defend);
        }
        if (other.gameObject.CompareTag("FistHeavy"))
        {
            Anim.SetTrigger("HeavyHit");
            MyPlayer.clip = HeavyPunch;
            MyPlayer.Play();
        }
        if (other.gameObject.CompareTag("KickHeavy"))
        {
            Anim.SetTrigger("HeavyHit");
            MyPlayer.clip = HeavyKick;
            MyPlayer.Play();
        }
        if (other.gameObject.CompareTag("KickLight"))
        {
            Anim.SetTrigger("HeavyHit");
            MyPlayer.clip = LightKick;
            MyPlayer.Play();
            Defend = 3;
            //Defend = Random.Range(0, 5);
        }
        if (other.gameObject.CompareTag("LegSweep"))
        {
            Anim.SetTrigger("LegSweep");
            MyPlayer.clip = LegSweep;
            MyPlayer.Play();
            Defend = Random.Range(0, 5);
        }
    }

    IEnumerator FaceRight()
    {
        if (FacingRightAI == true)
        {
            FacingRightAI = false;
            FacingLeftAI = true;
            yield return new WaitForSeconds(0.15f);
            Player1.transform.Rotate(0, -180, 0);
            Anim.SetLayerWeight(1, 0);
        }
    }
    IEnumerator FaceLeft()
    {
        if(FacingLeftAI == true)
        {
            FacingLeftAI = false;
            FacingRightAI = true;
        yield return new WaitForSeconds(0.15f);
        Player1.transform.Rotate(0, 180, 0);
            Anim.SetLayerWeight(1, 1);
        }
    }
    IEnumerator KnockedOut()
    {
        yield return new WaitForSeconds(0.1f);
        this.GetComponent<Player2MoveAITest>().enabled = false;
    } 
    
    IEnumerator ForwardFalse()
    {
        yield return new WaitForSeconds(0.6f);
         MoveAI = true;
    }
    IEnumerator EndBlock()
    {
        yield return new WaitForSeconds(2.0f);
        IsBlocking = false;
        Anim.SetTrigger("BlockOff");
        Defend = 0;
    }
}

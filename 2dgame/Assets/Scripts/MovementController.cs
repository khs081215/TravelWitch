using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

/**
 * 플레이어의 Movement를 책임지는 컴포넌트입니다.
 */
public class MovementController : MonoBehaviourPunCallbacks, IPunObservable
{
    [HideInInspector] public bool isTalking { get; set; } = false;

    #region private member

    private float movementSpeed = 3.0f;
    private Vector2 movement = new Vector2();
    private int jumpingCount = 0;
    private int direction = 0;
    private int directionForDoubleJump = 0;
    private float damping = 0;
    private Animator animator;
    private string animationState = "AnimationState";
    private Rigidbody2D rb2D;
    private SpriteRenderer sp2D;
    private BoxCollider2D bx2D;
    private bool isJumping = false;
    private bool isDoubleJumping = false;
    private Transform transForm;
    private float jumpTime = 0.0f;
    private float doubleJumpTime = 0.0f;
    private PhotonView pv;
    private Vector3 receivePos;
    private GameObject upJumpEffect;
    private GameObject rightJumpEffect;
    private GameObject leftJumpEffect;

    #endregion private member


    enum CharStates
    {
        walkEast = 1,
        walkSouth = 2,
        walkWest = 3,
        walkNorth = 4,
        idleSouth = 5
    }


    private void Start()
    {
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        transForm = GetComponent<Transform>();
        sp2D = GetComponent<SpriteRenderer>();
        bx2D = GetComponent<BoxCollider2D>();
        pv = GetComponent<PhotonView>();
        upJumpEffect = transform.GetChild(0).gameObject;
        rightJumpEffect = transform.GetChild(1).gameObject;
        leftJumpEffect = transform.GetChild(2).gameObject;
        if (!pv.IsMine && PhotonNetwork.CurrentRoom != null)
        {
            rb2D.gravityScale = 0;
        }
    }

    /**
     * jumpingCount는 아래와 같이 player의 State를 나타낸다.
     * 0 : 지상, 1 : 1단점프중, 2: 2단점프중, 3: 고속낙하 중(S키 입력시),  4: 착지 중(벽에 충돌시)
     */
    private void Update()
    {
        //내 클라이언트이거나, 마을일 경우.
        if (pv.IsMine || PhotonNetwork.CurrentRoom == null)
        {
            if (!isTalking && (EventSystem.current.currentSelectedGameObject == null ||
                               EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>() == null))
            {
                //지상에서 LeftShift를 누를 경우
                if (Input.GetKey(KeyCode.LeftShift) && jumpingCount == 0) movementSpeed = 6.0f;
                else movementSpeed = 4.0f;
                //지상이나 1단 점프는 좌우 입력한 값을 속도값에 적용합니다.
                if (jumpingCount == 0 || jumpingCount == 1) movement.x = Input.GetAxisRaw("Horizontal");
                //점프하는 경우
                if (Input.GetKeyDown(KeyCode.W))
                {
                    //2단 점프의 경우
                    if (jumpingCount == 1)
                    {
                        directionForDoubleJump = direction;
                        jumpingCount = 2;
                    }

                    //1단 점프의 경우
                    if (jumpingCount == 0)
                    {
                        isJumping = true;
                        jumpingCount = 1;
                    }
                }

                // 속도값을 속도에 매핑합니다.
                // x축은 앞서 받은 속도값을 사용하고, y축은 점프시에는 0.2초에 걸쳐 감소하는 속도값을 부여받습니다.
                if (isJumping)
                {
                    jumpTime += Time.deltaTime;
                    if (jumpTime < 0.20f)
                    {
                        rb2D.velocity = new Vector2(movement.x * movementSpeed, (20.0f - (jumpTime * 100)));
                    }
                    else
                    {
                        isJumping = false;
                        jumpTime = 0.0f;
                    }
                }
                else rb2D.velocity = new Vector2(movement.x * movementSpeed, 0.0f);

                //S키를 입력할 경우 낙하합니다.
                if (Input.GetKeyDown(KeyCode.S))
                {
                    jumpingCount = 3;
                }


                //jumpingCount
                if (jumpingCount == 0)
                {
                    upJumpEffect.SetActive(false);
                    rightJumpEffect.SetActive(false);
                    leftJumpEffect.SetActive(false);
                }

                if (jumpingCount == 1)
                {
                    upJumpEffect.SetActive(true);
                }

                if (jumpingCount == 2)
                {
                    upJumpEffect.SetActive(false);
                    if (directionForDoubleJump > 0) rightJumpEffect.SetActive(true);
                    if (directionForDoubleJump < 0) leftJumpEffect.SetActive(true);


                    doubleJumpTime += Time.deltaTime;
                    if (doubleJumpTime < 0.08f)
                    {
                        rb2D.velocity = new Vector2(directionForDoubleJump * 9.0f, 20.0f);
                    }
                    else rb2D.velocity = new Vector2(directionForDoubleJump * 9.0f, 0);
                }

                if (jumpingCount == 3) rb2D.velocity = new Vector2(0, -12.0f);
                if (jumpingCount == 4) rb2D.velocity = new Vector2(0, -3.0f);

                UpdateState();
            }
            else
            {
                rb2D.velocity = new Vector2(0, -3.0f);
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, receivePos, Time.deltaTime * damping);
        }
    }

    //벽이나 땅에 충돌시 현재 속도를 잃고 착지합니다.
    void OnCollisionEnter2D(Collision2D collision)
    {
        jumpingCount = 4;
        direction = 0;
    }

    //땅에 착지시 jumpingCount를 0으로 만듭니다.
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("JumpGround") || collision.gameObject.CompareTag("Enemy"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (Vector2.Dot(contact.normal, Vector2.up) > 0.5f)
                {
                    jumpingCount = 0;
                    doubleJumpTime = 0.0f;
                    return;
                }
            }
        }

        direction = 0;
    }

    //지형에서 낙하 시 1단 점프 중인 상태로 변경합니다.(x축으로는 자유롭게 이동하고, y축으로는 낙하해야 하므로)
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("JumpGround"))
        {
            jumpingCount = 1;
        }
    }

    //캐릭터 애니메이션을 업데이트합니다.
    private void UpdateState()
    {
        if (jumpingCount == 0)
        {
            if (Input.GetAxisRaw("Horizontal") > 0) animator.SetInteger(animationState, (int)CharStates.walkEast);
            else if (Input.GetAxisRaw("Horizontal") < 0) animator.SetInteger(animationState, (int)CharStates.walkWest);
            else animator.SetInteger(animationState, (int)CharStates.idleSouth);
        }
        else
        {
            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                direction = 1;
            }
            else if (Input.GetAxisRaw("Horizontal") < 0)
            {
                direction = -1;
            }

            if (direction == 1)
            {
                animator.SetInteger(animationState, (int)CharStates.walkEast);
            }
            else if (direction == -1)
            {
                animator.SetInteger(animationState, (int)CharStates.walkWest);
            }

            else if (rb2D.velocity.x > 0)
            {
                direction = 1;
                animator.SetInteger(animationState, (int)CharStates.walkEast);
            }
            else if (rb2D.velocity.x < 0)
            {
                direction = -1;
                animator.SetInteger(animationState, (int)CharStates.walkWest);
            }
            else
            {
                animator.SetInteger(animationState, (int)CharStates.idleSouth);
            }
        }
    }

    //포톤 서버에 캐릭터의 transform을 전송합니다.
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            receivePos = (Vector3)stream.ReceiveNext();
        }
    }
}
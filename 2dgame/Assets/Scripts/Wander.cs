using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * Walking, Attacking 2가지 State를 전환하는 FSM입니다.
 */
public class Wander : MonoBehaviour
{
    public Transform attackTarget { get; set; }
    
    private float attackTime = 1.0f;
    private float waitTime;
    private float walkRange = 5.0f;
    private Vector3 basePosition;
    private Rigidbody2D rb2D;
    private GameObject eammo;
    private SpriteRenderer rend;
    [SerializeField]private GameObject EAmmoPrefab;

    int direction = 1;

    enum State
    {
        Walking,	
        Attacking	
    };

    State state = State.Walking;       
    State nextState = State.Walking;	



    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Walking:
                Walking();
                break;
            case State.Attacking:
                Attacking();
                break;
        }

        if (state != nextState)
        {
            state = nextState;
            switch (state)
            {
                case State.Walking:
                    break;
                case State.Attacking:
                    AttackStart();
                    break;
            }
        }

    }
    ///일자 발판에서 모서리 끝을 만나면 방향을 전환합니다.
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("bound"))
        {
            if (direction == 1) direction = -1;
            else direction = 1;
            if (rend.flipX) rend.flipX = false;
            else rend.flipX = true;
        }
    }



    void ChangeState(State nextState)
    {
        this.nextState = nextState;
    }


    void Walking()
    {
        rb2D.velocity = new Vector2(2.0f*direction,0);
        if (attackTarget) ChangeState(State.Attacking);

    }
    void AttackStart()
    {
        attackTime = 1.0f;
    }


    void Attacking()
    {
        rb2D.velocity = new Vector2(0, 0);
        attackTime += Time.deltaTime;
        if (attackTime>2.0f)
        {
            eammo = Instantiate(EAmmoPrefab,transform.position, Quaternion.identity);
            eammo.GetComponent<Ammo>().Shoot(attackTarget.position,false);
            attackTime = 0.0f;
        }
        if(attackTarget==null) ChangeState(State.Walking);
    }
}

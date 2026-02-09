using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * Player가 공격 범위에 들어오면 Wander의 attackTarget으로 설정합니다.
 */
public class Lookattack : MonoBehaviour
{
    private Wander wander;

    private void Start()
    {
        wander=transform.parent.GetComponent<Wander>();
    }


    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            wander.attackTarget = collision.gameObject.transform; 
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            wander.attackTarget = null;
        }
    }
}

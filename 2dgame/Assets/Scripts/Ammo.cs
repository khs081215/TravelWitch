using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/**
 * 총알을 목표 방향으로 발사하여 5초가 지나거나, 충돌시 비활성화합니다.
 */
public class Ammo : MonoBehaviour
{
    [HideInInspector] public Vector2 shootPoint;

    private bool isShooting = false;
    private int damageStrength = 10;
    private bool isPlayer = false;
    private float maintainTime = 0.0f;
    private Rigidbody2D rb2D;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isShooting)
        {
            rb2D.velocity = shootPoint.normalized * 10.0f;
            maintainTime += Time.deltaTime;
            if (maintainTime > 5.0f)
            {
                Sleepthis();
            }
        }
    }

    public void Shoot(Vector2 targetLocation, bool isplayer)
    {
        shootPoint = targetLocation - new Vector2(transform.position.x, transform.position.y);
        isShooting = true;
        isPlayer = isplayer;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPlayer)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                if (collision.gameObject.TryGetComponent<Enemy>(out Enemy enemy))
                {
                    enemy.TakeDamaged(damageStrength, 0.0f);
                }

                Sleepthis();
            }

            if (!collision.gameObject.CompareTag("Player"))
            {
                Sleepthis();
            }
        }
        else
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Player player = collision.gameObject.GetComponent<Player>();
                player.TakeDamaged(damageStrength, 0.0f);
                Sleepthis();
            }

            if (!collision.gameObject.CompareTag("Enemy")) Sleepthis();
        }
    }

    private void Sleepthis()
    {
        maintainTime = 0;
        isShooting = false;
        gameObject.SetActive(false);
    }
}
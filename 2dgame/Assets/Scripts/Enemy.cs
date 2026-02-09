using System.Collections;
using UnityEngine;

public class Enemy : Character
{
    public bool isDamaging { get; private set; }

    private float hitPoints;
    private GameObject hpBackGroundUI;
    private GameObject hpBarUI;
    [SerializeField] private int damageStrength;
    [SerializeField] private GameObject hp;
    [SerializeField] private GameObject coin;


    private void OnEnable()
    {
        ResetCharacter();
        hpBackGroundUI=transform.GetChild(0).gameObject;
        hpBarUI = transform.GetChild(1).gameObject;
    }
    private void Update()
    {
        if (hitPoints != startingHitPoints)
        {
            hpBackGroundUI.SetActive(true);
            hpBarUI.SetActive(true);
            hpBarUI.transform.localScale = new Vector3(
                hitPoints / (float)startingHitPoints * 1.18f, hpBarUI.transform.localScale.y,
                hpBarUI.transform.localScale.z);
        }
    }
    public override void ResetCharacter()
    {
        hitPoints = startingHitPoints;
    }

    public override void TakeDamaged(int damage, float interval)
    {
        if (damageCoroutine == null)
        {
            damageCoroutine = StartCoroutine(DamageCharacter(damage, interval));
        }
    }


    protected override IEnumerator DamageCharacter(int damage, float interval)
    {
        while (true)
        {
            hitPoints = hitPoints - damage;

            if (hitPoints <= float.Epsilon)
            {
                KillCharacter();
                break;
            }

            if (interval > float.Epsilon)
            {
                yield return new WaitForSeconds(interval);
            }
            else
            {
                break;
            }
        }
    }
//처치시 포션과 코인 1개씩 드랍합니다.
    public override void KillCharacter()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.36f, transform.position.z);
        Instantiate(hp, transform.position, Quaternion.identity);
        transform.position = new Vector3(transform.position.x + 0.3f, transform.position.y, transform.position.z);
        Instantiate(coin, transform.position, Quaternion.identity);

        base.KillCharacter();
    }

//플레이어와 충돌중이면 플레이어에게 1초마다 10의 데미지를 줍니다.
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();


            if (damageCoroutine == null)
            {
                player.TakeDamaged(damageStrength, 1.0f);
            }
        }
    }
    
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            player.damageCoroutine = null;
        }
    }
}
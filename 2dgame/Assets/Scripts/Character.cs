using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Serialization;

/**
 * 캐릭터의 기본적인 속성을 가지는 추상 클래스입니다.
 */
public abstract class Character : MonoBehaviourPunCallbacks
{
    public float maxHitPoints;
    public float startingHitPoints;
    public Coroutine damageCoroutine { get; set; }

    public virtual void KillCharacter()
    {
        Destroy(gameObject);
    }

    public abstract void TakeDamaged(int damage, float interval);

    public abstract void ResetCharacter();
    protected abstract IEnumerator DamageCharacter(int damage, float interval);
}
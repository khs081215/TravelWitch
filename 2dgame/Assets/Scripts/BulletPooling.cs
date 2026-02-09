using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using System.Security;
using UnityEngine;

/**
 * 20개의 총알을 관리하는 오브젝트 풀입니다.
 */
public class BulletPooling : MonoBehaviour
{
    private GameObject[] bulletpool;
    [SerializeField]private GameObject Bullet;
    
    void Start()
    {
        bulletpool = new GameObject[20];
        for (int i = 0; i < bulletpool.Length; i++)
        {
            GameObject gameObject = Instantiate(Bullet);
            bulletpool[i] = gameObject;
            gameObject.SetActive(false);
        }
    }
/// 오브젝트 풀에서 비활성화된 오브젝트를 찾아 반환합니다.
    public GameObject SetGameObject()
    {
        for (int i = 0; i < 20; i++)
        {
            if( !bulletpool[i].activeSelf)
            {
                bulletpool[i].SetActive(true);
                return bulletpool [i];
            }
        }
        return null;
    }
}

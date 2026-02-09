using UnityEngine;
/**
 * 게임오브젝트를 해당 위치에 스폰합니다.
 */
public class SpawnPoint : MonoBehaviour
{
    [SerializeField]private GameObject prefabToSpawn;
    public float repeatInterval;

    public void Start()
    {
        if (repeatInterval > 0)
        {
            InvokeRepeating("SpawnObject", 0.0f, repeatInterval);
        }
    }

    public GameObject SpawnObject()
    {

        if (prefabToSpawn != null)
        {
            return Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
        }
        return null;
    }
    public GameObject SpawnObject(GameObject prefab)
    {

        if (prefab != null)
        {
            return Instantiate(prefab, transform.position, Quaternion.identity);
        }
        return null;
    }
}
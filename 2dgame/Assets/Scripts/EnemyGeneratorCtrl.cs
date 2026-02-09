using UnityEngine;
using System.Collections;
/**
 * regenTime마다 몬스터의 최대 수가 유지되도록 몬스터를 스폰합니다.
 */
public class EnemyGeneratorCtrl : MonoBehaviour {
	private float regenTime=30.0f;
	private GameObject[] existEnemys;
	[SerializeField]private int maxEnemy = 2;
	[SerializeField]private GameObject enemyPrefab;

	void Start()
	{
		existEnemys = new GameObject[maxEnemy];
		StartCoroutine(SpawnMonster());
	}
	
	IEnumerator SpawnMonster()
	{
		while(true){ 
			Generate();
			yield return new WaitForSeconds( regenTime );
		}
	}

	void Generate()
	{
		for(int enemyCount = 0; enemyCount < existEnemys.Length; enemyCount++)
		{
			if( existEnemys[enemyCount] == null ){
				existEnemys[enemyCount] = Instantiate(enemyPrefab,transform.GetChild(enemyCount).transform.position,transform.rotation) as GameObject;
			}
		}
	}
	
}

using System.Collections;
using UnityEngine;
using WSGJ.Utils;

namespace WSGJ
{
	public class EntitiesSpawner : MonoSingleton<EntitiesSpawner>
	{
		[System.Serializable]
		class EntitiesSpawnerData
		{
			public Transform[] SpawnPoints;
			public GameObject[] EnemiesPrefabs;
			public int MinEnemiesPerSpawn = 1, MaxEnemiesPerSpawn = 5;
			public float DelayBetweenSpawnsMin = 0.2f;
			public float DelayBetweenSpawnsMax = 0.45f;
			public float DelayBetweenSpawnBatchMin = 2f;
			public float DelayBetweenSpawnBatchMax = 5f;
		}

		[SerializeField, Header("Spawner Settings")]
		EntitiesSpawnerData data;

		TruckController truckController;
		
		void Start()
		{
			StartCoroutine(SpawningCoroutine());
		}

		protected override void Awake()
		{
			base.Awake();
			truckController = GetComponent<TruckController>();
		}

		IEnumerator SpawningCoroutine()
		{
			while(true)
			{
				float randomBatchSpawnDelay = Random.Range(data.DelayBetweenSpawnBatchMin, data.DelayBetweenSpawnBatchMax);
				yield return new WaitForSeconds(randomBatchSpawnDelay);
				int enemiesToSpawn = Random.Range(data.MinEnemiesPerSpawn, data.MaxEnemiesPerSpawn);
				for(int i = 0; i < enemiesToSpawn; ++i)
				{
					var randomEnemy = data.EnemiesPrefabs.GetRandomElement();
					var randomSpawnPoint = data.SpawnPoints.GetRandomElement();
					var spawnedEnemy = Instantiate(randomEnemy, randomSpawnPoint.position, Quaternion.identity);
					float randomSpawnDelay = Random.Range(data.DelayBetweenSpawnsMin, data.DelayBetweenSpawnsMax);
					yield return new WaitForSeconds(randomSpawnDelay);
				}
			}
		}
	}
}
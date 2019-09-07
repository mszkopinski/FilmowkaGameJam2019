using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor.Validation;
using UnityEngine;
using WSGJ.Utils;

namespace WSGJ
{
	public class BlocksSpawner : MonoSingleton<BlocksSpawner>
	{
	    [System.Serializable]
	    class BlockSpawnerData
	    {
		    public float DelayBetweenSpawns = 1.5f;
		    public GameObject BlockPrefab = null;
	    }
	    
	    [SerializeField, Header("Spawner Settings")]
	    BlockSpawnerData spawnerData;

		readonly List<FallingBlock> spawnedBlocks = new List<FallingBlock>();
		IEnumerator blocksSpawningCoroutine = null;
		WaitForSeconds waitForSeconds;
		
		protected override void Awake()
		{
			base.Awake();

			FallingBlock.Spawned += OnAnyBlockSpawned;
		}

		void Start()
		{
			waitForSeconds = new WaitForSeconds(spawnerData.DelayBetweenSpawns);
			
			StartSpawning();
		}

		void OnDestroy()
		{
			FallingBlock.Spawned -= OnAnyBlockSpawned;
		}

		void StartSpawning()
		{
			if(blocksSpawningCoroutine != null)
				return;

			blocksSpawningCoroutine = BlocksSpawningCoroutine();
			StartCoroutine(blocksSpawningCoroutine);
		}

		void StopSpawning()
		{
			if(blocksSpawningCoroutine == null)
				return;
			
			StopCoroutine(blocksSpawningCoroutine);
			blocksSpawningCoroutine = null;
		}

		IEnumerator BlocksSpawningCoroutine()
		{
			while(true)
			{
				yield return waitForSeconds;
				Debug.Log("Should spawn block");
				var spawnedBlock = Instantiate(spawnerData.BlockPrefab, transform);
				spawnedBlock.transform.position = GetRandomTopPosition();
			}
		}

		Vector2 GetRandomTopPosition()
		{
			var mainCamera = CameraController.Instance.MainCamera;
			var topPosition = Vector2.zero;
			topPosition.y = Screen.height;
			topPosition.x = Screen.width / 2f; // add 
			return mainCamera.ScreenToWorldPoint(topPosition);
		}
		
		void OnAnyBlockSpawned(FallingBlock fallingBlock)
		{
			
		}
	}
}
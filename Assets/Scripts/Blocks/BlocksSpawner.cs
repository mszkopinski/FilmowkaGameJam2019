using System.Collections;
using System.Collections.Generic;
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
		    public GameObject[] BlockPrefabs = null;
	    }
	    
	    [SerializeField, Header("Spawner Settings")]
	    BlockSpawnerData spawnerData;

		readonly List<FallingBlock> spawnedBlocks = new List<FallingBlock>();
		IEnumerator blocksSpawningCoroutine;
		WaitForSeconds waitForSeconds;
		FallingBlock currentFallingBlock;
		
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

		void Update()
		{
			if(currentFallingBlock != null)
			{
				currentFallingBlock.SetSpeedUpActive(Input.GetKey(KeyCode.Space));
			}
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

		bool shouldSpawnNextBlock = true;

		IEnumerator BlocksSpawningCoroutine()
		{
			while(true)
			{
				if(shouldSpawnNextBlock)
				{
					var spawnedBlock = Instantiate(
							spawnerData.BlockPrefabs.GetRandomElement(), 
							Vector3.zero, 
							Quaternion.identity, null)
						.GetComponent<FallingBlock>();

					var targetPos = GetRandomTopPosition();
					targetPos.y += spawnedBlock.SpriteBounds.y / 2f;
					spawnedBlock.transform.position = targetPos;
				
					shouldSpawnNextBlock = false;	

					spawnedBlock.Placed += () =>
					{
						OnAnyBlockPlaced(spawnedBlock);
					};

					spawnedBlock.Destroyed += () =>
					{
						OnAnyBlockPlaced(spawnedBlock);
					};
				}
				
				yield return waitForSeconds;
			}
		}

		Vector2 GetRandomTopPosition()
		{
			var mainCamera = CameraController.Instance.MainCamera;
			var screenTopPosition = Vector3.zero;
			screenTopPosition.y = Screen.height;
			screenTopPosition.x = Screen.width / 2f; // add random horizontal offset
			screenTopPosition.z = mainCamera.nearClipPlane;
			var targetPos = mainCamera.ScreenToWorldPoint(screenTopPosition);
			return targetPos;
		}
		
		void OnAnyBlockSpawned(FallingBlock fallingBlock)
		{
			if(currentFallingBlock != null)
				return;
			
			currentFallingBlock = fallingBlock;
			currentFallingBlock.IsSelected = true;
		}

		void OnAnyBlockPlaced(FallingBlock fallingBlock)
		{
			if(currentFallingBlock != fallingBlock)
				return;
			
			currentFallingBlock.IsSelected = false;
			currentFallingBlock = null;
			
			shouldSpawnNextBlock = true;
		}
	}
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSGJ.Utils;
using Random = System.Random;

namespace WSGJ
{
	public class BlocksSpawner : MonoSingleton<BlocksSpawner>
	{
		public static event Action<FallingBlock> BlockToSpawnChanged; 
		
	    [Serializable]
	    class BlockSpawnerData
	    {
		    public float DelayBetweenSpawns = 1.5f;
		    public GameObject[] BlockPrefabs = null;
		    
		    public float ChanceToSpawnAttachment = 0.5f;
		    public GameObject[] BlockAttachments;
	    }
	    
	    [SerializeField, Header("Spawner Settings")]
	    BlockSpawnerData spawnerData;

		readonly List<FallingBlock> spawnedBlocks = new List<FallingBlock>();
		IEnumerator blocksSpawningCoroutine;
		WaitForSeconds waitForSeconds;
		FallingBlock currentFallingBlock;

		public FallingBlock NextBlockToSpawn
		{
			get
			{
				return nextBlockToSpawn;
			}
			private set
			{
				if(value == nextBlockToSpawn) return;
				nextBlockToSpawn = value;
				OnBlockToSpawnChanged(nextBlockToSpawn);
			}
		}

		FallingBlock nextBlockToSpawn;

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
				bool isSpacePressed = Input.GetKey(KeyCode.Space); 
				currentFallingBlock.SetSpeedUpActive(isSpacePressed);
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

			NextBlockToSpawn = spawnerData.BlockPrefabs.GetRandomElement()
				.GetComponent<FallingBlock>();
			
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
							NextBlockToSpawn,  
							Vector3.zero, 
							Quaternion.identity, null)
						.GetComponent<FallingBlock>();

					if(UnityEngine.Random.Range(0f, 1f) > spawnerData.ChanceToSpawnAttachment)
					{
						var spawnedAttachement = Instantiate(spawnerData.BlockAttachments.GetRandomElement());
						spawnedBlock.AttachUpgrade(spawnedAttachement);
					}
					
					var targetPos = CameraController.Instance.GetScreenTopPosition();
					targetPos.y += spawnedBlock.SpriteBounds.y / 2f;
					spawnedBlock.transform.position = targetPos;
				
					shouldSpawnNextBlock = false;
					NextBlockToSpawn = spawnerData.BlockPrefabs.GetRandomElement()
						.GetComponent<FallingBlock>();

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

		protected virtual void OnBlockToSpawnChanged(FallingBlock nextBlock)
		{
			BlockToSpawnChanged?.Invoke(nextBlock);
		}
	}
}
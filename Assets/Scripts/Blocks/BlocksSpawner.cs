using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSGJ.Utils;

namespace WSGJ
{
	public class BlocksSpawner : MonoSingleton<BlocksSpawner>
	{
		public static event Action<FallingBlockWrapper> BlockToSpawnChanged; 
		
	    [Serializable]
	    class BlockSpawnerData
	    {
		    public float DelayBetweenSpawns = 1.5f;
		    public GameObject[] BlockPrefabs = null;
		    
		    public float ChanceToSpawnAttachment = 0.5f;
		    public GameObject[] BlockAttachments;
	    }

	    public struct FallingBlockWrapper
	    {
		    public FallingBlock Block;
		    public Color RandomTint;
	    }
	    
	    [SerializeField, Header("Spawner Settings")]
	    BlockSpawnerData spawnerData;
	    [SerializeField]
	    Color darkestTintValue;

		readonly List<FallingBlock> spawnedBlocks = new List<FallingBlock>();
		IEnumerator blocksSpawningCoroutine;
		WaitForSeconds waitForSeconds;
		FallingBlock currentFallingBlock;

		public FallingBlockWrapper NextBlockToSpawn
		{
			get
			{
				return nextBlockToSpawn;
			}
			private set
			{
				nextBlockToSpawn = value;
				OnBlockToSpawnChanged(nextBlockToSpawn);
			}
		}

		FallingBlockWrapper nextBlockToSpawn;

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

			NextBlockToSpawn = new FallingBlockWrapper
			{
				Block = spawnerData.BlockPrefabs.GetRandomElement()
					.GetComponent<FallingBlock>(),
				RandomTint = GetRandomTintColor()
			};
			
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
							NextBlockToSpawn.Block,  
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
					
					spawnedBlock.SetSpriteTint(nextBlockToSpawn.RandomTint);
				
					shouldSpawnNextBlock = false;
					NextBlockToSpawn = new FallingBlockWrapper
					{
						Block = spawnerData.BlockPrefabs.GetRandomElement()
							.GetComponent<FallingBlock>(),
						RandomTint = GetRandomTintColor()
					};

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
		
		public Color GetRandomTintColor()
		{
			var tintColor = Color.Lerp(Color.white, darkestTintValue, UnityEngine.Random.Range(0f, 1f));
			tintColor.a = 1f;
			return tintColor;
		}

		protected virtual void OnBlockToSpawnChanged(FallingBlockWrapper nextBlock)
		{
			BlockToSpawnChanged?.Invoke(nextBlock);
		}
	}
}
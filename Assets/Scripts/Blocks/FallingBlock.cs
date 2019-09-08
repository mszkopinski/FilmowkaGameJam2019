using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using WSGJ.Utils;

namespace WSGJ
{
	public class FallingBlock : MonoBehaviour
	{
		const float RotateTransitionTime = .25f;

		public static event Action<FallingBlock> Spawned;
		public event Action Placed, Destroyed;
		
		public bool IsSelected { get; set; }
		
		public bool IsAttachedToTruck
		{
			get => AttachedTruck != null;
		}
		
		public TruckController AttachedTruck { get; private set; }

		public Vector2 SpriteSize
		{
			get
			{
				if (spriteRenderer == null)
					return Vector2.zero;

				return spriteRenderer.bounds.size;
			}
		}

		public Sprite BlockSprite
		{
			get
			{
				if(spriteRenderer == null)
					spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);

				return spriteRenderer.sprite;
			}
		}

		
		[SerializeField, Header("Block Settings")]
		float defaultVelocity = 2f;
		[SerializeField]
		float boostedVelocity = 5f;
		[SerializeField]
		float horizontalStepSize = 1.5f;
		[SerializeField]
		Transform attachmentSlot;
		[SerializeField]
		GameObject explosionParticles;
	
		bool canRotateBlock = true;
		bool canMoveBlock = true;
		SpriteRenderer spriteRenderer;
		Rigidbody2D rb;
		float currentVelocity;
		const float transitionTime = .25f;
		AudioSource thisAudioSource;

		
		void Awake()
		{
			spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
			
			rb = GetComponent<Rigidbody2D>();
			thisAudioSource = GetComponent<AudioSource>();
			
			OnSpawned(this);
		}

		void Start()
		{
			currentVelocity = defaultVelocity;
		}

		void Update()
		{
            
            HandleAttachment();

            if (IsAttachedToTruck || !IsSelected)
				return;

			HandleMovement();
			HandleRotation();

        }

        private void OnDestroy()
        {
            if (attachmentSlot != null) Destroy(attachmentSlot.gameObject);
        }

        void HandleAttachment()
        {
            if (attachmentSlot.parent != null) return;
            attachmentSlot.position = transform.position;
            attachmentSlot.rotation = transform.rotation;
        }

		void HandleMovement()
		{
			var targetPosition = transform.position;
			targetPosition.y -= currentVelocity * Time.deltaTime;
			rb.MovePosition(targetPosition);

			bool isRightPressed = Input.GetKeyDown(KeyCode.RightArrow);
			bool isLeftPressed = Input.GetKeyDown(KeyCode.LeftArrow);

			if(canMoveBlock && (isRightPressed || isLeftPressed) 
			                && currentVelocity != boostedVelocity)
			{
				var targetPosX = transform.position.x 
				                 + (isRightPressed ? horizontalStepSize : -horizontalStepSize);

				transform.DOLocalMoveX(targetPosX, transitionTime)
					.SetEase(Ease.InOutCubic)
					.OnStart(() => { canMoveBlock = false; })
					.OnComplete(() => { canMoveBlock = true; });
			}
		}
		
		void HandleRotation()
		{
			bool isRotatingRight = Input.GetKeyDown(KeyCode.UpArrow);
			bool isRotatingLeft = Input.GetKeyDown(KeyCode.DownArrow);
			
			if(canRotateBlock && (isRotatingLeft || isRotatingRight))
			{
				var targetRotation = transform.localRotation.eulerAngles;
				targetRotation.z = targetRotation.z + (isRotatingLeft ? 90f : -90f);
				transform.DORotate(targetRotation, RotateTransitionTime)
					.OnStart(() => { canRotateBlock = false; })
					.OnComplete(() =>
					{
						canRotateBlock = true; 
						Debug.Log(transform.right.ToString());
					});
			}
		}
		
		void OnCollisionEnter2D(Collision2D col)
		{
			if(col.HasCollidedWithGround())
			{
				SoundManager.Instance.PlayBlockHitsGround();
				OnBlockDestroyed();	
			}
			
			if(col.HasCollidedWithBlock())
			{
				OnCollisionWithOtherBlock(col);
			}
		}

		const float raycastOffset = 0.1f;

		void OnCollisionWithOtherBlock(Collision2D colInfo)
		{
			if(IsSelected == false)
				return;
			
			bool hasBlockUnder = false;
			
			var bounds = colInfo.collider.bounds;

			var leftMostPos = colInfo.collider.transform.TransformPoint((Vector2)bounds.center +
			                                                         new Vector2(-bounds.size.x * .9f, -bounds.size.y) * 0.5f);
			
			var rightMostPos = colInfo.collider.transform.TransformPoint((Vector2)bounds.center +
			                                                            new Vector2(bounds.size.x * .9f, -bounds.size.y) * 0.5f);
			var centerPos = transform.position;
			
			var positionsToCheck = new List<Vector2>
			{
				leftMostPos, rightMostPos, centerPos
			};

			foreach(var posToCheck in positionsToCheck)
			{
				if(Physics2D.Raycast(posToCheck, -Vector2.up, .05f,
					LayerMask.GetMask("Blocks")))
				{
					hasBlockUnder = true;
					break;
				}
			}
			
			if(!hasBlockUnder)
			{
				Debug.Log("THIS BLOCK SHOULDNT HAVE ATTACHED.");
				OnBlockDestroyed();
				return;
			}
			
			thisAudioSource.clip = SoundManager.Instance.BlockHitsOtherBlock;
			thisAudioSource.PlayOneShot(SoundManager.Instance.BlockHitsOtherBlock);
			
			var otherBlock = colInfo.collider.GetComponentInParent<FallingBlock>();
			if(otherBlock != null && otherBlock.IsAttachedToTruck)
			{
				var truckController = otherBlock.AttachedTruck;
				OnBlockPlaced(truckController);
				truckController.OnBlockAttached(this);
			}
		}

		void OnTriggerEnter2D(Collider2D col)
		{
			if(col.HasCollidedWithTruck() && !IsAttachedToTruck)
			{
				thisAudioSource.clip = SoundManager.Instance.BlockHitsTheCart;
				thisAudioSource.PlayOneShot(SoundManager.Instance.BlockHitsTheCart);
				TruckController truckController = null;
				if((truckController = col.GetComponentInParent<TruckController>()) != null)
				{
					OnBlockPlaced(truckController);
					truckController.OnBlockAttached(this);
				}			
			}
		}

		void OnBlockDestroyed()
		{
			transform.DOScale(Vector3.zero, .15f)
				.SetEase(Ease.InOutBounce)
				.OnComplete(() =>
				{
					Destroyed?.Invoke();
					var explosion = Instantiate(explosionParticles);
					explosion.transform.position = transform.position;
					CameraController.Instance.ShakeCamera(1.2f);
					Destroy(gameObject);
				});
		}

		bool isSpeedUpActive = false;
		
		public void SetSpeedUpActive(bool isActive)
		{
			if(isActive == isSpeedUpActive)
				return;

			isSpeedUpActive = isActive;
			currentVelocity = isSpeedUpActive ? boostedVelocity : defaultVelocity;
			// CameraController.Instance.PlaySpeedUpEffect(
			// 	isSpeedUpActive ? -18f : 0f, 
			// 	isSpeedUpActive ? 0.596f : 0.364f);
		}

		public void AttachUpgrade(GameObject attachment)
		{
			if(attachment == null || attachmentSlot == null)
				return;
			
			var attachmentTransform = attachment.transform;
			attachmentTransform.SetParent(attachmentSlot);
			attachmentTransform.localPosition = Vector3.zero;
            attachmentSlot.parent = null;

        }

		public void SetSpriteTint(Color tintColor)
		{
			spriteRenderer.color = tintColor;
		}

		protected virtual void OnSpawned(FallingBlock @this)
		{
			Spawned?.Invoke(@this);
		}

		protected virtual void OnBlockPlaced(TruckController truckController)
		{
			AttachedTruck = truckController;
			
			StopAllCoroutines();

			transform.SetParent(AttachedTruck.transform);
			
			rb.isKinematic = true;
			rb.velocity = Vector2.zero;

			Placed?.Invoke();
			CameraController.Instance.ShakeCamera(1f + AttachedTruck.AttachedBlocksCounter * .05f);

			if(transform.position.y > CameraController.Instance.GetScreenTopPosition().y - 1.5f)
			{
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			}
		}
	}
}
using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using WSGJ.Utils;

namespace WSGJ
{
	public class FallingBlock : MonoBehaviour
	{
		const float RotateTransitionTime = .25f;

		public static event Action<FallingBlock> Spawned;
		public event Action Placed, Destroyed;
		
		public Vector2 SpriteBounds
		{
			get
			{
				if (spriteRenderer == null)
					return Vector2.zero;

				return spriteRenderer.bounds.size;
			}
		}
		
		[SerializeField, Header("Block Settings")]
		float defaultVelocity = 2f;
		[SerializeField]
		float boostedVelocity = 5f;
		[SerializeField]
		float verticalStepSize = 1.5f;
		[SerializeField]
		float horizontalStepSize = 1.5f;

		bool canRotateBlock = true;
		bool canMoveBlock = true;
		SpriteRenderer spriteRenderer;
		Rigidbody2D rb;
		float currentVelocity;
		
		void Awake()
		{
			OnSpawned(this);
			spriteRenderer = GetComponentInChildren<SpriteRenderer>();
			rb = GetComponent<Rigidbody2D>();
		}

		void Start()
		{
			currentVelocity = defaultVelocity;
		}

		void Update()
		{
			HandleMovement();
			HandleRotation();
		}

		public void SetSpeedUpActive(bool isActive)
		{
			currentVelocity = isActive ? boostedVelocity : defaultVelocity;
		}

		void HandleMovement()
		{
			var targetPosition = transform.position;
			targetPosition.y -= currentVelocity * Time.deltaTime;
			rb.MovePosition(targetPosition);
			
			if(canMoveBlock)
			{
				const float transitionTime = .25f;
				
				if(Input.GetKeyDown(KeyCode.RightArrow))
				{
					var targetPos = transform.position;
					targetPos.x += horizontalStepSize;
					rb.DOMove(targetPos, transitionTime)
						.SetEase(Ease.InOutCubic)
						.OnStart(() => { canMoveBlock = false; })
						.OnComplete(() =>
						{
							canMoveBlock = true;
						});	
				}
			
				if(Input.GetKeyDown(KeyCode.LeftArrow))
				{
					var targetPos = transform.position;
					targetPos.x -= horizontalStepSize;
					rb.DOMove(targetPos, transitionTime)
						.SetEase(Ease.InOutCubic)
						.OnStart(() => { canMoveBlock = false; })
						.OnComplete(() =>
						{
							canMoveBlock = true;
						});	
				}
			}
		}
		
		void HandleRotation()
		{
			if(Input.GetKeyDown(KeyCode.R))
			{
				RotateBlock();
			}
		}

		void RotateBlock()
		{
			if(!canRotateBlock)
				return;
			
			var targetRotation = transform.localRotation.eulerAngles;
			targetRotation.z -= 90f;
			transform.DOLocalRotate(targetRotation, RotateTransitionTime).OnStart(() =>
			{
				canRotateBlock = false;
			}).OnComplete(() => { canRotateBlock = true; });
		}
		
		void OnCollisionEnter2D(Collision2D collision2D)
		{
			var col = collision2D.collider;
			
			if(collision2D.HasCollidedWithGround())
			{
				OnBlockDestroyed();
			}
			else if(collision2D.HasCollidedWithTruck())
			{
				OnBlockPlaced();
				
				TruckController truckController = null;
				if((truckController = col.GetComponentInParent<TruckController>()) != null)
				{
					Debug.Log("jazda");
					StopAllCoroutines();
					truckController.OnFallingBlockAttached(this);
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
					Destroy(gameObject);
				});
		}

		protected virtual void OnSpawned(FallingBlock @this)
		{
			Spawned?.Invoke(@this);
		}

		protected virtual void OnBlockPlaced()
		{
			Placed?.Invoke();
			canMoveBlock = false;
		}
	}
}
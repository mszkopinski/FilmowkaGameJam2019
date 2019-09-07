using System;
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
		
		public bool IsSelected { get; set; }
		
		public bool IsAttachedToTruck
		{
			get => AttachedTruck != null;
		}
		public TruckController AttachedTruck { get; private set; }

		public Vector2 SpriteBounds
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

		bool canRotateBlock = true;
		bool canMoveBlock = true;
		SpriteRenderer spriteRenderer;
		Rigidbody2D rb;
		float currentVelocity;
		const float transitionTime = .25f;
		
		void Awake()
		{
			OnSpawned(this);
			spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
			rb = GetComponent<Rigidbody2D>();
		}

		void Start()
		{
			currentVelocity = defaultVelocity;
		}

		void Update()
		{
			if(IsAttachedToTruck || !IsSelected)
				return;

			HandleMovement();
			HandleRotation();
		}

		void HandleMovement()
		{
			var targetPosition = transform.position;
			targetPosition.y -= currentVelocity * Time.deltaTime;
			rb.MovePosition(targetPosition);

			bool isRightPressed = Input.GetKeyDown(KeyCode.RightArrow);
			bool isLeftPressed = Input.GetKeyDown(KeyCode.LeftArrow);

			if(canMoveBlock && (isRightPressed || isLeftPressed))
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
			if(Input.GetKeyDown(KeyCode.R) && canRotateBlock)
			{
				var targetRotation = transform.localRotation.eulerAngles;
				targetRotation.z -= 90f;
				transform.DOLocalRotate(targetRotation, RotateTransitionTime)
					.OnStart(() => { canRotateBlock = false; })
					.OnComplete(() => { canRotateBlock = true; });
			}
		}
		
		void OnCollisionEnter2D(Collision2D collision2D)
		{
			if(collision2D.HasCollidedWithGround())
			{
				Debug.Log("BLOCK SHOULD'VE DESTROYED");
				OnBlockDestroyed();	
			}

			if(collision2D.HasCollidedWithBlock())
			{
				var otherBlock = collision2D.collider.GetComponent<FallingBlock>();
				if(otherBlock != null && otherBlock.IsAttachedToTruck)
				{
					var truckController = otherBlock.AttachedTruck;
					OnBlockPlaced(truckController);
				}
			}
		}

		void OnTriggerEnter2D(Collider2D col)
		{
			if(col.HasCollidedWithTruck() && !IsAttachedToTruck)
			{
				TruckController truckController = null;
				if((truckController = col.GetComponentInParent<TruckController>()) != null)
				{
					OnBlockPlaced(truckController);
					truckController.OnBlockDropped(this);
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
		
		public void SetSpeedUpActive(bool isActive)
		{
			currentVelocity = isActive ? boostedVelocity : defaultVelocity;
		}

		protected virtual void OnSpawned(FallingBlock @this)
		{
			Spawned?.Invoke(@this);
		}

		protected virtual void OnBlockPlaced(TruckController truckController)
		{
			AttachedTruck = truckController;
			
			StopAllCoroutines();

			transform.SetParent(truckController.transform);
			
			rb.isKinematic = true;
			rb.velocity = Vector2.zero;

			Placed?.Invoke();
		}
	}
}
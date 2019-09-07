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
		float defaultStepTime = .5f;
		[SerializeField]
		float boostedStepTime = .1f;
		[SerializeField]
		float verticalStepSize = 1.5f;
		[SerializeField]
		float horizontalStepSize = 1.5f;

		bool canRotateBlock = true;
		bool canMoveBlock = true;
		SpriteRenderer spriteRenderer;
		float currentStepTime;
		Rigidbody2D rb;
		
		void Awake()
		{
			OnSpawned(this);
			spriteRenderer = GetComponentInChildren<SpriteRenderer>();
			rb = GetComponent<Rigidbody2D>();
		}

		void Start()
		{
			StartCoroutine(FallCoroutine());

			currentStepTime = defaultStepTime;
		}

		void Update()
		{
			HandleInput();
		}

		public void SetSpeedUpActive(bool isActive)
		{
			currentStepTime = isActive ? boostedStepTime : defaultStepTime;
		}
		
		void HandleInput()
		{
			if(Input.GetKeyDown(KeyCode.R))
			{
				RotateBlock();
			}

			if(canMoveBlock)
			{
				const float transitionTime = .25f;
				
				if(Input.GetKeyDown(KeyCode.RightArrow))
				{
					var targetPosition = transform.position;
					targetPosition.x += horizontalStepSize;
					rb.DOMove(targetPosition, transitionTime)
						.SetEase(Ease.InOutCubic)
						.OnStart(() => { canMoveBlock = false; })
						.OnComplete(() =>
						{
							canMoveBlock = true;
						});	
				}
			
				if(Input.GetKeyDown(KeyCode.LeftArrow))
				{
					var targetPosition = transform.position;
					targetPosition.x -= horizontalStepSize;
					rb.DOMove(targetPosition, transitionTime)
						.SetEase(Ease.InOutCubic)
						.OnStart(() => { canMoveBlock = false; })
						.OnComplete(() =>
						{
							canMoveBlock = true;
						});	
				}
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
		
		IEnumerator FallCoroutine()
		{
			while(true)
			{
				yield return new WaitForSeconds(currentStepTime);
				var targetPosition = transform.position;
				targetPosition.y -= verticalStepSize;
				rb.DOMove(targetPosition, .25f).SetEase(Ease.InOutCubic);
			}
		}

		void OnCollisionEnter2D(Collision2D col)
		{
			var collider = col.collider;
			
			if(col.HasCollidedWithGround())
			{
				OnBlockDestroyed();
			}
			else if(col.HasCollidedWithTruck())
			{
				OnBlockPlaced();
				
				TruckController truckController = null;
				if((truckController = collider.GetComponentInParent<TruckController>()) != null)
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
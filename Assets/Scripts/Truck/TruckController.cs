using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using WSGJ.Utils;

namespace WSGJ
{
	public class TruckController : MonoBehaviour
	{
		public static event Action<float, float> HealthChanged;

		public float CurrentHealth
		{
			get
			{
				return currentHealth;
			}
			private set
			{
				if(value == currentHealth) return;
				currentHealth = value;
				OnHealthChanged(currentHealth, startHealth);
			}
		}

		[SerializeField, Header("Truck Settings")]
		List<Transform> pathNodes;

		[SerializeField]
		float startHealth = 100f;

		[SerializeField]
		float movementVelocity = 2f;
		[SerializeField]
		List<Transform> wheelTransforms;

		Rigidbody2D rigidBody;
		Transform currentTargetNode;
		Animator animator;
		float currentHealth;
		int attachedBlocksCounter = 0;
		
		readonly int wheatDroppedAnimHash = Animator.StringToHash("isWheatDropped");

		void Awake()
		{
			rigidBody = GetComponent<Rigidbody2D>();
			animator = GetComponent<Animator>();
		}

		void Start()
		{
			currentTargetNode = pathNodes.GetRandomElement();
			CurrentHealth = startHealth;
		}

		void Update()
		{
			if(Input.GetKeyDown(KeyCode.K))
			{
				OnBlockAttached(null);
			}
			
			if((UnityEngine.Object)currentTargetNode == null)
				return;

			var currPos = rigidBody.transform.position;
			var targetPos = currentTargetNode.position;
			
			var targetDir = (targetPos - currPos).normalized;
			var distance = (currPos - targetPos).sqrMagnitude;

			// Debug.Log(distance.ToString(CultureInfo.InvariantCulture));
			if(distance < 3f)
			{
				currentTargetNode = pathNodes.FirstOrDefault(n => n != currentTargetNode);
				return;
			}
			
			rigidBody.MovePosition(currPos + movementVelocity * Time.deltaTime * targetDir);

			foreach(var wheelTransform in wheelTransforms)
			{
				var targetRotation = wheelTransform.localRotation.eulerAngles;
				targetRotation.z += 300f * Time.deltaTime;
				wheelTransform.localEulerAngles = targetRotation;
			}
		}
		
		public void OnBlockAttached(FallingBlock fallingBlock)
		{
			animator.SetTrigger(wheatDroppedAnimHash);
			++attachedBlocksCounter;
		}

		public void OnDamageTaken(float dmgValue)
		{
			CurrentHealth -= dmgValue;
		}

		protected virtual void OnHealthChanged(float newAmount, float maxAmount)
		{
			HealthChanged?.Invoke(newAmount, maxAmount);
		}
	}
}
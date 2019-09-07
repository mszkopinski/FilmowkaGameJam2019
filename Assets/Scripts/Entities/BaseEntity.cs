using System;
using DragonBones;
using UnityEngine;
using Transform = UnityEngine.Transform;

namespace WSGJ
{
	public abstract class BaseEntity : MonoBehaviour
	{
		[SerializeField, Header("Entity Settings")]
		float movementSpeed;

		Transform currentTarget;
		protected UnityArmatureComponent ArmatureComponent;
		protected bool CanMove = true;

		void Awake()
		{
			ArmatureComponent = GetComponentInChildren<UnityArmatureComponent>(true);
			SetTarget(FindObjectOfType<TruckController>()?.transform);
		}

		void Update()
		{
			if(CanMove == false)
				return;
			
			if((UnityEngine.Object)currentTarget == null)
				return;
			
			var dir = currentTarget.position - transform.position;
			var distance = dir.sqrMagnitude;

			var newScale = transform.localScale;
			newScale.x = Math.Sign(dir.x) > 0 ? -1f : 1f;
			transform.localScale = newScale;
			
			transform.position += movementSpeed * Time.deltaTime * Math.Sign(dir.x) * transform.right;

			if(!(distance < 25f))
			{			
				ArmatureComponent.animation.Play("attack");
			}

		}

		protected virtual void SetTarget(Transform target)
		{
			currentTarget = target;
		}

		public virtual void OnEntityDied()
		{
			CanMove = false;
		}
	}
}
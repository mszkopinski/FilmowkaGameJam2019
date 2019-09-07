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
		protected bool IsDead = false;

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

			bool isInAttackRange = distance < 25f;

			if(!isInAttackRange)
			{
				transform.position += movementSpeed * Time.deltaTime * Math.Sign(dir.x) * transform.right;

				if(ArmatureComponent.animationName != "move")
				{
					ArmatureComponent.animation.Play("move", -1);
				}
			}
			else
			{
				if(ArmatureComponent.animationName != "atack")
				{
					ArmatureComponent.animation.Play("atack", 1);
				}
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
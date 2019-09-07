using System;
using System.Collections;
using DragonBones;
using TMPro;
using UnityEngine;
using Transform = UnityEngine.Transform;

namespace WSGJ
{
	public abstract class BaseEntity : MonoBehaviour
	{
		[SerializeField, Header("Entity Settings")]
		float movementSpeed;
		[SerializeField]
		float attackDamage = 10f;
		[SerializeField]
		float delayBetweenAttacks = 2f;

		Transform currentTarget;
		TruckController truckController;
		protected UnityArmatureComponent ArmatureComponent;
		protected bool CanMove = true;
		protected bool IsDead = false;

		IEnumerator attackCoroutine;

		void Awake()
		{
			ArmatureComponent = GetComponentInChildren<UnityArmatureComponent>(true);
			SetTarget(FindObjectOfType<TruckController>()?.transform);
		}

		void Update()
		{
			if(CanMove == false || IsDead)
				return;
			
			if((UnityEngine.Object)currentTarget == null)
				return;
			
			var dir = currentTarget.position - transform.position;
			var distance = dir.sqrMagnitude;

			var newScale = transform.localScale;
			newScale.x = Math.Sign(dir.x) > 0 ? -1f : 1f;
			transform.localScale = newScale;

			bool isInAttackRange = distance < 25f;
			
			SetAttackState(isInAttackRange);

			if(!isInAttackRange)
			{
				transform.position += movementSpeed * Time.deltaTime * Math.Sign(dir.x) * transform.right;

				if(ArmatureComponent.animationName != "move")
				{
					ArmatureComponent.animation.Play("move", -1);
				}
			}
		}

		protected virtual void SetTarget(Transform target)
		{
			currentTarget = target;
			truckController = currentTarget.GetComponent<TruckController>();
		}

		void SetAttackState(bool isAttacking)
		{
			if(isAttacking && attackCoroutine == null)
			{
				attackCoroutine = AttackWithDelay();
				StartCoroutine(attackCoroutine);
			}

			if(!isAttacking && attackCoroutine != null)
			{
				StopCoroutine(attackCoroutine);
				attackCoroutine = null;
				
				if(ArmatureComponent.animationName != "move")
				{
					ArmatureComponent.animation.Play("move", -1);
				}
			}
		}

		IEnumerator AttackWithDelay()
		{
			while(true)
			{
				if(IsDead)
					yield break;
				
				yield return new WaitForSeconds(delayBetweenAttacks);
				
				if(IsDead)
					yield break;
				
				truckController.OnDamageTaken(attackDamage);
				ArmatureComponent.animation.Play("atack", 1);
			}
		}

		void OnTriggerEnter2D(Collider2D other)
		{
			if(other.CompareTag("Weapon") == false) return;
			OnEntityDied();
		}

		public virtual void OnEntityDied()
		{
			CanMove = false;
		}
	}
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WSGJ
{
	public class TruckLethalRange : MonoBehaviour
	{
		[SerializeField]
		float destructionRange = 9f;

		readonly List<BaseEntity> entitiesInRange = new List<BaseEntity>();
		CircleCollider2D circleCollider2D;
		Func<Collider2D, bool> IsEntityFunc = col => col.CompareTag("Entity");

		void Awake()
		{
			circleCollider2D = GetComponent<CircleCollider2D>();
		}

		void Start()
		{
			circleCollider2D.radius = destructionRange;
		}
		
		void OnTriggerEnter2D(Collider2D col)
		{
			if(!IsEntityFunc.Invoke(col)) return;
			var baseEntity = col.GetComponentInParent<BaseEntity>();
			if(baseEntity != null && entitiesInRange.Contains(baseEntity) == false)
			{
				entitiesInRange.Add(baseEntity);
			}
		}
		
		void OnTriggerExit2D(Collider2D col)
		{
			if(!IsEntityFunc.Invoke(col)) return;
			var baseEntity = col.GetComponentInParent<BaseEntity>();
			if(baseEntity != null && entitiesInRange.Contains(baseEntity))
			{
				entitiesInRange.Remove(baseEntity);
			}
		}

		public void DestroyEntitiesInRange()
		{
			foreach(var baseEntity in entitiesInRange)
			{
				baseEntity.OnEntityDied();
			}
		}
		
		void OnDrawGizmos()
		{
			Gizmos.DrawWireSphere(transform.position, destructionRange);
		}
	}
}
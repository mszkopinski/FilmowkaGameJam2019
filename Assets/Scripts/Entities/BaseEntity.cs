using System;
using System.Globalization;
using UnityEngine;

namespace WSGJ
{
	public abstract class BaseEntity : MonoBehaviour
	{
		[SerializeField, Header("Entity Settings")]
		float movementSpeed;

		Transform currentTarget;

		void Awake()
		{
			SetTarget(FindObjectOfType<TruckController>()?.transform);
		}

		void Update()
		{
			if((UnityEngine.Object)currentTarget == null)
				return;
			
			var dir = currentTarget.position - transform.position;
			var distance = dir.sqrMagnitude;

			var newScale = transform.localScale;
			newScale.x = Math.Sign(dir.x) > 0 ? -1f : 1f;
			transform.localScale = newScale;
			
			if(!(distance < 50f))
			{
				transform.position += movementSpeed * Time.deltaTime * transform.right;
			}
		}

		protected virtual void SetTarget(Transform target)
		{
			currentTarget = target;
		}
	}
}
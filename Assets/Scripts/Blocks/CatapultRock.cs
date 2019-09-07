using System;
using UnityEngine;

namespace WSGJ
{
	public class CatapultRock : MonoBehaviour
	{
		[SerializeField]
		float projectileVelocity = 9f;
		[SerializeField]
		float lifeTime = 3f;

		bool isLaunched = false;
		Vector2 launchVector;
		
		void Awake()
		{
			Invoke(nameof(DestroyProjectile), lifeTime);
		}

		void Update()
		{
			if(!isLaunched)
				return;

			transform.position += (Vector3)(projectileVelocity * Time.deltaTime * launchVector);
		}

		void OnTriggerEnter2D(Collider2D other)
		{
			if(other.CompareTag("Entity") == false) return;
			var entityComponent = other.GetComponentInParent<BaseEntity>();
			if(entityComponent != null)
			{
				entityComponent.OnEntityDied();
				DestroyProjectile();
			}
		}

		public void Launch(Vector2 launchDir)
		{
			transform.SetParent(null);
			launchVector = launchDir;
			Debug.Log(launchVector.ToString());
			isLaunched = true;
		}

		void DestroyProjectile()
		{
			Destroy(gameObject);
		}
	}
}

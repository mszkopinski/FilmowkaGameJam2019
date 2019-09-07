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
			Invoke(nameof(Destroy), lifeTime);
		}

		void Update()
		{
			if(!isLaunched)
				return;
			
			transform.Translate(projectileVelocity * Time.deltaTime * launchVector);
		}
		
		public void Launch(Vector2 launchDir)
		{
			transform.SetParent(null);
			launchVector = launchDir;
			isLaunched = true;
		}

		void Destroy()
		{
			Destroy(this);
		}
	}
}

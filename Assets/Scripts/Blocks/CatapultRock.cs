using UnityEngine;

namespace WSGJ
{
	public class CatapultRock : MonoBehaviour
	{
		[SerializeField]
		float projectileVelocity = 5f;

		bool isLaunched = false;
		Vector2 launchVector;

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
	}
}

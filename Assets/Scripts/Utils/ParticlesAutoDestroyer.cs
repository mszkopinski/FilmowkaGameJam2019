using UnityEngine;

namespace WSGJ.Utils
{
	[RequireComponent(typeof(ParticleSystem))]
	public class ParticlesAutoDestroyer : MonoBehaviour
	{
		ParticleSystem particleSystem;

		void Awake()
		{
			particleSystem = GetComponent<ParticleSystem>();
		}

		void Update()
		{
			if((UnityEngine.Object)particleSystem == null)
				return;

			if(particleSystem.IsAlive() == false)
			{
				Destroy(this);
			}
		}
	}
}
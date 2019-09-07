using System;
using UnityEngine;

namespace WSGJ
{
	public class FallingBlock : MonoBehaviour
	{
		public static event Action<FallingBlock> Spawned;
		
		void Awake()
		{
			OnSpawned(this);
		}

		protected virtual void OnSpawned(FallingBlock @this)
		{
			Spawned?.Invoke(@this);
		}
	}
}
using System.Linq;
using UnityEngine;
	
namespace WSGJ.Utils
{
	public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		[SerializeField]
		bool persistBetweenScenes = true;

		public static T Instance
		{
			get
			{
				if(instance != null)
					return instance;

				var components = Object.FindObjectsOfType<T>().ToList();
				instance = components.Count > 0 ? components.FirstOrDefault() :
					new GameObject($"S{typeof(T)}_SINGLETON").AddComponent<T>();

				return instance;
			}
		}

		static T instance;

		protected virtual void Awake()
		{
			if(persistBetweenScenes)
			{
				DontDestroyOnLoad(this);
			}
		}
	}
}

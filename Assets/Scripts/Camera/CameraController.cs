using UnityEngine;
using WSGJ.Utils;

namespace WSGJ
{
	[RequireComponent(typeof(Camera))]
	public class CameraController : MonoSingleton<CameraController>
	{
		public Camera MainCamera { get; private set; }

		protected override void Awake()
		{
			base.Awake();
			MainCamera = GetComponent<Camera>();
		}
	}
}
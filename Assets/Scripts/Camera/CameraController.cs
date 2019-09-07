using DG.Tweening;
using UnityEngine;
using WSGJ.Utils;

namespace WSGJ
{
	[RequireComponent(typeof(Camera))]
	public class CameraController : MonoSingleton<CameraController>
	{
		[SerializeField, Header("Camera Shake")]
		float cameraShakeDuration = 0.1f;
		[SerializeField]
		float cameraShakeStrenght = 0.05f;
		[SerializeField]
		int cameraShakeVibrato = 20;
		[SerializeField]
		float cameraShakeRandomness = 90f;
		[SerializeField]
		bool cameraShakeFadeOut = false;
		
		
		public Camera MainCamera { get; private set; }

		void Update()
		{
			if(Input.GetKeyDown(KeyCode.L))
			{
				ShakeCamera();				
			}
		}

		public void ShakeCamera(float strengthFactor = 1f)
		{
			MainCamera.DOShakePosition(cameraShakeDuration, 
				cameraShakeStrenght * strengthFactor, 
				cameraShakeVibrato, 
				cameraShakeRandomness, 
				cameraShakeFadeOut);
		}

		protected override void Awake()
		{
			base.Awake();
			MainCamera = GetComponent<Camera>();
		}
	}
}
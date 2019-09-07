using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
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
        public Rect Rectangle => MainCamera.rect;
		PostProcessVolume ppVolume;
		
		protected override void Awake()
		{
			base.Awake();
			MainCamera = GetComponent<Camera>();

			ppVolume = GetComponent<PostProcessVolume>();
		}

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

		public Vector2 GetScreenTopPosition()
		{
			var screenTopPosition = Vector3.zero;
			screenTopPosition.y = Screen.height;
			screenTopPosition.x = Screen.width / 2f;
			screenTopPosition.z = MainCamera.nearClipPlane;
			var targetPos = MainCamera.ScreenToWorldPoint(screenTopPosition);
			return targetPos;
		}

		public void PlaySpeedUpEffect(float intensityValue, float vignetteValue)
		{
			ppVolume.profile.TryGetSettings<LensDistortion>(out var lensDistortion);
			ppVolume.profile.TryGetSettings<Vignette>(out var vignette);

			if(lensDistortion != null)
			{
				DOTween.Sequence().Append(
					DOTween.To(() => lensDistortion.intensity,
						x => lensDistortion.intensity.Override(x),
						intensityValue,
						.2f)).SetEase(Ease.InOutCubic);	
			}

			if(vignette != null)
			{
				DOTween.Sequence().Append(
					DOTween.To(() => vignette.intensity,
						x => vignette.intensity.Override(x),
						vignetteValue,
						.2f)).SetEase(Ease.InOutCubic);	
			}
		}
	}
}
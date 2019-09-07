using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using WSGJ.Utils;

namespace WSGJ
{
	public class GameGUI : MonoSingleton<GameGUI>
	{
		[SerializeField, Header("UI SETTINGS")]
		Image nextBlockImage;
		[SerializeField]
		Image healthBarFill;

		protected override void Awake()
		{
			base.Awake();
			BlocksSpawner.BlockToSpawnChanged += OnNextBlockToSpawnChanged;
			TruckController.HealthChanged += OnHealthChanged;
		}

		void OnDestroy()
		{
			BlocksSpawner.BlockToSpawnChanged -= OnNextBlockToSpawnChanged;
			TruckController.HealthChanged -= OnHealthChanged;
		}
		
		void OnNextBlockToSpawnChanged(FallingBlock nextBlock)
		{
			const float transitionTime = .25f;
			var t = nextBlockImage.transform;

			t.DOScale(0f, transitionTime).OnComplete(() =>
			{
				nextBlockImage.sprite = nextBlock.BlockSprite;
				t.DOScale(1f, transitionTime);
			});
		}
		
		void OnHealthChanged(float newHealthValue, float maxValue)
		{
			healthBarFill.fillAmount = newHealthValue / maxValue;
		}
	}
}
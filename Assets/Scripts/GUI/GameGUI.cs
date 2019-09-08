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

		[SerializeField] Button HowToPlay;
		
		[SerializeField] Button Back;
		public Canvas HowTo;

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
		
		void OnNextBlockToSpawnChanged(BlocksSpawner.FallingBlockWrapper nextBlock)
		{
			const float transitionTime = .25f;
			var t = nextBlockImage.transform;

			t.DOScale(0f, transitionTime).OnComplete(() =>
			{
				nextBlockImage.sprite = nextBlock.Block.BlockSprite;
				nextBlockImage.color = nextBlock.RandomTint;
				t.DOScale(1f, transitionTime);
			});
		}
		
		void OnHealthChanged(float newHealthValue, float maxValue)
		{
			healthBarFill.fillAmount = newHealthValue / maxValue;
		}

		public void HowToPlayActive()
		{
			HowTo.gameObject.SetActive(true);
		}

		public void HowToPlayInActive()
		{
			HowTo.gameObject.SetActive(false);
		}


	}
}
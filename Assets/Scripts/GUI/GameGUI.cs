using System.Globalization;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using WSGJ.Utils;
using Image = UnityEngine.UI.Image;

namespace WSGJ
{
	public class GameGUI : MonoSingleton<GameGUI>
	{
		[SerializeField, Header("UI SETTINGS")]
		Image nextBlockImage;
		[SerializeField]
		Image healthBarFill;
		[SerializeField]
		TextMeshProUGUI scoreLabel;

		float currentScore = 0f;

		protected override void Awake()
		{
			base.Awake();
			BlocksSpawner.BlockToSpawnChanged += OnNextBlockToSpawnChanged;
			TruckController.HealthChanged += OnHealthChanged;
			BaseEntity.Died += OnEntityDied;
			PiggyEntity.Died += OnPiggyDied;
		}

		void Start()
		{
			RefreshScoreLabel(currentScore);
		}
		
		void OnDestroy()
		{
			BlocksSpawner.BlockToSpawnChanged -= OnNextBlockToSpawnChanged;
			TruckController.HealthChanged -= OnHealthChanged;
			BaseEntity.Died -= OnEntityDied;
			PiggyEntity.Died -= OnPiggyDied;
		}

		void OnEntityDied(BaseEntity entity)
		{
			RefreshScoreLabel(entity.ScoreValue);
		}
		
		void OnPiggyDied(PiggyEntity piggyEntity)
		{
			RefreshScoreLabel(piggyEntity.ScoreValue);
		}

		void RefreshScoreLabel(float value)
		{
			currentScore += value;
			scoreLabel.text = $"{currentScore.ToString(CultureInfo.InvariantCulture)}";
			scoreLabel.rectTransform.DOScale(1.1f, 0.25f)
				.OnStart(() =>
				{
					scoreLabel.DOColor(Color.white, 0.25f);
				})
				.OnComplete(() =>
				{
					scoreLabel.rectTransform.DOScale(1f, 0.15f);
					scoreLabel.DOColor(Color.black, 0.25f); 
				});
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
	}
}
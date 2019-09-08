using System.Globalization;
using DG.Tweening;
using TMPro;
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
			scoreLabel.text = $"SCORE: {currentScore.ToString(CultureInfo.InvariantCulture)}";
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
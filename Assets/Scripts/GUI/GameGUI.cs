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

		protected override void Awake()
		{
			base.Awake();
			BlocksSpawner.BlockToSpawnChanged += OnNextBlockToSpawnChanged;
		}

		void OnDestroy()
		{
			BlocksSpawner.BlockToSpawnChanged -= OnNextBlockToSpawnChanged;
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
	}
}
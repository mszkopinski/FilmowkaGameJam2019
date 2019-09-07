namespace WSGJ
{
	public class PeasantEntity : BaseEntity
	{
		public override void OnEntityDied()
		{
			base.OnEntityDied();
			
			ArmatureComponent.animation.Play("die");
			var currAnim = ArmatureComponent.animation;

			if(currAnim != null)
			{
				var duration = currAnim.animationConfig.duration;
				Invoke(nameof(DestroyEntity), duration);
			}
		}

		void DestroyEntity()
		{
			Destroy(gameObject);			
		}
	}
}
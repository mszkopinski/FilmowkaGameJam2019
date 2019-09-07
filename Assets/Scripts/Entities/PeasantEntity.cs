namespace WSGJ
{
	public class PeasantEntity : BaseEntity
	{
		public override void OnEntityDied()
		{
			base.OnEntityDied();
			ArmatureComponent.animation.Play("die");
		}

		void DestroyEntity()
		{
			Destroy(gameObject);			
		}
	}
}
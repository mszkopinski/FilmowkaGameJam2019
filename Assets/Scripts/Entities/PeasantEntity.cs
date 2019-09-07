namespace WSGJ
{
	public class PeasantEntity : BaseEntity
	{
		public override void OnEntityDied()
		{
			base.OnEntityDied();
			
			if(IsDead)
				return;
			
			ArmatureComponent.animation.Play("die", 1);
			Invoke(nameof(DestroyEntity), 10f);
			IsDead = true;
		}

		void DestroyEntity()
		{
			Destroy(gameObject);			
		}
	}
}
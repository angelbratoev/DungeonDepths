namespace DungeonDepths.Entities
{
	/// <summary>
	/// Archer class
	/// </summary>
	public class Archer : Entity
	{
		private const int strenght = 2;
		private const int intelligence = 0;
		private const int agility = 4;
		private const int range = 2;

		public Archer(int bonusStr, int bonusInt, int bonusAgi, int positionX, int positionY)
			: base(strenght + bonusStr, intelligence + bonusInt, agility + bonusAgi, range, positionX, positionY)
		{
			EnitySymbol = '#';
		}
	}
}

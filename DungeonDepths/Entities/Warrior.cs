namespace DungeonDepths.Entities
{
	/// <summary>
	/// Warrior class
	/// </summary>
	public class Warrior : Entity
	{
		private const int strenght = 3;
		private const int intelligence = 0;
		private const int agility = 3;
		private const int range = 1;

		public Warrior(int bonusStr, int bonusInt, int bonusAgi, int positionX, int positionY)
			: base(strenght + bonusStr, intelligence + bonusInt, agility + bonusAgi, range, positionX, positionY)
		{
			EnitySymbol = '@';
		}
	}
}

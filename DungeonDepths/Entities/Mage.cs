namespace DungeonDepths.Entities
{
	public class Mage : Entity
	{
		private const int strenght = 2;
		private const int intelligence = 3;
		private const int agility = 1;
		private const int range = 3;

		public Mage(int bonusStr, int bonusInt, int bonusAgi, int positionX, int positionY)
			: base(strenght + bonusStr, intelligence + bonusInt, agility + bonusAgi, range, positionX, positionY)
		{
			EnitySymbol = '*';
		}
	}
}

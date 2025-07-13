namespace DungeonDepths.Entities
{
	public class Enemy : Entity
	{
		public Enemy(int strenght, int intelligence, int agility, int range, int positionX, int positionY) 
			: base(strenght, intelligence, agility, range, positionX, positionY)
		{
			EnitySymbol = '◙';
		}

		public void Move(Entity player)
		{
			if (player.Position[0] > Position[0])
			{
				Position[0]++;
			}
			else if (player.Position[0] < Position[0])
			{
				Position[0]--;
			}

			if (player.Position[1] > Position[1])
			{
				Position[1]++;
			}
			else if (player.Position[1] < Position[1])
			{
				Position[1]--;
			}
		}
	}
}

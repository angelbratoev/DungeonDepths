using DungeonDepths.Entities;

namespace DungeonDepths.Utils
{
	public static class Creator
	{
		public static Entity CreatePlayer(int characterPick, int bonusStr, int bonusInt, int bonusAgi)
		{
			if (characterPick == 1)
			{
				return new Warrior(bonusStr, bonusInt, bonusAgi, 1, 1);
			}
			else if (characterPick == 2)
			{
				return new Archer(bonusStr, bonusInt, bonusAgi, 1, 1);
			}
			else
			{
				return new Mage(bonusStr, bonusInt, bonusAgi, 1, 1);
			}
		}

		public static Enemy CreateEnemy(Entity player)
		{
			int[] enemyPosition = EnemySpawnPosition(player.Position[0], player.Position[1]);
			Random random = new();

			return new Enemy(random.Next(1, 4),
				random.Next(1, 4),
				random.Next(1, 4),
				1,
				enemyPosition[0],
				enemyPosition[1]);
		}

		private static int[] EnemySpawnPosition(int playerPositionX, int playerPositionY)
		{
			Random random = new();
			int positionX = random.Next(10);
			int positionY = random.Next(10);

			if (positionX == playerPositionX && positionY == playerPositionY)
			{
				return EnemySpawnPosition(playerPositionX, playerPositionY);
			}
			else
			{
				return new int[] { positionX, positionY };
			}
		}

		public static char[,] CreateField(Entity player, List<Entity> enemies)
		{
			char[,] field = new char[10, 10];

			for (int i = 0; i < 10; i++)
			{
				for (int j = 0; j < 10; j++)
				{
					if (i == player.Position[0] && j == player.Position[1])
					{
						field[i, j] = player.EnitySymbol;
					}
					else if (enemies.Any(e => e.Position[0] == i && e.Position[1] == j))
					{
						field[i, j] = enemies.First().EnitySymbol;
					}
					else
					{
						field[i, j] = '▒';
					}
				}
			}

			return field;
		}

		public static void PrintField(char[,] field)
		{
			for (int i = 0; i < 10; i++)
			{
				for (int j = 0; j < 10; j++)
				{
					Console.Write(field[i, j]);
				}
				Console.WriteLine();
			}
		}
	}
}

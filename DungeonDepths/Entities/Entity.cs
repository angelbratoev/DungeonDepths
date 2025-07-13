using DungeonDepths.Contracts;

namespace DungeonDepths.Entities
{
	/// <summary>
	/// Abstract class for all active game pieces
	/// </summary>
	abstract public class Entity : IEntity
	{
		protected Entity(int strenght, int intelligence, int agility, int range, int positionX, int positionY)
		{
			Strenght = strenght;
			Intelligence = intelligence;
			Agility = agility;
			Range = range;
			Position = [positionX, positionY];

			Setup();
		}
		public int Strenght { get; set; }
		public int Intelligence { get; set; }
		public int Agility { get; set; }
		public int Range { get; set; }

		public int Health { get; set; }
		public int Mana { get; set; }
		public int Damage { get; set; }

		public char EnitySymbol { get; set; }
		public int[] Position { get; set; }

		public void Setup()
		{
			this.Health = this.Strenght * 5;
			this.Mana = this.Intelligence * 3;
			this.Damage = this.Agility * 2;
		}

		public void Attack(Entity target)
		{
			target.Health -= this.Damage;
		}

		public void Move(char direction)
		{
			int[] newPosition = new int[2];

			if (direction == 'w')
			{
				Position[0] -= 1;
			}
			else if (direction == 's')
			{
				Position[0] += 1;
			}
			else if (direction == 'a')
			{
				Position[1] -= 1;
			}
			else if (direction == 'd')
			{
				Position[1] += 1;
			}
			else if (direction == 'q')
			{
				Position[0] -= 1;
				Position[1] -= 1;
			}
			else if (direction == 'e')
			{
				Position[0] -= 1;
				Position[1] += 1;
			}
			else if (direction == 'x')
			{
				Position[0] += 1;
				Position[1] += 1;
			}
			else if (direction == 'z')
			{
				Position[0] += 1;
				Position[1] -= 1;
			}
		}

		public List<Entity> GetValidTargets(List<Entity> targets)
		{
			List<Entity> validTargets = new();

			foreach (Entity target in targets)
			{
				int differenceX = Math.Abs(Position[0] - target.Position[0]);
				int differenceY = Math.Abs(Position[1] - target.Position[1]);

				if (Math.Max(differenceX, differenceY) <= Range)
				{
					validTargets.Add(target);
				}
			}

			return validTargets;
		}
	}
}

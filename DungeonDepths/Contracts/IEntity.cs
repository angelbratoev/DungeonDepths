using DungeonDepths.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonDepths.Contracts
{
	public interface IEntity
	{
		public int Strenght { get; set; }
		public int Intelligence { get; set; }
		public int Agility { get; set; }
		public int Range { get; set; }
		public int Health { get; set; }
		public int Mana { get; set; }
		public int Damage { get; set; }
		public char EnitySymbol { get; set; }
		public int[] Position { get; set; }

		protected void Setup();
		public void Attack(Entity target);
		public void Move(char direction);
		public List<Entity> GetValidTargets(List<Entity> targets);
	}
}

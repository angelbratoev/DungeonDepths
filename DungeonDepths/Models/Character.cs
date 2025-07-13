using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonDepths.Models
{
	public class Character
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string ClassName { get; set; } = null!;

		[Required]
		public int Strenght { get; set; }

		[Required]
		public int Intelligence { get; set; }

		[Required]
		public int Agillity { get; set; }

		[Required]
		public int Range { get; set; }

		[Required]
		public int Health { get; set; }

		[Required]
		public int Mana { get; set; }

		[Required]
		public int Damage { get; set; }

		[Required]
		public DateTime TimeOfCreating { get; set; }
	}
}

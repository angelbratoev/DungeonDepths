using DungeonDepths.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonDepths.Models
{
	public class Session
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public int PlayerId { get; set; }

		[Required]
		[ForeignKey(nameof(PlayerId))]
		public Character Player { get; set; }

		[Required]
		public int MonstersKilled { get; set; }
	}
}

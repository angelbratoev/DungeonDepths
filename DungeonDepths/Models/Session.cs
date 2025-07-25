﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DungeonDepths.Models
{
	/// <summary>
	/// Entuty framework model for game session
	/// </summary>
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

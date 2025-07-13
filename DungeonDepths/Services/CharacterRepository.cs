using DungeonDepths.Contracts;
using DungeonDepths.Data;
using DungeonDepths.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonDepths.Services
{
	public class CharacterRepository : ICharacterRepository
	{
		private readonly DungeonDepthsContext context;

		public CharacterRepository(DungeonDepthsContext _context)
		{
			context = _context;
		}

		public async Task SaveAsync(Character character)
		{
			await context.Characters.AddAsync(character);
			await context.SaveChangesAsync();
		}

		public async Task<int> GetIdByDateAsync(DateTime characterCreationDate)
		{
			return await context.Characters
				.Where(c => c.TimeOfCreating == characterCreationDate)
				.Select(c => c.Id)
				.FirstOrDefaultAsync();
		}
	}
}

using DungeonDepths.Contracts;
using DungeonDepths.Data;
using DungeonDepths.Models;
using Microsoft.EntityFrameworkCore;

namespace DungeonDepths.Services
{
	/// <summary>
	/// Repository that saves the Character information to the database
	/// </summary>
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

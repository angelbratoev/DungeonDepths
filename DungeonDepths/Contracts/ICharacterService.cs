using DungeonDepths.Entities;
using DungeonDepths.Models;

namespace DungeonDepths.Contracts
{
	public interface ICharacterService
	{
		public Task<Character> CreateCharacterAsync(Entity entity, int characterPick);
		public Task<int> GetCharacterIdByDateAsync(DateTime date);
	}
}

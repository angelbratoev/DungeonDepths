using DungeonDepths.Entities;
using DungeonDepths.Models;

namespace DungeonDepths.Contracts
{
	/// <summary>
	/// Interface for Character Service
	/// </summary>
	public interface ICharacterService
	{
		public Task<Character> CreateCharacterAsync(Entity entity, int characterPick);
		public Task<int> GetCharacterIdByDateAsync(DateTime date);
	}
}

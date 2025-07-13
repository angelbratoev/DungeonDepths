using DungeonDepths.Models;

namespace DungeonDepths.Contracts
{
	public interface ICharacterRepository
	{
		Task SaveAsync(Character character);

		Task<int> GetIdByDateAsync(DateTime characterCreationDate);
	}
}

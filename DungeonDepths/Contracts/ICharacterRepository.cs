using DungeonDepths.Models;

namespace DungeonDepths.Contracts
{
	/// <summary>
	/// Interface for Character Repository
	/// </summary>
	public interface ICharacterRepository
	{
		Task SaveAsync(Character character);

		Task<int> GetIdByDateAsync(DateTime characterCreationDate);
	}
}

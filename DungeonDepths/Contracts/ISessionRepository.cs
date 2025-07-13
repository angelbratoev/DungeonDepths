using DungeonDepths.Models;

namespace DungeonDepths.Contracts
{
	/// <summary>
	/// Interface for Session Repository
	/// </summary>
	public interface ISessionRepository
	{
		Task SaveAsync(Session session);
	}
}

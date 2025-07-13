using DungeonDepths.Models;

namespace DungeonDepths.Contracts
{
	public interface ISessionRepository
	{
		Task SaveAsync(Session session);
	}
}

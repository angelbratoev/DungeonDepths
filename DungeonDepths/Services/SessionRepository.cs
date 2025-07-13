using DungeonDepths.Contracts;
using DungeonDepths.Data;
using DungeonDepths.Models;

namespace DungeonDepths.Services
{
	public class SessionRepository : ISessionRepository
	{
		private readonly DungeonDepthsContext context;

		public SessionRepository(DungeonDepthsContext _context)
		{
			context = _context;
		}

		public async Task SaveAsync(Session session)
		{
			await context.Sessions.AddAsync(session);
			await context.SaveChangesAsync();
		}
	}
}

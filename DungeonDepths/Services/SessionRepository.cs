using DungeonDepths.Contracts;
using DungeonDepths.Data;
using DungeonDepths.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

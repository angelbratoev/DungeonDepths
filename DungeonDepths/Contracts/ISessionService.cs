using DungeonDepths.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonDepths.Contracts
{
	public interface ISessionService
	{
		public Task CreateSessionAsync(int characterId, int monstersKilled);
	}
}

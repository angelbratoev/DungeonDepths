using DungeonDepths.Entities;
using DungeonDepths.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonDepths.Contracts
{
	public interface ICharacterService
	{
		public Task<Character> CreateCharacterAsync(Entity entity, int characterPick);
		public Task<int> GetCharacterIdByDateAsync(DateTime date);
	}
}

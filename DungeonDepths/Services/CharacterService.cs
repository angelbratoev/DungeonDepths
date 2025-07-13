using DungeonDepths.Contracts;
using DungeonDepths.Entities;
using DungeonDepths.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonDepths.Services
{
	public class CharacterService : ICharacterService
	{
		private readonly ICharacterRepository characterRepository;

		public CharacterService(ICharacterRepository _characterRepository)
		{
			characterRepository = _characterRepository;
		}

		public async Task<Character> CreateCharacterAsync(Entity entity, int characterPick)
		{
			string className = string.Empty;

			if (characterPick == 1)
			{
				className = "Warrior";
			}
			else if (characterPick == 2)
			{
				className = "Archer";
			}
			else
			{
				className = "Mage";
			}

			Character character = new()
			{
				ClassName = className,
				Strenght = entity.Strenght,
				Intelligence = entity.Intelligence,
				Agillity = entity.Agility,
				Health = entity.Health,
				Mana = entity.Mana,
				Damage = entity.Damage,
				Range = entity.Range,
				TimeOfCreating = DateTime.Now
			};

			await characterRepository.SaveAsync(character);

			return character;
		}

		public async Task<int> GetCharacterIdByDateAsync(DateTime date)
		{
			return await characterRepository.GetIdByDateAsync(date);
		}
	}
}

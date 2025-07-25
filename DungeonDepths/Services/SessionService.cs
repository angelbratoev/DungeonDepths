﻿using DungeonDepths.Contracts;
using DungeonDepths.Models;

namespace DungeonDepths.Services
{
	/// <summary>
	/// Service layer that connects the business logic with the database lyaer for Session
	/// </summary>
	public class SessionService : ISessionService
	{
		private readonly ISessionRepository sessionRepository;

		public SessionService(ISessionRepository _sessionRepository)
		{
			sessionRepository = _sessionRepository;
		}

		public async Task CreateSessionAsync(int characterId, int monstersKilled)
		{
			Session session = new()
			{
				PlayerId = characterId,
				MonstersKilled = monstersKilled
			};

			await sessionRepository.SaveAsync(session);
		}
	}
}

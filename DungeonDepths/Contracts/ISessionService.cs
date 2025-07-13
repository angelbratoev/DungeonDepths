namespace DungeonDepths.Contracts
{
	public interface ISessionService
	{
		public Task CreateSessionAsync(int characterId, int monstersKilled);
	}
}

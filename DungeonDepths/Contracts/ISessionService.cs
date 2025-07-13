namespace DungeonDepths.Contracts
{
	/// <summary>
	/// Interface for Session Service
	/// </summary>
	public interface ISessionService
	{
		public Task CreateSessionAsync(int characterId, int monstersKilled);
	}
}

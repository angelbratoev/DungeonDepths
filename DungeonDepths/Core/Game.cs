using DungeonDepths.Contracts;
using DungeonDepths.Entities;
using DungeonDepths.Enums;
using DungeonDepths.Models;
using DungeonDepths.Utils;

namespace DungeonDepths.Core
{
	/// <summary>
	/// Core business logic layer
	/// </summary>
	public class Game
	{
		private readonly ICharacterService characterService;
		private readonly ISessionService sessionService;
		private Screen screen;
		private List<Entity> enemies;

		public Game(ICharacterService _characterService, ISessionService _sessionService)
		{
			characterService = _characterService;
			sessionService = _sessionService;
			screen = Screen.MainMenu;
			enemies = new List<Entity>();
		}

		public async Task PlayAsync()
		{
			//Main menu screen
			while (screen == Screen.MainMenu)
			{
				Console.WriteLine("Welcome!");
				Console.WriteLine("Press any key to play.");
				Console.ReadLine();

				Console.Clear();
				screen = Screen.CharacterSelect;
			}

			//Character select screen

			int bonusStrPoints = 0,
				bonusIntPoints = 0,
				bonusAgiPoints = 0,
				characterPick = 0;

			while (screen == Screen.CharacterSelect)
			{
				Console.WriteLine("Choose character type:");
				Console.WriteLine("Options:");
				Console.WriteLine("1) Warrior");
				Console.WriteLine("2) Archer");
				Console.WriteLine("3) Mage");
				Console.Write("Your pick: ");

				characterPick = Validator.ValidateCharacter(Console.ReadLine());

				Console.WriteLine("Would you like to buff up your stats before starting?         (Limit: 3 points total)");
				Console.Write("Response (Y/N): ");

				char response = Validator.ValidateResponse(Console.ReadLine());

				int remainingStatPoints = 3;

				//Bonus stat allocation
				if (response == 'y')
				{
					Console.WriteLine($"Remaining Points: {remainingStatPoints}");
					Console.Write("Add to Strenght: ");

					int statToAdd = Validator.ValidateStatPoints(Console.ReadLine(), remainingStatPoints);
					bonusStrPoints += statToAdd;
					remainingStatPoints -= statToAdd;

					if (remainingStatPoints == 0)
					{
						screen = Screen.InGame;
						break;
					}

					Console.WriteLine($"Remaining Points: {remainingStatPoints}");
					Console.Write("Add to Intelligence: ");

					statToAdd = Validator.ValidateStatPoints(Console.ReadLine(), remainingStatPoints);
					bonusIntPoints += statToAdd;
					remainingStatPoints -= statToAdd;

					if (remainingStatPoints == 0)
					{
						screen = Screen.InGame;
						break;
					}

					Console.WriteLine($"Remaining Points: {remainingStatPoints}");
					Console.Write("Add to Agility: ");

					statToAdd = Validator.ValidateStatPoints(Console.ReadLine(), remainingStatPoints);
					bonusAgiPoints += statToAdd;
				}

				screen = Screen.InGame;
			}

			Console.Clear();

			Entity player = Creator.CreatePlayer(characterPick, bonusStrPoints, bonusIntPoints, bonusAgiPoints);

			//Save character in DB
			Character character = await characterService.CreateCharacterAsync(player, characterPick);
			DateTime characterCreationDate = character.TimeOfCreating;


			//In game screen

			int playerKillCount = 0;

			while (screen == Screen.InGame)
			{
				Console.Clear();
				enemies.Add(Creator.CreateEnemy(player));
				Console.WriteLine($"Health: {player.Health}   Mana: {player.Mana}");
				Console.WriteLine();

				char[,] field = Creator.CreateField(player, enemies);
				Creator.PrintField(field);

				Console.WriteLine("Choose action");
				Console.WriteLine("1) Attack");
				Console.WriteLine("2) Move");

				int action = Validator.ValidateAction(Console.ReadLine());

				if (action == 1) //Player combat
				{
					List<Entity> targets = player.GetValidTargets(enemies);

					if (targets.Count > 0)
					{
						for (int i = 0; i < targets.Count; i++)
						{
							Console.WriteLine($"{i}) target with remaining blood {targets[i].Health}");
						}

						Console.Write("Which one to attack: ");
						int targetIndex = Validator.ValidateTargetIndex(targets.Count, Console.ReadLine());
						Entity enemyTarget = targets[targetIndex];
						player.Attack(enemyTarget);

						if (enemyTarget.Health <= 0)
						{
							playerKillCount++;
							enemies.Remove(enemyTarget);
						}
					}
					else //Make the player move in case of no valid targets
					{
						Console.WriteLine("No available targets in your range! Please select which direction you want to move.");
						char direction = Validator.ValidateDirectionInput(player, Console.ReadLine(), field);
						player.Move(direction);
					}
				}
				else if (action == 2) //Player ovement
				{
					char direction = Validator.ValidateDirectionInput(player, Console.ReadLine(), field);
					player.Move(direction);
				}

				//Enemy turn
				foreach (Enemy enemy in enemies)
				{
					List<Entity> targets = new();
					targets.Add(player);

					if (enemy.GetValidTargets(targets).Count > 0)
					{
						enemy.Attack(player);

						if (player.Health <= 0)
						{
							screen = Screen.Exit;
						}
					}
					else
					{
						enemy.Move(player);
					}
				}
			}

			//Exit screen
			if (screen == Screen.Exit)
			{
				int characterId = await characterService.GetCharacterIdByDateAsync(characterCreationDate);
				await sessionService.CreateSessionAsync(characterId, playerKillCount);

				Console.Clear();
				Console.WriteLine("Game over!");
				Console.WriteLine($"Your final score is {playerKillCount} defeated monsters!");
			}
		}		
	}
}

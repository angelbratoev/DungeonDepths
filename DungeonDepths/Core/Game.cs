using DungeonDepths.Contracts;
using DungeonDepths.Data;
using DungeonDepths.Entities;
using DungeonDepths.Enums;
using DungeonDepths.Models;
using DungeonDepths.Services;
using DungeonDepths.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DungeonDepths.Core
{
	public class Game
	{
		private readonly ICharacterRepository characterRepository;
		private readonly ISessionRepository sessionRepository;
		private Screen screen;
		private int bonusStrPoints = 0;
		private int bonusIntPoints = 0;
		private int bonusAgiPoints = 0;
		private int characterPick;
		private List<Entity> enemies;
		private int playerKillCount = 0;
		private DateTime characterCreationDate;

		public Game(ICharacterRepository _characterRepository, ISessionRepository _sessionRepository)
		{
			characterRepository = _characterRepository;
			sessionRepository = _sessionRepository;
			screen = Screen.MainMenu;
			enemies = new List<Entity>();
		}

		public async Task Play()
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
			while (screen == Screen.CharacterSelect)
			{
				Console.WriteLine("Choose character type:");
				Console.WriteLine("Options:");
				Console.WriteLine("1) Warrior");
				Console.WriteLine("2) Archer");
				Console.WriteLine("3) Mage");
				Console.Write("Your pick: ");

				characterPick = Validator.ValidateCharacter(Console.ReadLine());

				Console.WriteLine("Would you like to buff up your stats before starting?");
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

			Entity player = CreatePlayer(characterPick, bonusStrPoints, bonusIntPoints, bonusAgiPoints);

			//Save character in DB
			Character character = CreateCharacterModel(player);
			characterCreationDate = character.TimeOfCreating;
			await characterRepository.SaveAsync(character);
			Console.Clear();

			//In game screen
			while (screen == Screen.InGame)
			{
				enemies.Add(CreateEnemy(player));
				Console.WriteLine($"Health: {player.Health}   Mana: {player.Mana}");
				Console.WriteLine();

				char[,] field = CreateField(player, enemies);
				PrintField(field);

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
				int characterId = await characterRepository.GetIdByDateAsync(characterCreationDate);
				Session session = CreateSessionModel(characterId);
				await sessionRepository.SaveAsync(session);

				Console.Clear();
				Console.WriteLine("Game over!");
				Console.WriteLine($"Your final score is {playerKillCount} defeated monsters!");
			}
		}

		//Private methods
		private Entity CreatePlayer(int characterPick, int bonusStr, int bonusInt, int bonusAgi)
		{
			if (characterPick == 1)
			{
				return new Warrior(bonusStr, bonusInt, bonusAgi, 1, 1);
			}
			else if (characterPick == 2)
			{
				return new Archer(bonusStr, bonusInt, bonusAgi, 1, 1);
			}
			else
			{
				return new Mage(bonusStr, bonusInt, bonusAgi, 1, 1);
			}
		}

		private Enemy CreateEnemy(Entity player)
		{
			int[] enemyPosition = EnemySpawnPosition(player.Position[0], player.Position[1]);
			Random random = new();

			return new Enemy(random.Next(1, 4),
				random.Next(1, 4),
				random.Next(1, 4),
				1,
				enemyPosition[0],
				enemyPosition[1]);
		}

		private int[] EnemySpawnPosition(int playerPositionX, int playerPositionY)
		{
			Random random = new();
			int positionX = random.Next(10);
			int positionY = random.Next(10);

			if (positionX == playerPositionX && positionY == playerPositionY)
			{
				return EnemySpawnPosition(playerPositionX, playerPositionY);
			}
			else
			{
				return new int[] { positionX, positionY };
			}
		}

		private Character CreateCharacterModel(Entity entity)
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

			return new Character()
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
		}

		private Session CreateSessionModel(int characterId)
		{
			return new Session()
			{
				PlayerId = characterId,
				MonstersKilled = playerKillCount
			};
		}

		private char[,] CreateField(Entity player, List<Entity> enemies)
		{
			char[,] field = new char[10, 10];

			for (int i = 0; i < 10; i++)
			{
				for (int j = 0; j < 10; j++)
				{
					if (i == player.Position[0] && j == player.Position[1])
					{
						field[i, j] = player.EnitySymbol;
					}
					else if (enemies.Any(e => e.Position[0] == i && e.Position[1] == j))
					{
						field[i, j] = enemies.First().EnitySymbol;
					}
					else
					{
						field[i, j] = '▒';
					}
				}
			}

			return field;
		}

		private void PrintField(char[,] field)
		{
			for (int i = 0; i < 10; i++)
			{
				for (int j = 0; j < 10; j++)
				{
					Console.Write(field[i, j]);
				}
				Console.WriteLine();
			}
		}
	}
}

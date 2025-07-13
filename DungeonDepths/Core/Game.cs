using DungeonDepths.Contracts;
using DungeonDepths.Data;
using DungeonDepths.Entities;
using DungeonDepths.Enums;
using DungeonDepths.Models;
using DungeonDepths.Services;
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
			while (screen == Screen.MainMenu)
			{
				Console.WriteLine("Welcome!");
				Console.WriteLine("Press any key to play.");
				Console.ReadLine();

				Console.Clear();
				screen = Screen.CharacterSelect;
			}

			while (screen == Screen.CharacterSelect)
			{
				Console.WriteLine("Choose character type:");
				Console.WriteLine("Options:");
				Console.WriteLine("1) Warrior");
				Console.WriteLine("2) Archer");
				Console.WriteLine("3) Mage");
				Console.Write("Your pick: ");

				characterPick = ValidateCharacter(Console.ReadLine());

				Console.WriteLine("Would you like to buff up your stats before starting?");
				Console.Write("Response (Y/N): ");

				char response = ValidateResponse(Console.ReadLine());

				int remainingStatPoints = 3;

				if (response == 'Y' || response == 'y')
				{
					Console.WriteLine($"Remaining Points: {remainingStatPoints}");
					Console.Write("Add to Strenght: ");

					int statToAdd = ValidateStatPoints(Console.ReadLine(), remainingStatPoints);
					bonusStrPoints += statToAdd;
					remainingStatPoints -= statToAdd;

					if (remainingStatPoints == 0)
					{
						screen = Screen.InGame;
						break;
					}

					Console.WriteLine($"Remaining Points: {remainingStatPoints}");
					Console.Write("Add to Intelligence: ");

					statToAdd = ValidateStatPoints(Console.ReadLine(), remainingStatPoints);
					bonusIntPoints += statToAdd;
					remainingStatPoints -= statToAdd;

					if (remainingStatPoints == 0)
					{
						screen = Screen.InGame;
						break;
					}

					Console.WriteLine($"Remaining Points: {remainingStatPoints}");
					Console.Write("Add to Agility: ");

					statToAdd = ValidateStatPoints(Console.ReadLine(), remainingStatPoints);
					bonusAgiPoints += statToAdd;

					screen = Screen.InGame;
				}
				else
				{
					screen = Screen.InGame;
				}
			}

			Console.Clear();

			Entity player = CreatePlayer(characterPick, bonusStrPoints, bonusIntPoints, bonusAgiPoints);

			Character character = CreateCharacterModel(player);
			characterCreationDate = character.TimeOfCreating;
			await characterRepository.SaveAsync(character);
			Console.Clear();

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

				int action = ValidateAction(Console.ReadLine());

				if (action == 1)
				{
					List<Entity> targets = player.ValidTargets(enemies);

					if (targets.Count > 0)
					{
						for (int i = 0; i < targets.Count; i++)
						{
							Console.WriteLine($"{i}) target with remaining blood {targets[i].Health}");
						}

						Console.Write("Which one to attack: ");
						int targetIndex = ValidateTarget(targets.Count, Console.ReadLine());
						Entity enemyTarget = targets[targetIndex];
						player.Attack(enemyTarget);

						if (enemyTarget.Health <= 0)
						{
							playerKillCount++;
							enemies.Remove(enemyTarget);
						}
					}
					else
					{
						Console.WriteLine("No available targets in your range! Please select which direction you want to move.");
						char direction = ValidateDirectionInput(player, Console.ReadLine(), field);
						player.Move(direction);
					}
				}
				else if (action == 2)
				{
					char direction = ValidateDirectionInput(player, Console.ReadLine(), field);
					player.Move(direction);
				}

				foreach (Enemy enemy in enemies)
				{
					List<Entity> targets = new();
					targets.Add(player);

					if (enemy.ValidTargets(targets).Count > 0)
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

		private int ValidateCharacter(string character)
		{
			if (int.TryParse(character, out int characterPick) && characterPick >= 1 && characterPick <= 3)
			{
				return characterPick;
			}
			else
			{
				Console.WriteLine("Invalid command! Please enter 1 for Warrior, 2 for Archer or 3 for Mage.");
				string input = Console.ReadLine();

				return ValidateCharacter(input);
			}
		}

		private char ValidateResponse(string input)
		{
			if (char.TryParse(input, out char response) && (response == 'Y' || response == 'y' || response == 'N' || response == 'n'))
			{
				return response;
			}
			else
			{
				Console.WriteLine("Invalid command! Please answer with Y or N");
				return ValidateResponse(Console.ReadLine());
			}
		}

		private int ValidateStatPoints(string input, int remainingStatPoints)
		{
			if (int.TryParse(input, out int statsToAdd) && statsToAdd >= 0 && statsToAdd <= remainingStatPoints)
			{
				return statsToAdd;
			}
			else
			{
				Console.WriteLine($"Invalid input! Please enter a number between 0 and {remainingStatPoints}");
				return ValidateStatPoints(Console.ReadLine(), remainingStatPoints);
			}
		}

		private int ValidateAction(string input)
		{
			if (int.TryParse(input, out int action) && action >= 1 && action <= 2)
			{
				return action;
			}
			else
			{
				Console.WriteLine("Invalid input! Please select 1 to Attack or 2 to Move");
				return ValidateAction(Console.ReadLine());
			}
		}

		private char ValidateDirectionInput(Entity player, string input, char[,] field)
		{
			if (char.TryParse(input.ToLower(), out char direction) &&
				(direction == 'q' || direction == 'w' || direction == 'e'
				|| direction == 'a' || direction == 's' || direction == 'd'
				|| direction == 'z' || direction == 'x'))
			{
				if ((direction == 'w' && player.Position[0] - 1 < 0)
					|| (direction == 's' && player.Position[0] + 1 > 9)
					|| (direction == 'a' && player.Position[1] - 1 < 0)
					|| (direction == 'd' && player.Position[1] + 1 > 9)
					|| (direction == 'q' && (player.Position[0] - 1 < 0 || player.Position[1] - 1 < 0))
					|| (direction == 'e' && (player.Position[0] - 1 < 0 || player.Position[1] + 1 > 9))
					|| (direction == 'z' && (player.Position[0] + 1 > 9 || player.Position[1] - 1 < 0))
					|| (direction == 'x' && (player.Position[0] + 1 > 9 || player.Position[1] + 1 > 9)))
				{
					Console.WriteLine("Out of bounds! Please select a different direction");
					return ValidateDirectionInput(player, Console.ReadLine(), field);
				}
				else if (direction == 'w' && field[player.Position[0] -1, player.Position[1]] != '▒'
						|| direction == 's' && field[player.Position[0] + 1, player.Position[1]] != '▒'
						|| direction == 'a' && field[player.Position[0], player.Position[1] - 1] != '▒'
						|| direction == 'd' && field[player.Position[0], player.Position[1] + 1] != '▒'
						|| direction == 'q' && field[player.Position[0] - 1, player.Position[1] - 1] != '▒'
						|| direction == 'e' && field[player.Position[0] - 1, player.Position[1] + 1] != '▒'
						|| direction == 'z' && field[player.Position[0] + 1, player.Position[1] - 1] != '▒'
						|| direction == 'x' && field[player.Position[0] + 1, player.Position[1] + 1] != '▒')
				{
					Console.WriteLine("Tile already ocupied! Please select a different direction.");
					return ValidateDirectionInput(player, Console.ReadLine(), field);
				}
				else
				{
					return direction;
				}
			}
			else
			{
				Console.WriteLine("Invalid input! Please enter a valid direction:");
				Console.WriteLine("W - Move up");
				Console.WriteLine("S - Move down");
				Console.WriteLine("D - Move right");
				Console.WriteLine("А - Move left");
				Console.WriteLine("Е - Move diagonally up & right");
				Console.WriteLine("X - Move diagonally down & right");
				Console.WriteLine("Q - Move diagonally up & left");
				Console.WriteLine("Z - Move diagonally down & left");
				return ValidateDirectionInput(player, Console.ReadLine(), field);
			}

		}

		private int ValidateTarget(int targetsAmount, string input)
		{
			if (int.TryParse(input, out int targetIndex) && targetIndex >= 0 && targetIndex <= targetsAmount - 1)
			{
				return targetIndex;
			}
			else
			{
				Console.WriteLine("Invalid input! Please enter a valid target index.");
				return ValidateTarget(targetsAmount, Console.ReadLine());
			}
		}

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

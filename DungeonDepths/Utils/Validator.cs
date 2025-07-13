using DungeonDepths.Entities;

namespace DungeonDepths.Utils
{
	/// <summary>
	/// Static class that does the user input validations
	/// </summary>
	public static class Validator
	{
		public static int ValidateCharacter(string character)
		{
			if (int.TryParse(character, out int characterPick) && characterPick >= 1 && characterPick <= 3)
			{
				return characterPick;
			}
			else
			{
				Console.WriteLine("Invalid command! Please enter 1 for Warrior, 2 for Archer or 3 for Mage.");

				return ValidateCharacter(Console.ReadLine());
			}
		}

		public static char ValidateResponse(string input)
		{
			if (char.TryParse(input.ToLower(), out char response) && (response == 'y' || response == 'n'))
			{
				return response;
			}
			else
			{
				Console.WriteLine("Invalid command! Please answer with Y or N");
				return ValidateResponse(Console.ReadLine());
			}
		}

		public static int ValidateStatPoints(string input, int remainingStatPoints)
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

		public static int ValidateAction(string input)
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

		public static char ValidateDirectionInput(Entity player, string input, char[,] field)
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
				else if (direction == 'w' && field[player.Position[0] - 1, player.Position[1]] != '▒'
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

		public static int ValidateTargetIndex(int targetsAmount, string input)
		{
			if (int.TryParse(input, out int targetIndex) && targetIndex >= 0 && targetIndex <= targetsAmount - 1)
			{
				return targetIndex;
			}
			else
			{
				Console.WriteLine("Invalid input! Please enter a valid target index.");
				return ValidateTargetIndex(targetsAmount, Console.ReadLine());
			}
		}
	}
}

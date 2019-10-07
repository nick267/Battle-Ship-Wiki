using System;
using SwinGameSDK;

/// <summary>
/// The EndingGameController is responsible for managing the interactions at the end
/// of a game.
/// </summary>

internal static class EndingGameController
{

	//Variables and Getters to Count Games Played/Won/Lost
	private static int _gamesWon = 0;
	private static int _gamesLost = 0;
	private static int _gamesPlayed = 0;

	public static int gamesWon { get { return _gamesWon; } }
	public static int gamesLost { get { return _gamesLost; } }
	public static int gamesPlayed { get { return _gamesPlayed; } }

	//Check if player surrendered the game
	private static bool _playerSurrendered = false;

	public static void PlayerSurrendered() {
		_playerSurrendered = true;
	}

	public static void PlayerSurrenderReset() {
		_playerSurrendered = false;
	}

	/// <summary>
	/// Draw the end of the game screen, shows the win/lose state
	/// </summary>
	public static void DrawEndOfGame()
	{

		Rectangle toDraw = new Rectangle();
		string whatShouldIPrint = null;

		UtilityFunctions.DrawField(GameController.ComputerPlayer.PlayerGrid, GameController.ComputerPlayer, true);
		UtilityFunctions.DrawSmallField(GameController.HumanPlayer.PlayerGrid, GameController.HumanPlayer);

		toDraw.X = 0;
		toDraw.Y = 250;
		toDraw.Width = SwinGame.ScreenWidth();
		toDraw.Height = SwinGame.ScreenHeight();

		if (GameController.HumanPlayer.IsDestroyed)
		{
			whatShouldIPrint = "YOU LOSE!";
		}
		else if (_playerSurrendered)
		{
			whatShouldIPrint = "SURRENDERED!";
		}
		else
		{
			whatShouldIPrint = "-- WINNER --";
		}

		SwinGame.DrawTextLines(whatShouldIPrint, Color.White, Color.Transparent, GameResources.GameFont("ArialLarge"), FontAlignment.AlignCenter, toDraw);
	}

	/// <summary>
	/// Handle the input during the end of the game. Any interaction
	/// will result in it reading in the highsSwinGame.
	/// </summary>
	public static void HandleEndOfGameInput()
	{
		if (SwinGame.MouseClicked(MouseButton.LeftButton) || SwinGame.KeyTyped(KeyCode.vk_RETURN) || SwinGame.KeyTyped(KeyCode.vk_ESCAPE))
		{
			_gamesPlayed++;
			
			if ((GameController.HumanPlayer.IsDestroyed) || (_playerSurrendered))
			{
				_gamesLost++;
			}
			else 
			{
				_gamesWon++;
			}
			//DEBUG
			//Console.WriteLine("Games Played: " + _gamesPlayed + " || Games Won: " + _gamesWon + " || Games Lost: " + _gamesLost);

			HighScoreController.ReadHighScore(GameController.HumanPlayer.Score);
			GameController.EndCurrentState();
		}
	}

}

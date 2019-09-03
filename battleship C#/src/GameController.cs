﻿using System;

//====================================================================================================
//The Free Edition of Instant C# limits conversion output to 100 lines per file.

//To subscribe to the Premium Edition, visit our website:
//https://www.tangiblesoftwaresolutions.com/order/order-instant-csharp.html
//====================================================================================================

using SwinGameSDK;

/// <summary>
/// The GameController is responsible for controlling the game,
/// managing user input, and displaying the current state of the
/// game.
/// </summary>
public static class GameController
{

	private static BattleShipsGame _theGame;
	private static Player _human;
	private static AIPlayer _ai;

	private static Stack<GameState> _state = new Stack<GameState>();

	private static AIOption _aiSetting;

	/// <summary>
	/// Returns the current state of the game, indicating which screen is
	/// currently being used
	/// </summary>
	/// <value>The current state</value>
	/// <returns>The current state</returns>
	public static GameState CurrentState
	{
		get
		{
			return _state.Peek();
		}
	}

	/// <summary>
	/// Returns the human player.
	/// </summary>
	/// <value>the human player</value>
	/// <returns>the human player</returns>
	public static Player HumanPlayer
	{
		get
		{
			return _human;
		}
	}

	/// <summary>
	/// Returns the computer player.
	/// </summary>
	/// <value>the computer player</value>
	/// <returns>the conputer player</returns>
	public static Player ComputerPlayer
	{
		get
		{
			return _ai;
		}
	}

	static GameController()
	{
		//bottom state will be quitting. If player exits main menu then the game is over
		_state.Push(GameState.Quitting);

		//at the start the player is viewing the main menu
		_state.Push(GameState.ViewingMainMenu);
	}

	/// <summary>
	/// Starts a new game.
	/// </summary>
	/// <remarks>
	/// Creates an AI player based upon the _aiSetting.
	/// </remarks>
	public static void StartGame()
	{
		if (_theGame != null)
		{
			EndGame();
		}

		//Create the game
		_theGame = new BattleShipsGame();

		//create the players
		switch (_aiSetting)
		{
			case AIOption.Medium:
				_ai = new AIMediumPlayer(_theGame);
				break;
			case AIOption.Hard:
				_ai = new AIHardPlayer(_theGame);
				break;
			default:
				_ai = new AIHardPlayer(_theGame);
				break;
		}

		_human = new Player(_theGame);

		//AddHandler _human.PlayerGrid.Changed, AddressOf GridChanged
		_ai.PlayerGrid.Changed += GridChanged;
		_theGame.AttackCompleted += AttackCompleted;

		AddNewState(GameState.Deploying);
	}

	/// <summary>
	/// Stops listening to the old game once a new game is started
	/// </summary>

	private static void EndGame()
	{
		//RemoveHandler _human.PlayerGrid.Changed, AddressOf GridChanged
		_ai.PlayerGrid.Changed -= GridChanged;
		_theGame.AttackCompleted -= AttackCompleted;
	}

	/// <summary>
	/// Listens to the game grids for any changes and redraws the screen
	/// when the grids change
	/// </summary>
	/// <param name="sender">the grid that changed</param>
	/// <param name="args">not used</param>
	private static void GridChanged(object sender, EventArgs args)
	{
		DrawScreen();
		SwinGame.RefreshScreen();
	}

	private static void PlayHitSequence(int row, int column, bool showAnimation)
	{
		if (showAnimation)
		{
			UtilityFunctions.AddExplosion(row, column);
		}

		Audio.PlaySoundEffect(GameResources.GameSound("Hit"));

		UtilityFunctions.DrawAnimationSequence();
	}

	private static void PlayMissSequence(int row, int column, bool showAnimation)
	{
		if (showAnimation)
		{
			UtilityFunctions.AddSplash(row, column);
		}

		Audio.PlaySoundEffect(GameResources.GameSound("Miss"));

		UtilityFunctions.DrawAnimationSequence();
	}

	/// <summary>
	/// Listens for attacks to be completed.
	/// </summary>
	/// <param name="sender">the game</param>
	/// <param name="result">the result of the attack</param>
	/// <remarks>
	/// Displays a message, plays sound and redraws the screen
	/// </remarks>
	private static void AttackCompleted(object sender, AttackResult result)
	{
		bool isHuman = _theGame.Player == HumanPlayer;

		if (isHuman)
		{
			UtilityFunctions.Message = "You " + result.ToString();
		}
		else
		{
			UtilityFunctions.Message = "The AI " + result.ToString();
		}

		switch (result.Value)
		{
			case ResultOfAttack.Destroyed:
				PlayHitSequence(result.Row, result.Column, isHuman);
				Audio.PlaySoundEffect(GameResources.GameSound("Sink"));

				break;
			case ResultOfAttack.GameOver:
				PlayHitSequence(result.Row, result.Column, isHuman);
				Audio.PlaySoundEffect(GameResources.GameSound("Sink"));

				while (Audio.SoundEffectPlaying(GameResources.GameSound("Sink")))
				{
					SwinGame.Delay(10);
					SwinGame.RefreshScreen();
				}

				if (HumanPlayer.IsDestroyed)
				{
					Audio.PlaySoundEffect(GameResources.GameSound("Lose"));
				}
				else
				{
					Audio.PlaySoundEffect(GameResources.GameSound("Winner"));
				}

				break;
			case ResultOfAttack.Hit:
				PlayHitSequence(result.Row, result.Column, isHuman);
				break;
			case ResultOfAttack.Miss:
				PlayMissSequence(result.Row, result.Column, isHuman);
				break;
			case ResultOfAttack.ShotAlready:
				Audio.PlaySoundEffect(GameResources.GameSound("Error"));
				break;
		}
	}

	/// <summary>
	/// Completes the deployment phase of the game and
	/// switches to the battle mode (Discovering state)
	/// </summary>
	/// <remarks>
	/// This adds the players to the game before switching
	/// state.
	/// </remarks>
	public static void EndDeployment()
	{
		//deploy the players
		_theGame.AddDeployedPlayer(_human);
		_theGame.AddDeployedPlayer(_ai);

		SwitchState(GameState.Discovering);
	}

	/// <summary>
	/// Gets the player to attack the indicated row and column.
	/// </summary>
	/// <param name="row">the row to attack</param>
	/// <param name="col">the column to attack</param>
	/// <remarks>
	/// Checks the attack result once the attack is complete
	/// </remarks>
	public static void Attack(int row, int col)
	{
		AttackResult result = _theGame.Shoot(row, col);
		CheckAttackResult(result);
	}

	/// <summary>
	/// Gets the AI to attack.
	/// </summary>
	/// <remarks>
	/// Checks the attack result once the attack is complete.
	/// </remarks>
	private static void AIAttack()
	{
		AttackResult result = _theGame.Player.Attack();
		CheckAttackResult(result);
	}

	/// <summary>
	/// Checks the results of the attack and switches to
	/// Ending the Game if the result was game over.
	/// </summary>
	/// <param name="result">the result of the last
	/// attack</param>
	/// <remarks>Gets the AI to attack if the result switched
	/// to the AI player.</remarks>
	private static void CheckAttackResult(AttackResult result)
	{
		switch (result.Value)
		{
			case ResultOfAttack.Miss:
				if (_theGame.Player == ComputerPlayer)
				{
					AIAttack();
				}

//====================================================================================================
//End of the allowed output for the Free Edition of Instant C#.

//To subscribe to the Premium Edition, visit our website:
//https://www.tangiblesoftwaresolutions.com/order/order-instant-csharp.html
//====================================================================================================
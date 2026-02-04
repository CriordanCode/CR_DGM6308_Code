using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

bool closeRequested = false;
State state = State.Main;
Stopwatch stopwatch = new();
TimeSpan framerate = TimeSpan.FromSeconds(1d / 60d);
bool direction = default;
int playerGoesFirst = default;
int x = 0;
int y = 0;
int x_max = 38;
int y_max = 14;
List<((int X, int Y)? Position, Player currentPlayer)> darts = new();
int computer_x = default;
int computer_y = default;
int computer_skip = default;

//new Variables
//Tracker for the current variable
int currentPlayer = 0;
//Number of humans playing the game
int humanPlayers = 0;
//List of player class to track player number, if human, and their score
List<Player> playerList = new();
//Simple int for gamemodes (allows for more gamemodes in the future if wanted)
int gameMode = 0;
//Max score for the second gamemode, can be adjusted for faster/shorter games
int maxScore = 50;

try
{
    Console.CursorVisible = false;
    Console.OutputEncoding = Encoding.UTF8;
    while (!closeRequested)
    {
        Render();
        Update();
    }
}
finally
{
    Console.CursorVisible = true;
    Console.Clear();
    Console.WriteLine("Darts was Closed.");
}

void Update()
{
    switch (state)
    {
        case State.Main:
            PressEnterToContinue();
            if (closeRequested)
            {
                return;
            }
            state = State.ModeSelect;
            Console.Clear();
            break;

        //New case for the game to select which mode the player wants to play each time
        case State.ModeSelect:
            gameMode = GameModeSelect();
            state = State.PlayerSelect;
            Console.Clear();
            break;

        //new case for the game to select how many human players are playing each time
        case State.PlayerSelect:
            humanPlayers = PlayerSelectToContinue();
            //Loop to add new players to a list for the amount selected
            for (int i = 0; i < humanPlayers; i++)
            {
                playerList.Add(new Player(i, true, 0));
            }
            //Loop to fill in the remaining amount with computers
            while(playerList.Count < 4)
            {
                playerList.Add(new Player(playerList.Count, false, 0));
            }
            //0 to 4 random who goes first
            playerGoesFirst = Random.Shared.Next(0, 4);
            //set the current player tracker
            currentPlayer = playerGoesFirst;
            state = State.ConfirmRandomTurnOrder;
            Console.Clear();
            break;

        case State.ConfirmRandomTurnOrder:
            PressEnterToContinue();
            if (closeRequested)
            {
                return;
            }
            //check the first player if they are human or not
            if (playerList[playerGoesFirst].IsHuman)
            {
                state = State.PlayerHorizontal;
            }
            else
            {
                state = State.ComputerHorizontal;
            }

            direction = true;
            x = 0;
            //if first player is not human setup turn for computer
            if (!playerList[playerGoesFirst].IsHuman)
            {
                computer_x = Random.Shared.Next(x_max + 1);
                computer_skip = Random.Shared.Next(1, 4);
            }
            stopwatch.Restart();
            Console.Clear();
            break;

        case State.ConfirmPlayerThrow:
            PressEnterToContinue();
            if (closeRequested)
            {
                return;
            }
            //if the gamemode is the first one end game on 20 darts otherwise keep going
            if (darts.Count >= 20 && gameMode == 0)
            {
                state = State.ConfirmGameEnd;
                Console.Clear();
                break;
            }
            //current player iterator, if it gets above 3 reset to 0 as that would be out of bounds
            if (++currentPlayer > 3)
            {
                currentPlayer = 0;
            }
            //if the next player is human set the next state for human play
            if (playerList[currentPlayer].IsHuman)
            {
                state = State.PlayerHorizontal;
            }
            else
            {
                state = State.ComputerHorizontal;
                computer_x = Random.Shared.Next(x_max + 1);
                computer_skip = Random.Shared.Next(1, 4);
            }
            direction = true;
            x = 0;

            stopwatch.Restart();
            Console.Clear();
            break;

        //Same changes that were made in the confirm player throw 
        //except this runs after a computer makes a throw
        case State.ConfirmComputerThrow:
            PressEnterToContinue();
            if (closeRequested)
            {
                return;
            }
            if (darts.Count >= 20 && gameMode == 0)
            {
                state = State.ConfirmGameEnd;
                Console.Clear();
                break;
            }
            if (++currentPlayer > 3)
            {
                currentPlayer = 0;
            }
            if (playerList[currentPlayer].IsHuman)
            {
                state = State.PlayerHorizontal;
            }
            else
            {
                state = State.ComputerHorizontal;
                computer_x = Random.Shared.Next(x_max + 1);
                computer_skip = Random.Shared.Next(1, 4);
            }
            direction = true;
            x = 0;
            stopwatch.Restart();
            Console.Clear();
            break;

        case State.PlayerHorizontal or State.ComputerHorizontal:
            if (KeyPressed() && state is State.PlayerHorizontal)
            {
                if (closeRequested)
                {
                    return;
                }
                state = State.PlayerVertical;
                direction = true;
                y = 0;
                stopwatch.Restart();
                Console.Clear();
                break;
            }
            if (closeRequested)
            {
                return;
            }
            if (direction)
            {
                x++;
            }
            else
            {
                x--;
            }
            if (state is State.ComputerHorizontal && x == computer_x)
            {
                computer_skip--;
                if (computer_skip < 0)
                {
                    state = State.ComputerVertical;
                    direction = true;
                    y = 0;
                    stopwatch.Restart();
                    computer_y = Random.Shared.Next(y_max + 1);
                    computer_skip = Random.Shared.Next(1, 4);
                }
            }
            if (x <= 0 || x >= x_max)
            {
                if (x < 0) x = 0;
                if (x > x_max) x = x_max;
                direction = !direction;
            }
            ControlFrameRate();
            break;

        case State.PlayerVertical or State.ComputerVertical:
            if (KeyPressed() && state is State.PlayerVertical)
            {
                if (closeRequested)
                {
                    return;
                }
                state = State.ConfirmPlayerThrow;
                (int X, int Y)? position = (x, y);
                for (int i = 0; i < darts.Count; i++)
                {
                    if (darts[i].Position == (x, y))
                    {
                        darts[i] = (null, darts[i].currentPlayer);
                        position = null;
                    }
                }
                darts.Add(new(position, playerList[currentPlayer]));
                //Runs the method to calculate the current score after a dart has been added
                CalculateScore(gameMode);
                //Removes darts if in gamemode 1 the score would set the user to below 0
                if(playerList[currentPlayer].Score < 0 && gameMode == 1)
                {
                    darts.RemoveAt(darts.Count-1);
                    //If darts are touched recalculate the score
                    CalculateScore(gameMode);
                }
                //If we are in gamemode 1 check if player has a 0 score to end the game
                if (gameMode == 1)
                {
                    foreach (Player player in playerList)
                    {
                        if(player.Score == 0)
                        {
                            state = State.ConfirmGameEnd;
                            Console.Clear();
                            break;
                        }
                    }
                }
                Console.Clear();
                break;
            }
            if (closeRequested)
            {
                return;
            }
            if (direction)
            {
                y++;
            }
            else
            {
                y--;
            }
            if (state is State.ComputerVertical && y == computer_y)
            {
                computer_skip--;
                if(computer_skip < 0)
                {
                    state = State.ConfirmComputerThrow;
                    (int X, int Y)? position = (x, y);
                    for (int i =  0; i < darts.Count; i++)
                    {
                        if (darts[i].Position == (x, y))
                        {
                            darts[i] = (null, darts[i].currentPlayer);
                            position = null;
                        }
                    }
                    darts.Add(new(position, playerList[currentPlayer]));
                    //Calculate score after computer throws a dart
                    CalculateScore(gameMode);
                    //If the thrown dart brings them below 0 remove it and recalculate score in gamemode 1
                    if(playerList[currentPlayer].Score < 0 && gameMode == 1)
                    {
                        darts.RemoveAt(darts.Count-1);
                        CalculateScore(gameMode);
                    }
                    //Check all players for a 0 score to end the game
                    if (gameMode == 1)
                    {
                        foreach (Player player in playerList)
                        {
                            if(player.Score == 0)
                            {
                                state = State.ConfirmGameEnd;
                                Console.Clear();
                                break;
                            }
                        }
                    }
                    Console.Clear();
                    break;
                }
            }
            if (y <= 0 || y >= y_max)
            {
                if (y < 0) y = 0;
                if (y > y_max) y = y_max;
                direction = !direction;
            }
            ControlFrameRate();
            break;

        case State.ConfirmGameEnd:
            PressEnterToContinue();
            if (closeRequested)
            {
                return;
            }
            state = State.Main;
            darts = new();
            break;

        default:
            throw new NotImplementedException();

    }
}

void ControlFrameRate()
{
    TimeSpan elapsed = stopwatch.Elapsed;
    if (framerate > elapsed)
    {
        Thread.Sleep(framerate - elapsed);
    }
    stopwatch.Restart();
}

void PressEnterToContinue()
{
    while (true)
    {
        switch (Console.ReadKey(true).Key)
        {
            case ConsoleKey.Enter: return;
            case ConsoleKey.Escape: closeRequested = true; return;
        }
    }
}

//Method to select cases or keys 1-4 to set players for option B
int PlayerSelectToContinue()
{
    while (true)
    {
        GetPlayerNum:
        switch (Console.ReadKey(true).Key)
        {
            case ConsoleKey.D1: return 1;
            case ConsoleKey.D2: return 2;
            case ConsoleKey.D3: return 3;
            case ConsoleKey.D4: return 4;
            default: goto GetPlayerNum;
        }
    }
}

//Method to select which gamemode the player(s) would like to play
int GameModeSelect()
{
    while (true)
    {
        GetMode:
        switch (Console.ReadKey(true).Key)
        {
            case ConsoleKey.D1: return 0;
            case ConsoleKey.D2: return 1;
            default: goto GetMode;
        }
    }
}

bool KeyPressed()
{
    bool keyPressed = false;
    while (Console.KeyAvailable)
    {
        keyPressed = true;
        if (Console.ReadKey(true).Key is ConsoleKey.Escape)
        {
            closeRequested = true;
        }
    }
    return keyPressed;
}

void Render()
{
    var render = new StringBuilder();

    if (state is State.Main)
    {
        StringBuilder output = new();
        output.AppendLine();
        output.AppendLine("  Darts");
        output.AppendLine();
        output.AppendLine("  Welcome to Darts. In this game you and the computer will");
		output.AppendLine("  throw darts at a dart board in attempts to get the most ");
		output.AppendLine("  points. If your dart lands on a line, it will round down");
		output.AppendLine("  amongst all the regions it is touching. If your dart lands");
		output.AppendLine("  on another dart it will knock both darts off the board so");
		output.AppendLine("  they will each be worth 0 points. You and the computer each");
		output.AppendLine("  get to throw 5 darts.");
		output.AppendLine();
		output.AppendLine("  Human darts: ○");
		output.AppendLine("  Computer darts: ●");
		output.AppendLine();
		output.AppendLine("  Press [escape] at any time to close the game.");
		output.AppendLine();
		output.Append("  Press [enter] to begin...");
        Console.Clear();
        Console.Write(output);
        return;
    }

    //Render for gamemode selection text
    if (state is State.ModeSelect)
    {
        StringBuilder output = new();
        output.AppendLine();
        output.AppendLine("  Darts");
        output.AppendLine();
        output.AppendLine("This game can be played in a second mode.");
        output.AppendLine();
        output.AppendLine("In this second mode each player has the goal");
        output.AppendLine("of getting their score down to 0. As you");
        output.AppendLine("Throw darts they will lower your score from 50.");
        output.AppendLine("To win the game you have to get down to 0 exactly.");
        output.AppendLine("If your latest throw results in a score below 0");
        output.AppendLine("that throw will not count and the next player's");
        output.AppendLine("will begin");
        output.AppendLine();
        output.AppendLine("If you would like to play this mode press [2]");
        output.AppendLine("Otherwise press [1] to continue.");
        Console.Clear();
        Console.Write(output);
        return;
    }

    //Render for player selection test
    if (state is State.PlayerSelect)
    {
        StringBuilder output = new();
        output.AppendLine();
        output.AppendLine("  Darts");
        output.AppendLine();
        output.AppendLine("  This game can be played with up to 4 human players,");
		output.AppendLine("  with any remaining slots given to computers.");
		output.AppendLine("  Please use the numbers below to select the ");
		output.AppendLine("  amount of human players you currently have.");
		output.AppendLine();
		output.AppendLine("  [1]    [2]     [3]     [4]");
		output.AppendLine();
		output.AppendLine();
		output.AppendLine("  Press [escape] at any time to close the game.");
		output.AppendLine();
        Console.Clear();
        Console.Write(output);
        return;
    }

    string[] board = 
    [
		"╔═══════╤═══════╤═══════╤═══════╤═══════╗",
		"║       │       │       │       │       ║",
		"║   1   │   2   │   3   │   2   │   1   ║",
		"║      ┌┴┐    ┌─┴─┐   ┌─┴─┐    ┌┴┐      ║",
		"╟──────┤6├────┤ 5 ├───┤ 5 ├────┤6├──────╢",
		"║      └┬┘    └─┬─┘   └─┬─┘    └┬┘      ║",
		"║   2   │   3   │   4   │   3   │   2   ║",
		"║       │       │  ┌─┐  │       │       ║",
		"╟───────┼───────┼──┤9├──┼───────┼───────╢",
		"║       │       │  └─┘  │       │       ║",
		"║   2   │   3   │   4   │   3   │   2   ║",
		"║      ┌┴┐    ┌─┴─┐   ┌─┴─┐    ┌┴┐      ║",
		"╟──────┤6├────┤ 5 ├───┤ 5 ├────┤6├──────╢",
		"║      └┬┘    └─┬─┘   └─┬─┘    └┬┘      ║",
		"║   1   │   2   │   3   │   2   │   1   ║",
		"║       │       │       │       │       ║",
		"╚═══════╧═══════╧═══════╧═══════╧═══════╝",
	];
    for (int i = 0; i < board.Length; i++)
    {
        for (int j = 0; j < board[i].Length; j++)
        {
            foreach (var dart in darts)
            {
                if (dart.Position == (j - 1, i - 1))
                {
                    render.Append(dart.currentPlayer.IsHuman ? '○' : '●');
                    goto DartRendered;
                }
            }
            render.Append(board[i][j]);
        DartRendered:
            continue;
        }
        if (state is State.PlayerHorizontal or State.PlayerVertical or State.ComputerHorizontal or State.ComputerVertical or State.ConfirmPlayerThrow or State.ConfirmComputerThrow)
        {
            render.Append(' ');
            if (i - 1 == y && state is not State.PlayerHorizontal or State.ComputerHorizontal)
            {
                render.Append("│██│");
            }
            else
            {
                render.Append(i is 0 ? "┌──┐" : i == board.Length - 1 ? "└──┘" : "│  │");
            }
        }
        render.AppendLine();
    }
    if (state is State.PlayerHorizontal or State.PlayerVertical or State.ComputerHorizontal or State.ComputerVertical or State.ConfirmPlayerThrow or State.ConfirmComputerThrow)
    {
        render.AppendLine("┌───────────────────────────────────────┐");
        for (int j = 0; j <= x_max + 2; j++)
        {
            render.Append (
                j - 1 == x ? '█' :
				j is 0 ? '│' :
				j == x_max + 2 ? '│' :
				' ');
        }
        render.AppendLine();
        render.AppendLine("└───────────────────────────────────────┘");
    }
    //Read the current player and accurately display who's turn it is.
    if (state is State.PlayerHorizontal or State.PlayerVertical)
    {
        render.AppendLine();
        render.AppendLine($"  Player {(currentPlayer + 1)}, it is your turn.");
        render.Append("  Press any key to aim your ○ dart... ");
        render.AppendLine();
        RenderScore(render);
    }
    if (state is State.ComputerHorizontal or State.ComputerVertical)
    {
        render.AppendLine();
        render.AppendLine($"  Computer {(currentPlayer + 1)}'s Turn. Wait for it to throw it's ● dart.");
        RenderScore(render);
    }
    //Correctly display who is going first and if they are a human or computer
    if (state is State.ConfirmRandomTurnOrder)
    {
        render.AppendLine();
        render.AppendLine("  The game will now randomly determine who is going first. ");
        render.AppendLine((playerList[playerGoesFirst].IsHuman ? "Player" : "Computer") + $" {playerGoesFirst + 1} will go first.");
        RenderScore(render);
        render.AppendLine();
        render.Append("  Press [enter] to continue...");
    }
    //Correctly display when someone throws a dart these also render the scoreboard with it
    if (state is State.ConfirmPlayerThrow)
    {
        render.AppendLine();
        render.AppendLine($"  Player {(currentPlayer + 1)} threw a dart.");
        if (darts[^1].Position is null)
        {
            render.AppendLine();
            render.AppendLine("  Dart Collision! Both darts fell off the board.");
        }
        RenderScore(render);
        render.AppendLine();
        render.Append("  Press [enter] to continue...");
    }
    if (state is State.ConfirmComputerThrow)
    {
        render.AppendLine();
        render.AppendLine($"  Computer {(currentPlayer + 1)} threw a dart.");
        if (darts[^1].Position is null)
        {
            render.AppendLine();
            render.AppendLine("  Dart Collision! Both darts fell off the board.");
        }
        RenderScore(render);   
        render.AppendLine();
        render.Append("  Press [enter] to continue...");
    }
    if (state is State.ConfirmGameEnd)
    {
        render.AppendLine();
        render.AppendLine("  Game Complete! Final Scores...");
        render.AppendLine($"  Player One Score:  {playerList[0].Score}");
        render.AppendLine($"  Player Two Score:  {playerList[1].Score}");
        render.AppendLine($"  Player Three Score:  {playerList[2].Score}");
        render.AppendLine($"  Player Four Score:  {playerList[3].Score}");
        render.AppendLine();

        // Add code to determine overall winner printout\
        if(gameMode == 0){
            int highest = -1;
            int winner = 0;
            int winnerindex = -1;
            foreach(Player player in playerList)
            {
                if(player.Score >= highest)
                {
                    highest = player.Score;
                }
            }
            foreach(Player player in playerList)
            {
                if(highest == player.Score)
                {
                    winner++;
                    winnerindex = player.PlayerNum;
                }
            }
            if(winner > 1)
            {
                render.AppendLine("  There was a Draw!");
            }
            else
            {
                render.AppendLine("  " + (playerList[winnerindex].IsHuman ? "Player" : "Computer") + $" {winnerindex + 1} Wins!");
            }
        }
        //Code to write out winner if second gamemode is active since it is the first player to reach 0 and not highest score.
        else if (gameMode == 1)
        {
            foreach (Player player in playerList)
            {
                if(player.Score == 0){
                    render.AppendLine((player.IsHuman ? "  Player " : "  Computer ") + (player.PlayerNum + 1) + " is the winner!");
                    render.AppendLine();
                }
            }
        }
        render.AppendLine();
        render.Append("  Press [enter] to return to the main screen...");

    }

    Console.CursorVisible = false;
    Console.SetCursorPosition(0, 0);
    Console.Write(render);
}

void CalculateScore(int GameMode)
{
    string[] scoreBoard =
    [
		"111111112222222233333332222222211111111",
		"111111112222222233333332222222211111111",
		"111111112222222233333332222222211111111",
		"111111162222225553333355522222261111111",
		"222222223333333344444443333333322222222",
		"222222223333333344444443333333322222222",
		"222222223333333344444443333333322222222",
		"222222223333333344494443333333322222222",
		"222222223333333344444443333333322222222",
		"222222223333333344444443333333322222222",
		"222222223333333344444443333333322222222",
		"111111162222225553333355522222261111111",
		"111111112222222233333332222222211111111",
		"111111112222222233333332222222211111111",
		"111111112222222233333332222222211111111",
	];

    foreach (Player player in playerList)
    {
        player.Score = 0;
    }

    foreach (var dart in darts)
    {
        if (dart.Position.HasValue)
        {
            if(dart.currentPlayer.PlayerNum == 0)
            {
                playerList[0].Score += scoreBoard[dart.Position.Value.Y][dart.Position.Value.X] - '0';
            }
            if(dart.currentPlayer.PlayerNum == 1)
            {
                playerList[1].Score += scoreBoard[dart.Position.Value.Y][dart.Position.Value.X] - '0';
            }
            if(dart.currentPlayer.PlayerNum == 2)
            {
                playerList[2].Score += scoreBoard[dart.Position.Value.Y][dart.Position.Value.X] - '0';
            }
            if(dart.currentPlayer.PlayerNum == 3)
            {
                playerList[3].Score += scoreBoard[dart.Position.Value.Y][dart.Position.Value.X] - '0';
            }
        }
    }
    //Check for gamemode and start subtracting from max value if in the secondary gamemode.
    if (gameMode == 1){
        foreach (Player player in playerList)
        {
            player.Score -= maxScore;
            player.Score = -player.Score;
        }
    }
    return;
}

//Method to render the scoreboard since it needs to be written in each state the game could be in.
void RenderScore(StringBuilder output)
{
    output.AppendLine();
    output.AppendLine("Scoreboard:");
    output.AppendLine($"Player 1: {playerList[0].Score}   |   Player 2: {playerList[1].Score}");
    output.AppendLine($"Player 3: {playerList[2].Score}   |   Player 4: {playerList[3].Score}");
    return;
}

enum State
{
    Main,
    ConfirmRandomTurnOrder,
    PlayerHorizontal,
    PlayerVertical,
    ConfirmPlayerThrow,
    ComputerHorizontal,
    ComputerVertical,
    ConfirmComputerThrow,
    ConfirmGameEnd,
    PlayerSelect,
    ModeSelect,
}

public class Player
{
    public Player(int num, bool human, int score)
    {
        PlayerNum = num;
        IsHuman = human;
        Score = score;
    }

    public int PlayerNum {get; }
    public bool IsHuman {get; }
    public int Score{get; set;}

}


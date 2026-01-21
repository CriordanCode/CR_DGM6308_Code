using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading;

bool closeRequested = false;

//Variables for each possible player
bool playerTurnA = false;
bool playerTurnB = false;
bool compTurnA = false;
bool compTurnB = false;

//Variable for the gamemode type
int gameMode = 0;

//Tracking for coordinates of the special space
int specialSpaceX;
int specialSpaceY;

//Triggers for whether the player receives a bonus turn or not
bool specialTrigger = false;
bool turnSkip = false;

char[,] board;


// //One time random check to see if CPU should go first (not within while loop to only go once)
// int firstTurn = Random.Shared.Next(0, 2);
//     if (firstTurn == 1)
//     {
//         playerTurnA = false;
//         compTurnB = true;
//     }

if (!closeRequested)
    {
        Console.WriteLine();
        Console.WriteLine("""
                Please Select Gamemode:
                [1] - Player vs Computer
                [2] - Player vs Player
                [3] - Computer vs Computer
                """);
        GetInput:
            //Console.CursorVisible = false;
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.D1:     gameMode = 0; break;
                case ConsoleKey.D2:     gameMode = 1; break;
                case ConsoleKey.D3:     gameMode = 2; break;
                default: goto GetInput; 
            }    
    }

//One time randomizer of turns before going based on gamemode selection
int firstTurn = Random.Shared.Next(0,2);
if(gameMode == 0)
{
    if(firstTurn == 1)
    {
        compTurnA = true;
    } else
    {
        playerTurnA = true;
    }
} else if (gameMode == 1)
{
    if(firstTurn == 1)
    {
        playerTurnB = true;
    } else
    {
    playerTurnA = true;
    }
} else
{
    if (firstTurn == 1)
    {
        compTurnA = true;
    } else
    {
        compTurnB = true;
    }
}

while (!closeRequested)
{
    board = new char[3, 3]
    {
        {' ', ' ', ' ', },
        {' ', ' ', ' ', },
        {' ', ' ', ' ', },
    };

    //Randomly chooses a "Special Space" that allows for the player to go twice in a row
    specialTrigger = false;
    specialSpaceX = Random.Shared.Next(0, 3);
    specialSpaceY = Random.Shared.Next(0, 3);

    while (!closeRequested)
    {
        if (playerTurnA)
        {
            PlayerTurn('X', turnSkip);
            turnSkip = false; 
            if (CheckForThree('X'))
            {
                EndGame("  Player 1 Wins.");
                //On win set playerTurn to false for start of next game (CPU goes first)
                playerTurnA = false;
                if(gameMode == 1)
                {
                    playerTurnB = true;
                } else
                {
                    compTurnA = true;
                }
                break;
            }   
            //If the board had their marker in the special space trigger a turnskip of the opponent 
            if (board[specialSpaceX, specialSpaceY] == 'X' && !specialTrigger)
            {
                turnSkip = true;
                
            }
                   
        }
        else if (playerTurnB)
        {
            PlayerTurn('O', turnSkip);
            turnSkip = false;
            if (CheckForThree('O'))
            {
                EndGame("  Player 2 Wins.");
                break;
            }
            if(board[specialSpaceX, specialSpaceY] == 'O' && !specialTrigger)
            {
                turnSkip = true;
            }
            
        }
        else if (compTurnA)
        {
            ComputerTurn('O');
            if (CheckForThree('O'))
            {
                if(gameMode == 0)
                {
                    EndGame("  You Lose.");
                    playerTurnA = true;
                    break;
                } else
                {
                    EndGame("  Computer O Wins.");
                    compTurnB = true;
                }
                compTurnA = false;
                break;
            }
        }
        else if (compTurnB)
        {
            //Delay so each turn isn't taken immediately
            Thread.Sleep(1000);
            ComputerTurn('X');
            Thread.Sleep(1000);
            if (CheckForThree('X'))
            {
                EndGame(" Computer X Wins.");
                compTurnB = false;
                compTurnA = true;
                break;
            }
            
        }
        //Logic to handle the switching of turns & skips this logic if the player has triggered a turn skip
        if(gameMode == 0 && !turnSkip)
        {
            playerTurnA = !playerTurnA;
            compTurnA = !compTurnA;
            turnSkip = false;
        } else if (gameMode == 1 && !turnSkip)
        {
            playerTurnA = !playerTurnA;
            playerTurnB = !playerTurnB;
            turnSkip = false;
        } else if (gameMode == 2 && !turnSkip)
        {
            compTurnA = !compTurnA;
            compTurnB = !compTurnB;
            turnSkip = false;
        } else
        {
            specialTrigger = true;
        }
        
        //Finally check if the board is full for a draw.
        if (CheckForFullBoard())
        {
            EndGame("  Draw.");
            break;
        }
    }
    
    if (!closeRequested)
    {
        Console.WriteLine();
        Console.WriteLine("  Play Again [enter], or quit [escape]?");
        GetInput:
            Console.CursorVisible = false;
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.Enter: break;
                case ConsoleKey.Escape:
                    closeRequested = true;
                    break;
                default: goto GetInput;
        }
    }
    Console.CursorVisible = true;

    void PlayerTurn(char token, bool specialTrigger)
    {
        var (row, column) = (0, 0);
        bool moved = false;
        while (!moved && !closeRequested)
        {
            Console.Clear();
            RenderBoard();
            Console.WriteLine();
            if(token == 'X' && !specialTrigger)
            {
                Console.WriteLine("It is Player 1's Turn.");
            } else if (token == 'O' && !specialTrigger)
            {
                Console.WriteLine("It is Player 2's Turn.");
            } else
            {
                Console.WriteLine("You Triggered Special Space Go Again!");
            }
            Console.WriteLine(" Use the arrow and enter keys to select a move or select the space using the NumPad [1-9].");
            Console.SetCursorPosition(column * 4 + 4, row * 2 + 4);
            Console.CursorVisible = true;
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.UpArrow:    row = row <= 0 ? 2 : row - 1; break;
                case ConsoleKey.DownArrow:  row = row >= 2 ? 0 : row + 1; break;
                case ConsoleKey.LeftArrow:  column = column <= 0 ? 2 : column - 1; break;
                case ConsoleKey.RightArrow: column = column >= 2 ? 0 : column + 1; break;
                case ConsoleKey.Enter:
                    if (board[row, column] is ' ')
                    {
                        board[row, column] = token;
                        moved = true;
                    }
                    break;
                case ConsoleKey.Escape:
                    Console.Clear();
                    closeRequested = true;
                    break;

                //Cases for numpad support (This should work however my keyboard is returning top row number codes when using the numpad)
                //I don't know why but every keyboard i tested with returns Numpad keys like they are the normal top row numbers
                //Therefore instead of ConsoleKey.NumPad* it is using ConsoleKey.D* for usability
                case ConsoleKey.D1:
                    if (board[2, 0] is ' ')
                    {
                        board[2, 0] = token;
                        moved = true;
                    }
                    break;
                case ConsoleKey.D2:
                    if (board[2, 1] is ' ')
                    {
                        board[2, 1] = token;
                        moved = true;
                    }
                    break;
                case ConsoleKey.D3:
                    if (board[2, 2] is ' ')
                    {
                        board[2, 2] = token;
                        moved = true;
                    }
                    break;
                case ConsoleKey.D4:
                    if (board[1, 0] is ' ')
                    {
                        board[1, 0] = token;
                        moved = true;
                    }
                    break;
                case ConsoleKey.D5:
                    if (board[1 ,1] is ' ')
                    {
                        board[1 ,1] = token;
                        moved = true;
                    }
                    break;
                case ConsoleKey.D6:
                    if (board[1, 2] is ' ')
                    {
                        board[1, 2] = token;
                        moved = true;
                    }
                    break;
                case ConsoleKey.D7:
                    if (board[0, 0] is ' ')
                    {
                        board[0, 0] = token;
                        moved = true;
                    }
                    break;
                case ConsoleKey.D8:
                    if (board[0, 1] is ' ')
                    {
                        board[0, 1] = token;
                        moved = true;
                    }
                    break;
                case ConsoleKey.D9:
                    if (board[0, 2] is ' ')
                    {
                        board[0, 2] = token;
                        moved = true;
                    }
                    break;

            }
        }
    }

    void ComputerTurn(char token)
    {
        //In the event of CPU vs CPU this allows for the board to render before the ending
        Console.Clear();
        RenderBoard();
        Console.WriteLine();
        var possibleMoves = new List<(int X, int Y)>();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (board[i, j] == ' ') 
                {
                        possibleMoves.Add((i, j));
                }
            }
        }
        int index = Random.Shared.Next(0, possibleMoves.Count);
        var (X, Y) = possibleMoves[index];
        board[X, Y] = token;
    }

    bool CheckForThree(char c) =>
        board[0, 0] == c && board[1, 0] == c && board[2, 0] == c ||
	    board[0, 1] == c && board[1, 1] == c && board[2, 1] == c ||
	    board[0, 2] == c && board[1, 2] == c && board[2, 2] == c ||
	    board[0, 0] == c && board[0, 1] == c && board[0, 2] == c ||
	    board[1, 0] == c && board[1, 1] == c && board[1, 2] == c ||
	    board[2, 0] == c && board[2, 1] == c && board[2, 2] == c ||
	    board[0, 0] == c && board[1, 1] == c && board[2, 2] == c ||
	    board[2, 0] == c && board[1, 1] == c && board[0, 2] == c;

    bool CheckForFullBoard() =>
        board[0, 0] != ' ' && board[1, 0] != ' ' && board[2, 0] != ' ' &&
	    board[0, 1] != ' ' && board[1, 1] != ' ' && board[2, 1] != ' ' &&
	    board[0, 2] != ' ' && board[1, 2] != ' ' && board[2, 2] != ' ';

    void RenderBoard()
    {
        Console.WriteLine($"""

		  Tic Tac Toe

		  ╔═══╦═══╦═══╗
		  ║ {board[0, 0]} ║ {board[0, 1]} ║ {board[0, 2]} ║
		  ╠═══╬═══╬═══╣
		  ║ {board[1, 0]} ║ {board[1, 1]} ║ {board[1, 2]} ║
		  ╠═══╬═══╬═══╣
		  ║ {board[2, 0]} ║ {board[2, 1]} ║ {board[2, 2]} ║
		  ╚═══╩═══╩═══╝
		""");
    }

    void EndGame(string message)
    {
        Console.Clear();
        RenderBoard();
        Console.WriteLine();
        Console.Write(message);
    }
    
}
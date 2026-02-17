using System.Diagnostics;

namespace Checkers;

//Class to control the game
public class Game
{
	//Private for the game class to set the amount of pieces per color
	private const int PiecesPerColor = 12;

	//Private to random add traps until 
	private const bool ShopAdded = false;

	//Private Add Trap Counter
	private int turnSinceTrap = 0;
	
	//Private delay for when a trap should be added
	private int trapDelay = 5;


	//Public variable for the turn, public get but private set
	public PieceColor Turn { get; private set; }
	//Public board retrieval during the game
	public Board Board { get; }
	//Public get and private set for the color who the winner is
	public PieceColor? Winner { get; private set; }
	//List of players with the get method available
	public List<Player> Players { get; }

	//Constructor that requires how many human players are involved
	public Game(int humanPlayerCount)
	{
		//If theree are less than 0 human players or more than 2 throw an exception since that isn't possible
		if (humanPlayerCount < 0 || 2 < humanPlayerCount) throw new ArgumentOutOfRangeException(nameof(humanPlayerCount));
		//Create a new board
		Board = new Board();
		//Create a new players for player 1 and 2
		Players = new()
		{
			new Player(humanPlayerCount >= 1, Black),
			new Player(humanPlayerCount >= 2, White),
		};
		//Start turn with black
		Turn = Black;
		//Set winner as null
		Winner = null;
	}

	//Method to allow for a move to happen
	public void PerformMove(Move move)
	{
		//Create touble for move.To
		(move.PieceToMove.X, move.PieceToMove.Y) = move.To;
		//If they move to the edge of the board change the piece to be promoted
		if ((move.PieceToMove.Color is Black && move.To.Y is 7) ||
			(move.PieceToMove.Color is White && move.To.Y is 0))
		{
			move.PieceToMove.Promoted = true;
		}
		//If there is a piece to capture during the move, remove that piece
		if (move.PieceToCapture is not null)
		{
			Board.Pieces.Remove(move.PieceToCapture);
		}
		//If there is a piece to capture with possible moves that aren't null for capture set the board agressor (Keeps the player moving?)
		if (move.PieceToCapture is not null &&
			Board.GetPossibleMoves(move.PieceToMove).Any(m => m.PieceToCapture is not null))
		{
			Board.Aggressor = move.PieceToMove;
		}
		//Set the board agressor to null and switch turns
		else
		{
			Board.Aggressor = null;
			Turn = Turn is Black ? White : Black;
		}
		//Check for a winner before moving on with the game after a move has been made
		CheckForWinner();
		
		//Call the method to resolve any traps on the board
		CheckForTraps();
		//Since there is not a trap placed this turn increment the counter
		turnSinceTrap++;
		//Until the shop is added, create a random trap on an open space until 
		if (!ShopAdded && turnSinceTrap == trapDelay)
		{
			Board.CreateTrapRand(Neutral);
			turnSinceTrap = 0;
		}
		
	}

	//Check to see if a win condition has been satisfied
	public void CheckForWinner()
	{
		//If no black pieces remain set the winner to white
		if (!Board.Pieces.Any(piece => piece.Color is Black))
		{
			Winner = White;
		}
		//If no white pieces remain set the winner to black
		if (!Board.Pieces.Any(piece => piece.Color is White))
		{
			Winner = Black;
		}
		//If there is no winner and there are no possible moves the winner is the last person to make a move
		if (Winner is null && Board.GetPossibleMoves(Turn).Count is 0)
		{
			Winner = Turn is Black ? White : Black;
		}
	}

	//Method to count the amount of pieces that have been taken (uses how many piecess remain to count against)
	public int TakenCount(PieceColor colour) =>
		PiecesPerColor - Board.Pieces.Count(piece => piece.Color == colour);

	//Method to check and resolve any traps that remain on the board
	public void CheckForTraps()
	{
		//Printout line to check method is being called
		//Console.WriteLine("Checking for traps");
		//Run while there are any traps on the board
		while(Board.Traps.Count > 0)
		{
			//Get basic information about the current trap being acted on
			int range = Board.Traps[0].Range;
			int xPos = Board.Traps[0].X;
			int yPos = Board.Traps[0].Y;
			//Boolean for if there is a piece to be removed by the trap
			bool removePiece = false;
			//printoutline to check method was reading a trap
			//Console.WriteLine("Reading Range of Trap " + range);

			//List of pieces that need to be removed from the trap
			List<Piece> removeByTrap = new List<Piece>();
			//Iterate through all of the pieces on the board
			foreach(Piece piece in Board.Pieces)
			{

				//Nested for loop that looks at -1/+1 (if range of bomb is only 1, if larger will account for that as well) around the trap position
				for(int xIter = xPos - range; xIter <= xPos + range; xIter++)
				{
					for(int yIter = yPos - range; yIter <= yPos + range; yIter++)
					{
						//First check to make sure that this is inrange before acting upon it, if not continue 
						//(helps with edge cases of a trap being on an edge of the board)
						if(Board.IsValidPosition(xIter, yIter)){
							//For when the position is equal to the trap/skip over center
							if(yIter == yPos && xIter == xPos)
							{
								continue;
							}
							//If the piece is equal to the spot in the loop it will remove it	
							if(piece.X == xIter && piece.Y == yIter)
							{
								removePiece = true;
							}
						}
					}
				}
				//Add to the list of pieces that need to be removed from the trap
				if(removePiece){
					removeByTrap.Add(piece);		
					removePiece = false;
				}
			}
			//Go through the removeByTrap list and remove from the list of pieces on the board 
			//and then remove from the first of the list of pieces in removeByTrap
			while(removeByTrap.Count > 0)
			{
				Board.Pieces.Remove(removeByTrap[0]);
				removeByTrap.RemoveAt(0);
			}
			//Remove the first trap from both the board and the list of traps
			Board.Pieces.Remove(Board.Traps[0]);
			Board.Traps.RemoveAt(0);

		}
			
	}
}



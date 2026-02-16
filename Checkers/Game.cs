using System.Diagnostics;

namespace Checkers;

//Class to control the game
public class Game
{
	//Private for the game class to set the amount of pieces per color
	private const int PiecesPerColor = 12;

	//Private to random add traps until 
	private const bool ShopAdded = false;

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
		
		CheckForTraps();
		//Until the shop is added, create a random trap on an open space until 
		if (!ShopAdded)
		{
			Board.CreateTrapRand(Neutral);
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

	public void CheckForTraps()
	{
		while(Board.Traps.Count > 0)
		{
			foreach(Piece piece in Board.Pieces)
			{
				for(int i = 1; i <= Board.Traps[0].Range; i++)
				{
					if(((piece.X + i) == Board.Traps[0].X || (piece.X - i) == Board.Traps[0].X) &&	
				   		((piece.Y + i) == Board.Traps[0].Y || (piece.Y - i) == Board.Traps[0].Y)){
						Board.Pieces.Remove(piece);
					} 
				}
			}
			Board.Pieces.Remove(Board.Traps[0]);
			Board.Traps.RemoveAt(0);
		}
	}

}

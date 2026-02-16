namespace Checkers;




public class Trap : Piece
{

    public int Cost { get; }
    public int Range { get; }

    // public PieceColor TrapOwner { get; init; }

    public string TrapPosition
    {
        get => Board.ToPositionNotationString(X, Y);
		set => (X, Y) = Board.ParsePositionNotation(value);
    }

    

    public Trap(int xPos, int yPos, PieceColor owner)
    {
        X = xPos;
        Y = yPos;
        Cost = 3;
        Color = owner;
        Range = 1;
    }

    public Trap()
    {
        X = Random.Shared.Next(0,7);
        Y = Random.Shared.Next(0,7);
        Cost = 0;
        Color = Neutral;
        Range = 1;
    }

    public Trap(PieceColor owner)
    {
        X = Random.Shared.Next(0, 7);
        Y = Random.Shared.Next(0, 7);
        Cost = 0;
        Color = owner;
        Range = 1;
    }
}



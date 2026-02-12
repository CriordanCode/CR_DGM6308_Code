namespace Checkers;






public class Trap
{
    

    public int X { get;set; }
    public int Y { get;set; }
    public int Cost { get; }
    public int Range { get; }

    public PieceColor TrapOwner { get; init; }

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
        TrapOwner = owner;
        Range = 1;
    }

    public Trap()
    {
        X = Random.Shared.Next(0,8);
        Y = Random.Shared.Next(0,8);
        Cost = 0;
        TrapOwner = (PieceColor) Random.Shared.Next(0, 2);
        Range = 1;
    }

}



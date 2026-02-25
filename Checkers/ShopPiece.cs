namespace Checkers;






public class ShopPiece : Piece
{
    private int Cost {get;}



    public ShopPiece(int price)
    {
        Cost = price;

    }

    public int getCost()
    {
        return this.Cost;
    }

}
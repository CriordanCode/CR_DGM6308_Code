namespace Checkers;








public class Shop
{
    private List<ShopPiece> Inventory;

    private StringBuilder display;


    public Shop()
    {
        Inventory = new List<ShopPiece>();
        display = new StringBuilder();
    }

    public void addShopItem(ShopPiece item)
    {
        Inventory.Add(item);
    }
    public void createShopRender()
    {
        display.AppendLine( "|--------------------------------------|");
        display.AppendLine( "|                 Shop                 |");
        display.AppendLine( "|--------------------------------------|");
        display.AppendLine($"|            |            |            |");
        display.AppendLine($"|            |            |            |");
        display.AppendLine($"|   Cost: {Inventory[0].getCost()}   |   Cost: {Inventory[1].getCost()}  |   Cost: {Inventory[2].getCost()}  |");
        display.AppendLine( "|--------------------------------------|");
    }
}
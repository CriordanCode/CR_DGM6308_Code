namespace Checkers;








public class Shop
{
    private List<ShopPiece> Inventory;




    public Shop()
    {
        Inventory = new List<ShopPiece>();
    }

    public void addShopItem(ShopPiece item)
    {
        Inventory.Add(item);
    }

    public void RenderShop(List<String> display)
    {
        display[3]  += "   ╔══════════════════════════════════════╗";
        display[4]  += "   ║                 Shop                 ║";
        display[5]  += "   ║══════════════════════════════════════║";
        display[6]  += "   ║            ║            ║            ║";
        display[7]  += "   ║            ║            ║            ║";
        display[8]  += "   ║            ║            ║            ║";
        display[9]  += "   ║            ║            ║            ║";
        display[10] += "   ║            ║            ║            ║";
        display[11] += "   ╚══════════════════════════════════════╝";
    }

    public void ClearShop(List<String> display)
    {
        display[3]  += "                                           ";
        display[4]  += "                                           ";
        display[5]  += "                                           ";
        display[6]  += "                                           ";
        display[7]  += "                                           ";
        display[8]  += "                                           ";
        display[9]  += "                                           ";
        display[10] += "                                           ";
        display[11] += "                                           ";
    }
}
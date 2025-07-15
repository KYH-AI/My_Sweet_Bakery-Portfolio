public class BreadBasketTray : TrayBase
{
    public int _MAX_BREAD_BASKET_COUNT = 16;
    
    public override bool CanAddTrayItem()
    {
        return _MAX_BREAD_BASKET_COUNT > GetTrayObjectCount();
    }
}

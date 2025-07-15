public class ParperBagTray : TrayBase
{
    public int MaxParperBagCount = 1;
    
    public override bool CanAddTrayItem()
    {
        return MaxParperBagCount > GetTrayObjectCount();
    }
}

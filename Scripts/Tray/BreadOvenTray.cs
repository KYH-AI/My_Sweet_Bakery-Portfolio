public class BreadOvenTray : TrayBase
{
    public int _MAX_BREAD_OVEN_TRAY_COUNT = 10;

    public override bool CanAddTrayItem()
    {
        return _MAX_BREAD_OVEN_TRAY_COUNT > GetTrayObjectCount();
    }
}

using UnityEngine;

public class TableTray : TrayBase
{
    [SerializeField] private int _MAX_TABLE_TRAY_COUNT;
    
    protected override void Awake()
    {
        _OnTrayItemTypeChanageEvent -= ChangeTrayItemTypeChangeType;
        _OnTrayItemTypeChanageEvent += ChangeTrayItemTypeChangeType;
    }
    
    public override bool CanAddTrayItem()
    {
        return _MAX_TABLE_TRAY_COUNT > GetTrayObjectCount();
    }
}

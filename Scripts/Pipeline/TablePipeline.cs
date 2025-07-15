public class TablePipeline : Upgarde
{
    private TableArea[] _baseUpgradeable;
    private TableArea _currentObject;
    
    protected override void Init()
    {
        _baseUpgradeable = GetComponentsInChildren<TableArea>(true);
        _currentObject = _baseUpgradeable[0];
        
        var upgradeData = _currentObject.GetUpgradeData;
        upgradeData.FillArea.OnUpgradeEvent -= RemoveLegacyObject;
        upgradeData.FillArea.OnUpgradeEvent += RemoveLegacyObject;
        upgradeData.FillArea.OnUpgradeEvent -= Upgrade;
        upgradeData.FillArea.OnUpgradeEvent += Upgrade;
    }

    private bool IsMaxLevel()
    {
        int currentLevel = _currentObject.GetUpgradeData.Level;
        int nextLevel = currentLevel + 1;
    
        // 최대 레벨 체크
        if (nextLevel >= _baseUpgradeable.Length)
        {
            return true;
        }
        return false;
    }

    public override void RemoveLegacyObject()
    {
        if (_currentObject == null) return;
        // 최대 레벨 체크
        if (IsMaxLevel()) 
        {
            _currentObject.GetUpgradeData.FillArea.OnUpgradeEvent -= RemoveLegacyObject;
            return;
        }
        
        _currentObject.Despawn();
        _currentObject.gameObject.SetActive(false);
    }

    public override void Upgrade()
    {
        if (_currentObject == null) return;
        // 최대 레벨 체크
        if (IsMaxLevel())
        {
            _currentObject.GetUpgradeData.FillArea.OnUpgradeEvent -= Upgrade;
            return;
        }
        
        _currentObject = _baseUpgradeable[_currentObject.GetUpgradeData.Level + 1];
        _currentObject.gameObject.SetActive(true);
        _currentObject.Spawn();
        var upgradeData = _currentObject.GetUpgradeData;
        
        upgradeData.FillArea.OnUpgradeEvent -= RemoveLegacyObject;
        upgradeData.FillArea.OnUpgradeEvent += RemoveLegacyObject;
        upgradeData.FillArea.OnUpgradeEvent -= Upgrade;
        upgradeData.FillArea.OnUpgradeEvent += Upgrade;
    }

    public override BaseUpgradeable GetCurrentObject()
    {
        return _currentObject;
    }

    public override int GetCurrentObjectLevel()
    {
        if (_currentObject)
        {
            return _currentObject.GetUpgradeData.Level;
        }
        return 0;
    }
    
    public Table FindTable(int guestCount)
    {
        foreach (var table in _currentObject.GetAllTables())
        {
            // 테이블 사용 여부 확인
            if(!table.CanUseObject())  continue;
            // 테이블 의자 갯수 확인
            if(table.ChairCount() < guestCount) continue;

            return table;
        }
        return null;
    }

    public Table GetTableByIndex(int index)
    {
        return _currentObject.GetTableByIndex(index);
    }
}

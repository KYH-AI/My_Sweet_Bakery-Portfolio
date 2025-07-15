using System.Collections.Generic;
using UnityEngine;

public class TableArea : BaseUpgradeable
{
    [SerializeField] private Table[] _tables;
    [SerializeField] private Transform _spawnVFXRoot;
    private bool _isShowedFocusTable = false;
    
    public override void Spawn()
    {
        if (!_spawnVFXRoot) return;
        var appear = GameManager.Instance.ObjectPoolManager.Spawn<VFX_AppearSignStand>();
        appear.gameObject.transform.SetParent(_spawnVFXRoot);
        appear.transform.localPosition = Vector3.zero;
    }

    public override void Despawn()
    {
        foreach (var removeObject in _removeObjects)
        {
            removeObject.SetActive(false);
        }
    }


    public IReadOnlyCollection<Table> GetAllTables()
    {
        if (!_isShowedFocusTable && _tables.Length == 0)
        {
            GameManager.Instance.MainCamera.LookAtTarget(this.gameObject.transform.position);
            _isShowedFocusTable = true;
        }
        
        return _tables;
    }

    public Table GetTableByIndex(int index)
    {
        if (_tables.Length - 1 >= index)
        {
            return _tables[index];
        }

        return null;
    }
    
}

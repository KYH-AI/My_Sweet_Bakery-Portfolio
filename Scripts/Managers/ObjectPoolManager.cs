using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    [SerializeField] private List<PoolRoot> _poolRoots = new List<PoolRoot>();

    private Dictionary<PoolRootType, PoolRoot> _poolRootCache;
    private ObjectPoolFactory _objectPoolFactory;
    
    public void Init()
    {
        _poolRootCache = new Dictionary<PoolRootType, PoolRoot>(_poolRoots.Count);
        foreach (PoolRoot root in _poolRoots)
        {
            if (!_poolRootCache.ContainsKey(root.GetRootType))
            {
                _poolRootCache.Add(root.GetRootType, root);
            }
        }
        
        _objectPoolFactory = new ObjectPoolFactory(this);
    }

    public PoolRoot GetPoolRoot(PoolRootType rootType)
    {
        return _poolRootCache[rootType];
    }

    public T Spawn<T>() where  T : IObjectPoolable
    {
        return _objectPoolFactory.Spawn<T>();
    }
    
    
    public void ReturnPool(IObjectPoolable poolable)
    {
        _objectPoolFactory.ReturnPool(poolable);
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolFactory
{
    private Dictionary<Type, Queue<IObjectPoolable>> _pooledGameObjects;
    private Dictionary<Type, IObjectPoolable> _prefabCache;

    public ObjectPoolFactory(ObjectPoolManager manager)
    {
        GameObject[] poolObjects1 = ResourceLoad.FindResources<GameObject>("TrayItems");
        GameObject[] poolObjects2 = ResourceLoad.FindResources<GameObject>("Particles");
        GameObject[] poolObjects3 = ResourceLoad.FindResources<GameObject>("Characters");
        Clear();
        _pooledGameObjects = new Dictionary<Type, Queue<IObjectPoolable>>(poolObjects1.Length + poolObjects2.Length + poolObjects3.Length);
        _prefabCache = new Dictionary<Type, IObjectPoolable>(poolObjects1.Length  + poolObjects2.Length + poolObjects3.Length);

        foreach (GameObject poolPrefab in poolObjects1)
        {
            if (poolPrefab.TryGetComponent(out IObjectPoolable objectPoolable))
            {
                _prefabCache.Add(objectPoolable.GetType(), objectPoolable);   
                PoolRoot root = manager.GetPoolRoot(objectPoolable.GetRootType());
                MakePool(root, objectPoolable);
            }
        }
        
        foreach (GameObject poolPrefab in poolObjects2)
        {
            if (poolPrefab.TryGetComponent(out IObjectPoolable objectPoolable))
            {
                _prefabCache.Add(objectPoolable.GetType(), objectPoolable);   
                PoolRoot root = manager.GetPoolRoot(objectPoolable.GetRootType());
                MakePool(root, objectPoolable);
            }
        }
        
        foreach (GameObject poolPrefab in poolObjects3)
        {
            if (poolPrefab.TryGetComponent(out IObjectPoolable objectPoolable))
            {
                _prefabCache.Add(objectPoolable.GetType(), objectPoolable);   
                PoolRoot root = manager.GetPoolRoot(objectPoolable.GetRootType());
                MakePool(root, objectPoolable);
            }
        }
    }
    
    public T Spawn<T>() where T : IObjectPoolable
    {
        Type type = typeof(T);
        if (!_pooledGameObjects.ContainsKey(type))
        {
            DebugLog.CustomLog($"{type.Name}은 풀에 등록된 오브젝트가 아닙니다!", Color.red);
            return default;
        }

        IObjectPoolable poolObject = null;
        
        while (_pooledGameObjects[type].Count > 0)
        {
            poolObject = _pooledGameObjects[type].Dequeue();
            if (poolObject.GameObject)
            {
                break;
            }
            poolObject = null;
        }

        // 사용 가능한 오브젝트가 없는 경우 
        if (poolObject == null)
        {
            poolObject = CreateNewPooledObject(type);
            if (poolObject.GameObject)
            {
                _pooledGameObjects[type].Dequeue();
            }
        }
        poolObject.OnSpawn();

        // GameObject에서 컴포넌트 직접 가져오기
        return poolObject.GameObject.GetComponent<T>();
    }

    public void ReturnPool(IObjectPoolable poolObject)
    {
        Type type = poolObject.GetType();
        if (!_pooledGameObjects.ContainsKey(type))
        {
            DebugLog.CustomLog($"{type.Name}은 이 풀에서 관리하는 타입이 아닙니다!", Color.red);
            return;
        }
        
        poolObject.OnDespawn();
        PoolRoot root = GameManager.Instance.ObjectPoolManager.GetPoolRoot(poolObject.GetRootType());
        poolObject.Transform.SetParent(root.transform);
        _pooledGameObjects[type].Enqueue(poolObject);
    }
    
    private void MakePool(PoolRoot root, IObjectPoolable metaData)
    {
        Type type = metaData.GetType();
        if (_pooledGameObjects.ContainsKey(type))
        {
            DebugLog.CustomLog($"{type.Name}은 이미 풀에 등록된 오브젝트 입니다!", Color.red);
            return;
        }
        _pooledGameObjects.Add(type, new Queue<IObjectPoolable>(metaData.PoolSize));
        for (int i = 0; i < metaData.PoolSize; i++)
        {
            GameObject poolable = ResourceLoad.Instantiate(metaData.GameObject, root.transform);
            if (!poolable.activeSelf)
            {
                DebugLog.CustomLog($"{type.Name}오브젝트 프리팹을 activeSelf 활성화 해주세요!", Color.yellow);
            }
            IObjectPoolable commponent = poolable.GetComponent<IObjectPoolable>();
            commponent.GameObject.SetActive(false);
            commponent.OnDespawn();
            commponent.Transform.parent = root.transform;
          
            _pooledGameObjects[type].Enqueue(commponent);
        }
    }

    private IObjectPoolable CreateNewPooledObject(Type type)
    {
        if (!_prefabCache.ContainsKey(type))
            return null;

        IObjectPoolable prefab = _prefabCache[type];
        if (!prefab.GameObject)
        {
            DebugLog.CustomLog($"프리팹 캐쉬에 {type.Name} 오브젝트가 존재하지 않습니다!", Color.red);
            return null;
        }
        
        PoolRoot root = GameManager.Instance.ObjectPoolManager.GetPoolRoot(prefab.GetRootType());
        GameObject newObj = ResourceLoad.Instantiate(prefab.GameObject, root.transform);
        IObjectPoolable component = newObj.GetComponent<IObjectPoolable>();
        
        component.OnDespawn(); // 비활성화 상태로 초기화
        
        _pooledGameObjects[type].Enqueue(component);
        return component;
    }
    
    private void Clear()
    {
        if (_pooledGameObjects == null || _pooledGameObjects.Count == 0) return;
        
        foreach (var pair in _pooledGameObjects)
        {
            foreach (IObjectPoolable item in pair.Value)
            {
                if (item.GameObject)
                {
                    UnityEngine.Object.Destroy(item.GameObject);
                }
            }
        }
        _pooledGameObjects.Clear();
    }
}

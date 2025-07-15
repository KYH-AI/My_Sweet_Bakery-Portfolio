using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public abstract class TrayBase : MonoBehaviour
{
    [SerializeField] private int _x;
    [SerializeField] private int _y;
    [SerializeField] private Vector3 _size;
    [SerializeField] private float _dropInterval = 0.2f;
    [SerializeField] private float _jumpPower = 5f;

    protected Queue<GameObject> _trayMovingObjects = new Queue<GameObject>();
    protected Stack<GameObject> _trayInObjects = new Stack<GameObject>();
    protected event Action<TrayPickItemType> _OnTrayItemTypeChanageEvent;
    protected event Action _OnTrayRemoveItemEvent; 
    public TrayPickItemType TrayPickItemType { get; protected set; } = TrayPickItemType.None;

    protected virtual void Awake()
    {
        _OnTrayItemTypeChanageEvent = null;
        _OnTrayRemoveItemEvent = null;
    }
    
    public void ChangeTrayItemTypeChangeType(TrayPickItemType type)
    {
        if (TrayPickItemType == type) return;
        TrayPickItemType = type;
    }
    
    public int GetTrayObjectCount()
    {
        return _trayInObjects.Count + _trayMovingObjects.Count;
    }

    protected int GetTrayQueueObjectCount()
    {
        return _trayInObjects.Count;
    }

    public void AddTrayByObject(GameObject gameObject, TrayPickItemType itemType, bool doJump = true, bool useRotation = false)
    {
        _OnTrayItemTypeChanageEvent?.Invoke(itemType);
        _trayMovingObjects.Enqueue(gameObject);
        _trayInObjects.Push(gameObject);
        gameObject.transform.SetParent(this.transform);
        Vector3 dest = GetTrayPositionByIndex(_trayInObjects.Count - 1);
        if (doJump)
        {
            gameObject.transform.DOJump(dest, _jumpPower, 1, 0.3f).OnComplete(() =>
            {
                // 로컬 위치 재조정
                if (_x == 1 && _y == 1)
                {
                    gameObject.transform.localPosition = new Vector3(0f, gameObject.transform.localPosition.y, 0f);
                }
                if (!useRotation)
                {
                    gameObject.transform.localRotation = Quaternion.identity;
                }
                _trayMovingObjects.Dequeue();
            });
        }
        else
        {
            // 로컬 위치 재조정
            if (_x == 1 && _y == 1)
            {
                gameObject.transform.localPosition = new Vector3(0f, gameObject.transform.localPosition.y, 0f);
            }
            gameObject.transform.position = dest;
            if (!useRotation)
            {
                gameObject.transform.localRotation = Quaternion.identity;
            }
            _trayMovingObjects.Dequeue();
        }
    }

    public GameObject RemoveFromTrayObject(bool keepTrayItemType = false, bool removeAll = false)
    {
        if (_trayInObjects.Count == 0)
        {
            return null;
        }

        GameObject item = _trayInObjects.Pop();
        if (removeAll)
        {
            while (_trayInObjects.Count > 0)
            {
                GameObject trayItem = _trayInObjects.Pop();
                GameManager.Instance.ObjectPoolManager.ReturnPool(trayItem.GetComponent<IObjectPoolable>());
            }
        }
        
        if (!keepTrayItemType)
        {
            if (_trayInObjects.Count + _trayMovingObjects.Count == 0)
            {
                TrayPickItemType = TrayPickItemType.None;
            }
        }

        _OnTrayRemoveItemEvent?.Invoke();
        return item;
    }
    
    

    private Vector3 GetTrayPositionByIndex(int index)
    {
        Vector3 offset = new Vector3((_x - 1) * _size.x / 2, 
                                        0, 
                                        (_y - 1) * _size.z / 2);
        Vector3 startPos = this.transform.position - offset;

        int x = (index / _x) % _y;
        int y = index % _x;
        int height = index / (_x * _y);

        float X = startPos.x + y * _size.x;
        float Y = startPos.y + height * _size.y;
        float Z = startPos.z + x * _size.z;

        return new Vector3(X, Y, Z);
    }

    public abstract bool CanAddTrayItem();

    private void OnDestroy()
    {
        _OnTrayItemTypeChanageEvent = null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector3 offset = new Vector3((_x - 1) * _size.x / 2, 
                                        0, 
                                        (_y - 1) * _size.z / 2);
        Vector3 startPos = this.transform.position - offset;
        Gizmos.color = Color.yellow;

        for (int x = 0; x < _x; x++)
        {
            for (int y = 0; y < _y; y++)
            {
                Vector3 center = startPos + new Vector3(x * _size.x, _size.y / 2, y * _size.z);
                Gizmos.DrawWireCube(center, _size);
            }
        }
    }
#endif


}

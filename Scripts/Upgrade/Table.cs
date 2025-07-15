using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Table : MonoBehaviour
{
    [SerializeField] private WaitingPoints _tableWayPoints;
    [SerializeField] private Transform[] _chairs;
    [SerializeField] private int _moneySpawnPackageCount;
    [SerializeField] private float _minEatingTime = 3f;
    [SerializeField] private float _maxEatingTime = 10f;
    [SerializeField] private Transform _cleanVFXRoot;

    [SerializeField] private TriggerZone _tableTrigger;
    [SerializeField] private TriggerZone _moneyTrigger;
    [SerializeField] private MoneyTray _moneyTray;
    [SerializeField] private TableTray _tableTray;

    private Queue<CustomerController> _customers;
    private float _eatingTime;

    public TableStateType TableStateType { get; private set; }

    private void Start()
    {
        _customers = new Queue<CustomerController>(_chairs.Length);
        _tableTrigger.OnTriggerEvent -= OnTableTriggerInteraction;
        _tableTrigger.OnTriggerEvent += OnTableTriggerInteraction;
        _moneyTrigger.OnTriggerEvent -= OnMoneyTriggerInteraction;
        _moneyTrigger.OnTriggerEvent += OnMoneyTriggerInteraction;
    }
    
    private void Update()
    {
        UpdateTableWayPoint();
        UpdateTableState();
    }

    private void UpdateTableWayPoint()
    {
        if (_customers.Count == 0) return;
        if (_customers.Peek().CustomerStateType != CustomerStateType.Serving) return;
        
        foreach (var customer in _customers)
        {
            if (!customer.HasArrivedDestination()) continue;
            if (customer.CurrentWaitingIndex > 0)
            {
                customer.CurrentWaitingIndex--;
                Vector3 frontWaitingLine = _tableWayPoints.GetWayPoint(customer.CurrentWaitingIndex).position;
                customer.SetDestination(frontWaitingLine);
            }
        }
    }
    
    private void UpdateTableState()
    {
        if (TableStateType is TableStateType.Reserved)
        {
            // 의자 앉기 확인
            foreach (var c in _customers)
            {
                if (!c.HasArrivedDestination())
                {
                    return;
                }
            }

            // 의자 앉기 및 먹기
            int chairIndex = -1;
            foreach (var c in _customers)
            {
                chairIndex++;
                c.StopAgent();
                c.ChangeCustomerState(CustomerStateType.Eating);
                c.transform.position = _chairs[chairIndex].position;
                c.transform.rotation = _chairs[chairIndex].rotation;
                GameObject bread = c.ThisTray.RemoveFromTrayObject(false, true);
                if(!bread) continue;
                _tableTray.AddTrayByObject(bread, TrayPickItemType.Croissant, false);
            }

            _eatingTime = UnityEngine.Random.Range(_minEatingTime, _maxEatingTime + 1);
            ChangeTableState(TableStateType.Eating);
        }
        else if (TableStateType is TableStateType.Eating)
        {
            _eatingTime -= Time.deltaTime;
            if (_eatingTime > 0) return;

            _eatingTime = 0;

            // 식사 종료, 테이블 빵 모두 제거
            var bread = _tableTray.RemoveFromTrayObject(false, true);
            GameManager.Instance.ObjectPoolManager.ReturnPool(bread.GetComponent<IObjectPoolable>());

            int moneySpawnRemaining = 0;
            
            // 테이블 쓰레기 생성, 돈 생성
            for (int i = 0; i < _customers.Count; i++)
            {
                var garbage = GameManager.Instance.ObjectPoolManager.Spawn<Garbage>();
               _tableTray.AddTrayByObject(garbage.gameObject, TrayPickItemType.Garbage, false);
               moneySpawnRemaining += _moneySpawnPackageCount;
            }
            
            // TODO : 의자 넘어트리기

            for (int i = moneySpawnRemaining; i > 0; i--)
            {
                var money = GameManager.Instance.ObjectPoolManager.Spawn<Money>();
                _moneyTray.AddTrayByObject(money.gameObject, TrayPickItemType.Money, false);
            }
            

            // 출구로 이동
            while (_customers.Count > 0)
            {
                var c = _customers.Dequeue();
                c.ChangeCustomerState(CustomerStateType.TakeIn);
                c.HideEmoji();
               c.ShowTopVFX(GameManager.Instance.ObjectPoolManager.Spawn<VFX_Emoji>());
                GameManager.Instance.CustomerManager.AddExitQueue(c);
            }
            ChangeTableState(TableStateType.Garbage);
        }
        // 테이블이 쓰레기가 있으면 진행 X
        else if (TableStateType is TableStateType.Garbage) 
        {
            if (_tableTray.GetTrayObjectCount() <= 0 && _tableTray.TrayPickItemType == TrayPickItemType.None)
            {
                ChangeTableState(TableStateType.None);
            }
        }
    }

    public void AddCustomer(CustomerController c)
    {
        c.CurrentWaitingIndex = _tableWayPoints.GetWayPointCount() - 1;
        c.ChangeCustomerState(CustomerStateType.Serving);
        c.SetDestination(_tableWayPoints.GetWayPoint(_tableWayPoints.GetWayPointCount() - 1).position);
        _customers.Enqueue(c);
    }

    public void ChangeTableState(TableStateType type)
    {
        if (TableStateType == type) return;
        TableStateType = type;
    }

    public bool CanUseObject()
    {
        switch (TableStateType)
        {
            case TableStateType.Garbage:
            case TableStateType.Eating:
            case TableStateType.Reserved:
                return false;
            default:
                return true;
        }
    }

    public int ChairCount()
    {
        return _chairs.Length;
    }

    private void OnTableTriggerInteraction(PlayerCharacterController pc)
    {
        // 테이블이 쓰레기를 가지고, 플레이어는 빵을 가진 상태면 X (보류 : 플레이어가 무엇을 들고 있든 청소 진행하기)
        /*
        if (_tableTray.TrayPickItemType != TrayPickItemType.Garbage ||
            pc.ThisTray.TrayPickItemType == TrayPickItemType.Croissant)
        {
            return;
        }
        */
        if (_tableTray.TrayPickItemType != TrayPickItemType.Garbage) return;

            GameObject garbage = _tableTray.RemoveFromTrayObject(false, true);
        if(garbage)  GameManager.Instance.ObjectPoolManager.ReturnPool(garbage.GetComponent<IObjectPoolable>());
        
        // TODO : 넘어진 의자 원상복구
        
        pc.PlaySFXAudio(GameManager.Instance.ResourceManager.GetAudioClipByType(SFXType.Garbage));
        
        if (_cleanVFXRoot)
        {
           var clean =  GameManager.Instance.ObjectPoolManager.Spawn<VFX_Clean>();
           clean.transform.SetParent(_cleanVFXRoot);
           clean.transform.localPosition = Vector3.zero;
        }
    }

    private void OnMoneyTriggerInteraction(PlayerCharacterController pc)
    {
        GameObject money = _moneyTray.RemoveFromTrayObject();
        if (!money) return;
        money.transform.DOMove(pc.transform.position, 0.1f).OnComplete(() =>
        {
            GameManager.Instance.ObjectPoolManager.ReturnPool(money.GetComponent<IObjectPoolable>());
            GameManager.Instance.Money += GameManager.MONEY_VALUE;
        });
    }
}

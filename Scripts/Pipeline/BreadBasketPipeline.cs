using System.Collections.Generic;
using UnityEngine;

public class BreadBasketPipeline : MonoBehaviour
{
    /* 1. 빵 넣는 Trigger
     * 2. 빵 Tray
     * 3. 손님 Queue
     */

    [SerializeField] private int _MIN_BREAD_ORDER_COUNT = 1;
    [SerializeField] private int _MAX_BREAD_ORDER_COUNT = 3;

    private BreadBasketTray _breadBasketTray;
    private TriggerZone _breadBasketTrigger;
    private List<Transform> _waitingPoints;
    private Queue<CustomerController> _waitingQueue;
    private Queue<CustomerController> _readyMoveCounterQueue;

    private int _currentEmptyWaitingPointIndex = 0;
    private float _lastBreadDeliveryTime = 0f;
    [Header("고객에게 빵 전달 딜레이")]
    [SerializeField] private float _breadDeliveryDelay = 0.25f;

    public bool IsBusy => _breadBasketTrigger && _breadBasketTrigger.IsBusy;

    private void Awake()
    {
        _waitingPoints = GetComponentInChildren<WaitingPoints>().GetAllWayPointsList();
        _waitingQueue = new Queue<CustomerController>(_waitingPoints.Count);
        _readyMoveCounterQueue = new Queue<CustomerController>();
        
        _breadBasketTray = GetComponentInChildren<BreadBasketTray>();
        _breadBasketTray.ChangeTrayItemTypeChangeType(TrayPickItemType.Croissant);
        
        _breadBasketTrigger = GetComponentInChildren<TriggerZone>();
        _breadBasketTrigger.OnTriggerEvent -= OnPlayerBreadBasketInteraction;
        _breadBasketTrigger.OnTriggerEvent += OnPlayerBreadBasketInteraction;
    }
    

    private void Update()
    {
        // 빵 픽업 대기 큐가 있을 때만 프로세스 실행
        if (_waitingQueue.Count <= 0) return;
        
        // 주문 프로세스 업데이트
        UpdateOrderProcess();
            
        // 빵 픽업 프로세스 업데이트
        UpdateBreadPickupProcess();
        
        // 계산대 이동 프로세스 업데이트
        UpdateMoveCounter();
    }

    private void OnPlayerBreadBasketInteraction(PlayerCharacterController pc)
    {
        if (!_breadBasketTray.CanAddTrayItem() || pc.ThisTray.TrayPickItemType != _breadBasketTray.TrayPickItemType) return;

        GameObject bread = pc.ThisTray.RemoveFromTrayObject();
        if (!bread) return;
        _breadBasketTray.AddTrayByObject(bread, TrayPickItemType.Croissant);
        pc.PlaySFXAudio(GameManager.Instance.ResourceManager.GetAudioClipByType(SFXType.PutObject));
    }
    
    public void AddWaitingCustomer(CustomerController cs)
    {
        cs.CurrentWaitingIndex = _currentEmptyWaitingPointIndex; 
        cs.ChangeCustomerState(CustomerStateType.Waiting);
        cs.SetDestination(_waitingPoints[_currentEmptyWaitingPointIndex].position);
        _currentEmptyWaitingPointIndex = (_currentEmptyWaitingPointIndex + 1) % _waitingPoints.Count;
        _waitingQueue.Enqueue(cs);
    }

    //  주문 프로세스 
    private void UpdateOrderProcess()
    {
        // 대기 큐가 비어있으면 종료
        if (_waitingQueue.Count == 0)
        {
            return;
        }

        foreach (var customer in _waitingQueue)
        {
            // 손님 이동상태 확인
            if (!customer.HasArrivedDestination())
            {
                continue;
            }

            // 손님이 이미 주문 진행중이면 경우
            if (customer.CustomerStateType != CustomerStateType.PickedBread && customer.OrderCount > 0)
            {
                continue;
            }
        
            // 도착했으면 빵 픽업 갯수 할당
            customer.SetOrderCount(UnityEngine.Random.Range(_MIN_BREAD_ORDER_COUNT, _MAX_BREAD_ORDER_COUNT + 1));
            
           // Debug.Log($"{customer.gameObject.name}에게 주문 할당: {customer.OrderCount}개");
        }
    }

    // 빵 전달 프로세스 
    private void UpdateBreadPickupProcess()
    {
        // 대기 큐가 비어있으면 종료
        if (_waitingQueue.Count == 0)
        {
            return;
        }

        // 플레이어가 빵을 넣고 있으면
        if (_breadBasketTrigger.IsBusy)
        {
            return;
        }
        
        CustomerController customer = _waitingQueue.Peek();
        
        // 손님 이동상태 확인
        if (!customer.HasArrivedDestination())
        {
            return;
        }

        // 주문이 있는 동안 빵 전달
        if (customer.OrderCount > 0)
        {
            _lastBreadDeliveryTime += Time.deltaTime;
            
            // 딜레이 체크
            if (_lastBreadDeliveryTime < _breadDeliveryDelay)
            {
                return; // 아직 딜레이 시간이 지나지 않음
            }
            
            // 빵이 있는지 확인
            GameObject bread = _breadBasketTray.RemoveFromTrayObject(true);
            if (!bread) return;
            customer.ThisTray.AddTrayByObject(bread, TrayPickItemType.Croissant);
            customer.SetOrderCount(Mathf.Max(0, customer.OrderCount - 1));
            customer.PlaySFXAudio(GameManager.Instance.ResourceManager.GetAudioClipByType(SFXType.GetObject));
            
           // Debug.Log($"{customer.gameObject.name}에게 빵 전달. 남은 주문: {customer.OrderCount}개");

            if (customer.CustomerStateType == CustomerStateType.Waiting && customer.OrderCount <= 0)
            {
                // 빵 픽업 완료 
                customer.ChangeCustomerState(CustomerStateType.PickedBread);
             //   Debug.Log($"{customer.gameObject.name} 빵 픽업 완료");
                _readyMoveCounterQueue.Enqueue(customer);
            }
            _lastBreadDeliveryTime = 0f;
        }
    }
    
    private void UpdateMoveCounter()
    {
        if (_readyMoveCounterQueue.Count == 0) return;
        
        // 큐에서 제거
        if (GameManager.Instance.CustomerManager.CheckMoveCounterDeskWaiting(_readyMoveCounterQueue.Peek()))
        {
            _waitingQueue.Dequeue();
            _readyMoveCounterQueue.Dequeue();
            GameManager.Instance.CustomerManager.SpawnCustomer();
        }
    }

    public int GetWaitingQueueCount()
    {
        return _waitingQueue.Count;
    }

    public int GetWaitingPointCount()
    {
        return _waitingPoints.Count;
    }
    
    public bool IsFullWaitingQueue()
    {
        return _waitingPoints.Count == _waitingQueue.Count;
    }
    
}

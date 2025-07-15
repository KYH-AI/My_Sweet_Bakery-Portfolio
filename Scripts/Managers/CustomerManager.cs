using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    [Header("=== 고객 생성 관련 옵션 ===")] 
    [SerializeField] private WaitingPoints spawnPoint;
    [SerializeField] private WaitingPoints takeOutExitPoint;
    [SerializeField] private WaitingPoints takeInExitPoint;
    [SerializeField] private int _minSpawnCustomerValue = 1;
    [SerializeField] private int _maxSpawnCustomerValue = 3;
    [SerializeField] private float _cutomerSpawnTime;
    private WaitForSeconds _customerSpawnSeconds;
    private Coroutine _customerSpawnCoroutine;
    
    [Header("=== 파이프 라인 ===")]
    [SerializeField] private BreadOvenPipeline _zeroPipeline;
    [SerializeField] private BreadBasketPipeline _firstPipeline;
    [SerializeField] private CounterDeskPipeline _secondPieline;
    [SerializeField] private TablePipeline _thirdPipeline;

    public BreadOvenPipeline BreadOven => _zeroPipeline;
    public BreadBasketPipeline BreadBasket => _firstPipeline;
    public CounterDeskPipeline CounterDesk => _secondPieline;
    public TablePipeline Table => _thirdPipeline;
    
    
    private Queue<CustomerController> _entryQueue;
    private Queue<CustomerController> _exitQueue;
    

    public void Init()
    {
        _entryQueue = new Queue<CustomerController>(_maxSpawnCustomerValue);
        _exitQueue = new Queue<CustomerController>();
        _customerSpawnSeconds = new WaitForSeconds(_cutomerSpawnTime);
    }

    private void Start()
    {
        SpawnCustomer();
    }

    private void Update()
    {
        // 입구
        UpdateEntryCustomer();

        // 출구
        UpdateTakeOutExitWaiting();
        UpdateTakeInExitWaiting();
        
        // 처리
        UpdateExitCustomer();
    }

    private void UpdateEntryCustomer()
    {
        if (_entryQueue.Count == 0) return;
        
        while (_entryQueue.Count > 0)
        {
            CustomerController customer = _entryQueue.Peek(); 
            if (!customer.HasArrivedDestination())
            {
                break; 
            }
            _entryQueue.Dequeue();
            _firstPipeline.AddWaitingCustomer(customer);
        }
    }

    private void UpdateTakeOutExitWaiting()
    {
        if (_exitQueue.Count == 0) return;
       
        int index = -1;
        foreach (var customer in _exitQueue)
        {
            if(customer.CustomerStateType is CustomerStateType.TakeIn) continue;

            index++;
            if (!customer.HasArrivedDestination()) continue;
            if (customer.CurrentWaitingIndex > index)
            {
                customer.CurrentWaitingIndex--;
                Vector3 frontWaitingLine = takeOutExitPoint.GetWayPoint(customer.CurrentWaitingIndex).position;
                customer.SetDestination(frontWaitingLine);
            }
        }
    }
    
    private void UpdateTakeInExitWaiting()
    {
        if (_exitQueue.Count == 0) return;
       
        int index = -1;
        foreach (var customer in _exitQueue)
        {
            if(customer.CustomerStateType is CustomerStateType.TakeOut) continue;
            
            index++;
            if (!customer.HasArrivedDestination()) continue;
            if (customer.CurrentWaitingIndex > index)
            {
                customer.CurrentWaitingIndex--;
                Vector3 frontWaitingLine = takeOutExitPoint.GetWayPoint(customer.CurrentWaitingIndex).position;
                customer.SetDestination(frontWaitingLine);
            }
        }
    }

    private void UpdateExitCustomer()
    {
        if (_exitQueue.Count == 0) return;
        
        while (_exitQueue.Count > 0)
        {
            CustomerController customer = _exitQueue.Peek();
            if (customer.CurrentWaitingIndex == 0 && customer.HasArrivedDestination())
            {
                customer = _exitQueue.Dequeue();
        
                if (customer)
                {
                    GameManager.Instance.ObjectPoolManager.ReturnPool(customer);
                }
            }
            else
            {
                break; // 조건 미충족시 대기
            }
        }
    }

    public void AddExitQueue(CustomerController c)
    {
        if (c.CustomerStateType is CustomerStateType.TakeOut)
        {
            c.CurrentWaitingIndex = takeOutExitPoint.GetWayPointCount() - 1;
            c.ChangeCustomerState(CustomerStateType.Waiting);
            c.SetDestination(takeOutExitPoint.GetWayPoint(takeOutExitPoint.GetWayPointCount() -1).position);
        }
        else if (c.CustomerStateType is CustomerStateType.TakeIn)
        {
            c.CurrentWaitingIndex = takeInExitPoint.GetWayPointCount() - 1;
            c.ChangeCustomerState(CustomerStateType.Waiting);
            c.SetDestination(takeInExitPoint.GetWayPoint(takeInExitPoint.GetWayPointCount() -1).position);
        }
        c.PlaySFXAudio(GameManager.Instance.ResourceManager.GetAudioClipByType(SFXType.POS));
        _exitQueue.Enqueue(c);
    }
    
    
    private IEnumerator SpawnCustomerProcess(int count)
    {
        for (int i = 0; i < count; i++)
        { 
            CustomerController customer = GameManager.Instance.ObjectPoolManager.Spawn<CustomerController>();
            customer.transform.SetParent(null);
           customer.transform.position = spawnPoint.GetWayPoint(0).position;
           customer.AgentAIEnable();
           customer.SetDestination(spawnPoint.GetWayPoint(1).position);
            _entryQueue.Enqueue(customer);
            yield return _customerSpawnSeconds;
        }
        _customerSpawnCoroutine = null;
    }

    public void SpawnCustomer()
    {
        if (_firstPipeline.IsFullWaitingQueue())
        {
            return;
        }
        
        int value = UnityEngine.Random.Range(_minSpawnCustomerValue, _maxSpawnCustomerValue + 1);
       
       if (_firstPipeline.GetWaitingQueueCount() + value > _firstPipeline.GetWaitingPointCount())
       {
           return;
       }

       if (_customerSpawnCoroutine != null)
       {
           StopCoroutine(_customerSpawnCoroutine);
           _customerSpawnCoroutine = null;
       }
       _customerSpawnCoroutine = StartCoroutine(SpawnCustomerProcess(value));
    }
    
    
    private CustomerStateType GetCustomerPayType()
    {
        CustomerStateType payType = UnityEngine.Random.Range(0f, 1f) < 0.7f ? CustomerStateType.TakeOut : CustomerStateType.TakeIn;
        return payType;
    }
    
    public bool CheckMoveCounterDeskWaiting(CustomerController c)
    {
        var payType = GetCustomerPayType();
        if (payType is CustomerStateType.TakeOut)
        {
            if (_secondPieline.IsFullTakeInWaitingQueue())
            {
                return false;
            }
        }
        else
        {
            if (_secondPieline.IsFullTakeOutWaitingQueue())
            {
                return false;
            }
        }
        c.ChangeCustomerState(payType);
        _secondPieline.AddWaitingCustomer(c);
        return true;
    }

    public Table FindTable(int count)
    {
        return _thirdPipeline.FindTable(count);
    }
    
    
    /*
     *  1. 손님 스폰
     *    1-1. 스폰 기준은 빵 진열대에 3명 미만인 경우
     *    1-2. 빵 진열대 대기 줄에서 사람이 빠져나 갈 때 1~3명 랜덤으로 선택
     *    1-3. 현재 빵 진열대 대기 수 + 랜덤으로 뽑은 손님 수 <= MAX 빵 진열대 대기 가능인원 아닌 경우에만 손님 생성
     *   
     *  2. 빵 진열대에서 빵을 픽업 후 3~4번 랜덤으로 진행
     * 
     *  3. 카운터 테이크 아웃 
     *   3-1. 테이크 아웃 대기가 Full 이 아니면  바로 이동
     *   3-2. 테이크 아웃 대기가 Full 이면 대기
     *   3-3. 테이크 아웃 대기가 자신의 차례시 Tray에 봉투 생성
     *     - Tray에 봉투가 생기고
     *     - 고객이 들고 있는 빵들을 모두 봉투에 넣는다.
     *   3-4. 봉투를 고객 Tray에 전달하면 돈 Tray에 돈이 생성됨
     *   3-5. 고객 Exit
     * 
     *  4. 카운터 테이크 인
     *   4-1. 테이블 개념이 필요
     *   
     */
}

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CounterDeskPipeline : MonoBehaviour
{
    /* 1. 돈 Tray
     * 2. 돈 Trigger
     * 3. POS Trigger
     * 4. Take Out 손님 Queue
     * 5. Take Out 방 봉투 Tray
     * 6. Take In 손님 Queue
     */

   [SerializeField] private MoneyTray _moneyTray;
   [SerializeField] private ParperBagTray _paperBagTray;
   [SerializeField] private TriggerZone _moneyTrigger;
   [SerializeField] private TriggerZone _posTrigger;

   [SerializeField] private float _moneySpawnTime;
   [SerializeField] private int _moneySpawnPackageCount;
   [SerializeField] private int _moneySpawnRemaining;
   private WaitForSeconds _moneySeconds;
   private WaitUntil _waitUntilHasMoney;
   
   private List<Transform> _takeOutWaitingPoints;
   private Queue<CustomerController> _takeOutWaitingQueue;
   private List<Transform> _takeInWaitingPoints;
   private Queue<CustomerController> _takeInWaitingQueue;

   public MoneyTray MoneyTray => _moneyTray;
   
   private void Start()
   {
       _moneySeconds = new WaitForSeconds(_moneySpawnTime);
       _waitUntilHasMoney = new WaitUntil( () => _moneySpawnRemaining > 0);

       WaitingPoints[] waitingPoints = GetComponentsInChildren<WaitingPoints>();
       if (waitingPoints.Length >= 2)
       {
           _takeOutWaitingPoints = waitingPoints[0].GetAllWayPointsList();
           _takeInWaitingPoints = waitingPoints[1].GetAllWayPointsList();
           _takeOutWaitingQueue = new Queue<CustomerController>(waitingPoints[0].GetWayPointCount());
           _takeInWaitingQueue = new Queue<CustomerController>(waitingPoints[1].GetWayPointCount());
       }
       
       _moneyTrigger.OnTriggerEvent -= OnPlayerMoneyTriggerInteraction;
       _moneyTrigger.OnTriggerEvent += OnPlayerMoneyTriggerInteraction;
       _posTrigger.OnTriggerEvent -= OnPlayerCounterTriggerInteraction;
       _posTrigger.OnTriggerEvent += OnPlayerCounterTriggerInteraction;
       
       StartCoroutine(nameof(SpawnMoneyProcess));
   }

   private void Update()
   {
       UpdateTakeOutWaiting();
       UpdateTakeInWaiting();
   }

   private void UpdateTakeOutWaiting()
   {
       if (_takeOutWaitingQueue.Count == 0) return;
       
       int index = -1;
       foreach (var customer in _takeOutWaitingQueue)
       {
           index++;
           if (!customer.HasArrivedDestination()) continue;
           if (customer.CurrentWaitingIndex > index)
           {
               customer.CurrentWaitingIndex--;
               Vector3 frontWaitingLine = _takeOutWaitingPoints[customer.CurrentWaitingIndex].position;
               customer.SetDestination(frontWaitingLine);
           }
       }
   }

   private void UpdateTakeInWaiting()
   {
       if (_takeInWaitingQueue.Count == 0) return;
       
       int index = -1;
       foreach (var customer in _takeInWaitingQueue)
       {
           index++;
           if (!customer.HasArrivedDestination()) continue;
           if (customer.CurrentWaitingIndex > index)
           {
               customer.CurrentWaitingIndex--;
               Vector3 frontWaitingLine = _takeInWaitingPoints[customer.CurrentWaitingIndex].position;
               customer.SetDestination(frontWaitingLine);
               
               // Take In 가장 첫 번째 줄 경우 이모티콘 변경
               if (customer.CurrentWaitingIndex == 0)
               {
                   customer.ShowEmoji(EmojiType.TakeIn, false);
               }
           }
       }
   }
   
   private IEnumerator SpawnMoneyProcess()
   {
       while (true)
       {
           yield return _waitUntilHasMoney;

           _moneySpawnRemaining--;
           
           IObjectPoolable money = GameManager.Instance.ObjectPoolManager.Spawn<Money>();
           _moneyTray.AddTrayByObject(money.GameObject, TrayPickItemType.Money, false);
           yield return _moneySeconds;
       }
   }

   public void AddWaitingCustomer(CustomerController cs)
   {
       cs.ShowEmoji(EmojiType.Pos, false);
       if (cs.CustomerStateType is CustomerStateType.TakeOut)
       {
           cs.CurrentWaitingIndex = _takeOutWaitingPoints.Count - 1;
           cs.ChangeCustomerState(CustomerStateType.Waiting);
           cs.SetDestination(_takeOutWaitingPoints[_takeOutWaitingPoints.Count - 1].position);
           _takeOutWaitingQueue.Enqueue(cs);
         //  Debug.Log("TakeOut 대기 큐로 이동");
       }
       else if (cs.CustomerStateType is CustomerStateType.TakeIn)
       {
           cs.CurrentWaitingIndex = _takeInWaitingPoints.Count - 1;
           cs.ChangeCustomerState(CustomerStateType.Waiting);
           cs.SetDestination(_takeInWaitingPoints[_takeInWaitingPoints.Count - 1].position);
           _takeInWaitingQueue.Enqueue(cs);
         //  Debug.Log("TakeIn 대기 큐로 이동");
       }
   }
   
    
   public bool IsFullTakeOutWaitingQueue()
   {
       return _takeOutWaitingPoints.Count == _takeOutWaitingQueue.Count;
   }

   public bool IsFullTakeInWaitingQueue()
   {
       return _takeInWaitingPoints.Count == _takeInWaitingQueue.Count;
   }

   private void OnPlayerMoneyTriggerInteraction(PlayerCharacterController pc)
   {
       GameObject money = _moneyTray.RemoveFromTrayObject();
       if (!money) return;
       money.transform.DOMove(pc.transform.position, 0.1f).OnComplete(() =>
       {
           GameManager.Instance.ObjectPoolManager.ReturnPool(money.GetComponent<IObjectPoolable>());
           GameManager.Instance.Money += GameManager.MONEY_VALUE;
       });
   }

   private void OnPlayerCounterTriggerInteraction(PlayerCharacterController pc)
   {
       //  Take Out 웨이팅 확인
       if (_takeOutWaitingQueue.Count != 0)
       {
           var c = _takeOutWaitingQueue.Peek();
           
           // 가장 앞 그리고  줄 도착 확인 
           if (c.CurrentWaitingIndex == 0 && c.HasArrivedDestination())
           {
               if (_takeOutCoroutine == null)
               {
                   _takeOutCoroutine = StartCoroutine(nameof(TakeOutProcess));
               }
           }
       }
       
       //  Take In 웨이팅 확인
       if (_takeInWaitingQueue.Count != 0)
       {
           var c = _takeInWaitingQueue.Peek();
           
           // 가장 앞 그리고  줄 도착 확인 
           if (c.CurrentWaitingIndex == 0 && c.HasArrivedDestination())
           {
               int maxTakeInCount = Mathf.Min(2, _takeInWaitingQueue.Count);
               int takeInOrderCount = UnityEngine.Random.Range(1, maxTakeInCount + 1);

               Table table = GameManager.Instance.CustomerManager.FindTable(takeInOrderCount);
               if (!table) return;

               // 손님 테이블 큐에 넣음 + 의자로 이동 + takeInWaitingQueue 제거
               for (int i = 0; i < takeInOrderCount; i++)
               {
                   var customer = _takeInWaitingQueue.Dequeue();
                   customer.HideEmoji();
                   table.AddCustomer(customer);
               }
               
               // 테이블 상태 변경
               table.ChangeTableState(TableStateType.Reserved);
           }
       }
   }

   private Coroutine _takeOutCoroutine;
   private IEnumerator TakeOutProcess()
   {
       var paperBag = GameManager.Instance.ObjectPoolManager.Spawn<PaperBag>();
       var paperBagComponent = paperBag.GetComponent<PaperBag>();
      _paperBagTray.AddTrayByObject(paperBagComponent.gameObject, TrayPickItemType.PaperBag, false);

      yield return new WaitForSeconds(1f);
                   
       var customer = _takeOutWaitingQueue.Peek();
       int count = customer.ThisTray.GetTrayObjectCount();
       for (int i = 0; i < count; i++)
       {
           GameObject bread = customer.ThisTray.RemoveFromTrayObject();
           if(!bread) continue;
           paperBagComponent.AddBread(bread.GetComponent<IObjectPoolable>());
           yield return new WaitForSeconds(0.5f);
       }
       paperBagComponent.PaperBagCloseAnimation();
       yield return new WaitForSeconds(1f);
       GameObject paperBagGameObject = _paperBagTray.RemoveFromTrayObject();
       customer.ThisTray.AddTrayByObject(paperBagGameObject, TrayPickItemType.PaperBag, true, true);
       
       yield return new WaitForSeconds(1f);
       customer.HideEmoji();
       customer.ShowTopVFX(GameManager.Instance.ObjectPoolManager.Spawn<VFX_Emoji>());
       _moneySpawnRemaining += _moneySpawnPackageCount;
       _takeOutWaitingQueue.Dequeue();
       
       customer.ChangeCustomerState(CustomerStateType.TakeOut);
       // TakeOut 출구로 이동
       GameManager.Instance.CustomerManager.AddExitQueue(customer);
       
       _takeOutCoroutine = null;
   }
}


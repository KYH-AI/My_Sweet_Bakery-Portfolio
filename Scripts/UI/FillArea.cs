using System;
using UnityEngine;
using TMPro;

public class FillArea : UIBase
{
    [SerializeField] private TextMeshProUGUI _moneyText;
    [SerializeField] private float _interval;
    
    private PlayerCharacterController _pc;

    // 돈 모든 충전 시 업그레이드 이벤트 
    public event Action OnUpgradeEvent = null;
    // 누적된 돈
    public long SpentMoney { get; private set; }
    // 업그레이드 총 비용
    public long RequestMoney { get; set; }
    // 업그레이드 남은 비용
    public long RemainingMoney => RequestMoney - SpentMoney; 

    private void Start()
    {
        var moneyTrigger = GetComponentInChildren<TriggerZone>();
        if (moneyTrigger)
        {
            moneyTrigger.OnTriggerEvent -= OnMoneyTriggerInteraction;
            moneyTrigger.OnTriggerEvent += OnMoneyTriggerInteraction;
        }

        UpdateUI();
    }
    
    private void OnDisable()
    {
        OnUpgradeEvent = null;
    }

    private void OnDestroy()
    {
        OnUpgradeEvent = null;
    }

    private void OnMoneyTriggerInteraction(PlayerCharacterController pc)
    {
        if (GameManager.Instance.Money <= 0) return;
        
        long money = (long)(RequestMoney * _interval);
        
        if (GameManager.Instance.Money < money) return;

        GameManager.Instance.Money -= money;
        SpentMoney += money;
        if (SpentMoney >= RequestMoney)
        {
            SpentMoney = RequestMoney;
            pc.PlaySFXAudio(GameManager.Instance.ResourceManager.GetAudioClipByType(SFXType.Success));
            OnUpgradeEvent?.Invoke();
          //  Debug.Log("돈 충전 완료");
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        _moneyText.text = RemainingMoney.ToString();
    }
}

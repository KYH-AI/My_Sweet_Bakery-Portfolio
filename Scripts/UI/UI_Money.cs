using TMPro;
using UnityEngine;

public class UI_Money : UIBase
{
   [SerializeField] private TextMeshProUGUI _moneyText;

   public override void Init()
   {
       base.Init();
       UpdateMoneyText();
       GameManager.Instance.EventManager.AddEvent(EventType.Money, UpdateMoneyText);
   }
   
   public override void Hide()
   {
       base.Hide();
       GameManager.Instance.EventManager.RemoveEvent(EventType.Money, UpdateMoneyText);
   }
   
   public override void Show()
   {
       base.Show();
       GameManager.Instance.EventManager.AddEvent(EventType.Money, UpdateMoneyText);
   }

   private void OnDestroy()
   {
       GameManager.Instance.EventManager.RemoveEvent(EventType.Money, UpdateMoneyText);
   }

   private void UpdateMoneyText()
   {
      _moneyText.text = MoneyFormatter.FormatMoney(GameManager.Instance.Money);
   }
}

using System.Collections;
using UnityEngine;

public class BreadOvenPipeline : MonoBehaviour
{
    [SerializeField] private float _breadBakeTime;
    [SerializeField] private Transform _breadOvenRoot;
    
    private BreadOvenTray _breadOvenTray;
    private TriggerZone _breadOvenTrigger;
    private WaitForSeconds _seconds;
    private WaitUntil _waitUntil;

    public bool IsBusy => _breadOvenTrigger && _breadOvenTrigger.IsBusy;

    private void Start()
    {
        _breadOvenTray = GetComponentInChildren<BreadOvenTray>();
        _breadOvenTray.ChangeTrayItemTypeChangeType(TrayPickItemType.Croissant);

        _waitUntil = new WaitUntil(_breadOvenTray.CanAddTrayItem);
        _seconds = new WaitForSeconds(_breadBakeTime);
        
        _breadOvenTrigger = GetComponentInChildren<TriggerZone>();
        _breadOvenTrigger.OnTriggerEvent -= OnBreadOvenInteraction;
        _breadOvenTrigger.OnTriggerEvent += OnBreadOvenInteraction;

        StartCoroutine(nameof(BakeBreadSpawnProcess));
    }

    private IEnumerator BakeBreadSpawnProcess()
    {
        while (true)
        {
            yield return _waitUntil;
            IObjectPoolable bread = GameManager.Instance.ObjectPoolManager.Spawn<Croissant>();
            bread.Transform.position = _breadOvenRoot.position;
            _breadOvenTray.AddTrayByObject(bread.GameObject, TrayPickItemType.Croissant);
            yield return _seconds;
        }
    }

    private void OnBreadOvenInteraction(BaseCharacter character)
    {
        if (character.ThisTray.TrayPickItemType is TrayPickItemType.None or TrayPickItemType.Croissant)
        {
            if (!character.ThisTray.CanAddTrayItem()) return;
            
            GameObject bread = _breadOvenTray.RemoveFromTrayObject();
            if (!bread) return;
            character.ThisTray.AddTrayByObject(bread, TrayPickItemType.Croissant);
            character.PlaySFXAudio(GameManager.Instance.ResourceManager.GetAudioClipByType(SFXType.GetObject));
        }
    }
}

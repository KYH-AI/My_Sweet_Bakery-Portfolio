using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TriggerZone : MonoBehaviour, IPlayerSettable
{
    [SerializeField] private GameObject _triggerZoneShadow;
    [SerializeField] private float _interval = 0.25f;
    private Vector3 _triggerZoneShadowOriginalScale;
    private PlayerCharacterController _pc;
    private Coroutine _triggerCoroutine = null;
    private WaitForSeconds _seconds;
    
    public bool IsBusy => _triggerCoroutine != null;
    public event Action<PlayerCharacterController> OnTriggerEvent;

    private void Awake()
    {
        _seconds = new WaitForSeconds(_interval);
        GetComponent<Collider>().isTrigger = true;
        if(_triggerZoneShadow) _triggerZoneShadowOriginalScale = _triggerZoneShadow.transform.localScale;
    }

    private IEnumerator TriggerProcess()
    {
        while (true)
        {
            yield return _seconds;
            if (_pc)
            {
                OnTriggerEvent?.Invoke(_pc);
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags._PLAYER_TAG))
        {
            if (_triggerZoneShadow)
            {
                _triggerZoneShadow.transform
                    .DOScale(_triggerZoneShadowOriginalScale + new Vector3(0.5f, 0.5f, 0f), 0.25f);
            }
            
            Connect(other.GetComponent<PlayerCharacterController>());
            if (_triggerCoroutine != null)
            {
                StopCoroutine(_triggerCoroutine);
                _triggerCoroutine = null;
            }
            _triggerCoroutine = StartCoroutine(nameof(TriggerProcess));
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tags._PLAYER_TAG))
        {
            if (_triggerZoneShadow)
            {
                _triggerZoneShadow.transform
                    .DOScale(_triggerZoneShadowOriginalScale, 0.25f);
            }
            
            if (_triggerCoroutine != null)
            {
                StopCoroutine(_triggerCoroutine);
                _triggerCoroutine = null;
            }
            Disconnect();
        }
    }
    
    public void Connect(BaseCharacter player)
    {
        if (player == null) return;
        _pc = player as PlayerCharacterController;
    }

    public void Disconnect()
    {
        _pc = null;
    }

    private void OnDisable()
    {
        OnTriggerEvent = null;
    }

    private void OnDestroy()
    {
        OnTriggerEvent = null;
    }
}

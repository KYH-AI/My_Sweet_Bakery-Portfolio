using UnityEngine;
using UnityEngine.AI;

public class CustomerController : BaseCharacter, IObjectPoolable
{
    private NavMeshAgent _agent;
    [SerializeField] private Transform _vfxTopRoot;
    [SerializeField] private UI_SpeechBubble _uiSpeechBubble;
    private static int _Sitting_Talking = Animator.StringToHash("Sitting_Talking");
    public CustomerStateType CustomerStateType { get; private set; } = CustomerStateType.None;
    public int OrderCount { get; private set; } = 0;
    public int CurrentWaitingIndex { get; set; } = 0;
    public GameObject GameObject => _thisGameObejct;
    public Transform Transform => _thisTransform;

    [Header("Agent Settings")]
    [SerializeField] private float _stoppingDistance = 0.1f;

    
    protected override void Awake()
    {
        base.Awake();
        if (!_agent)
        {
            _agent = GetComponent<NavMeshAgent>();
        }
        if (!_uiSpeechBubble)
        {
            _uiSpeechBubble = GetComponentInChildren<UI_SpeechBubble>();
        }
        
        _agent.speed = _moveSpeed;
        _agent.angularSpeed = _rotateSpeed;
        _agent.stoppingDistance = _stoppingDistance;
    }

    [SerializeField] private int _poolSize = 10;
    public int PoolSize => _poolSize;

    public PoolRootType GetRootType()
    {
        return PoolRootType.Character;
    }

    public void OnSpawn()
    {
        ChangeCustomerState(CustomerStateType.None);
        ChangeMoveState(CharacterMoveStateType.None);
        OrderCount = 0;
        CurrentWaitingIndex = 0;
        _uiSpeechBubble.gameObject.SetActive(false);
        AnimationHandle();
        this.gameObject.SetActive(true);
        // !! AI Agent 컴포넌트는 풀링 Root에서 특정 위치로 이동한 후  활성화 하기 !!!
    }

    public void OnDespawn()
    {
        _agent.enabled = false;
        ChangeCustomerState(CustomerStateType.None);
        ChangeMoveState(CharacterMoveStateType.None);
        
        // Tray 확인
        var t = ThisTray.RemoveFromTrayObject(false, true);
        if(t) GameManager.Instance.ObjectPoolManager.ReturnPool(t.GetComponent<IObjectPoolable>());
        
        // VFX TOP 확인 - child를 모두 풀로 반환
        while (_vfxTopRoot.childCount > 0)
        {
            Transform child = _vfxTopRoot.GetChild(0);
            if (child.TryGetComponent(out IObjectPoolable poolable))
            {
                GameManager.Instance.ObjectPoolManager.ReturnPool(poolable);
            }
            else
            {
                Destroy(child.gameObject); // 풀링 대상이 아니면 파괴
            }
        }
        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        Move();
        Rotate();
        Gravity();
    }

    protected override void Move()
    {
        if (HasArrivedDestination())
        {
            _agent.isStopped = true;
            ChangeMoveState(CharacterMoveStateType.Idle);
        }
        else
        {
            ChangeMoveState(CharacterMoveStateType.Move);
        }
    }

    protected override void Rotate()
    {
        Vector3 moveDir = (GetDestination() - this.transform.position).normalized;
        if (moveDir != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(moveDir);
            this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, lookRotation, Time.deltaTime * _rotateSpeed);
        }
    }

    protected override void AnimationHandle()
    {
        base.AnimationHandle();
        switch (CustomerStateType)
        {
            case CustomerStateType.Eating : 
                PlayAnimation(_Sitting_Talking, 0.01f);
                break;
        }
    }

    public void ChangeCustomerState(CustomerStateType type)
    {
        if (CustomerStateType == type) return;
        CustomerStateType = type;
    }

    public void SetDestination(Vector3 dest)
    {
        _agent.SetDestination(dest);
        _agent.isStopped = false;
    }

    public void StopAgent()
    {
        _agent.isStopped = true;
        _agent.ResetPath();
    }

    public void AgentAIEnable()
    {
        _agent.enabled = true;
    }
    public void AgentAIDisable()
    {
        _agent.enabled = false;
    }

    public Vector3 GetDestination()
    {
        return _agent.destination;
    }

    public bool HasArrivedDestination()
    {
        return Vector3.Distance(GetDestination(), _thisTransform.position) < _stoppingDistance;
        /*
            Vector3 dir = GetDestination() - _thisTransform.position;
            return dir.sqrMagnitude <= _stoppingDistance;
        */
    }

    public void SetOrderCount(int count)
    {
        OrderCount = count;
     
        if (OrderCount > 0)
        {
            if (!_uiSpeechBubble.gameObject.activeSelf)
            {
                ShowEmoji(EmojiType.Bread, true);
            }
            _uiSpeechBubble.Count = OrderCount;
        }
        else
        {
            HideEmoji();
        }
    }

    public void ShowEmoji(EmojiType emojiType, bool useText, string str = "")
    {
        if (_uiSpeechBubble.CurrentEmoji == emojiType) return;
        
        Sprite emoji = GameManager.Instance.ResourceManager.GetEmojiByType(emojiType);
        _uiSpeechBubble.SetRequestImage(emoji, emojiType);
        if (useText && !str.Equals(""))
        {
            _uiSpeechBubble.SetRequestText(str);
        }
        else
        {
            _uiSpeechBubble.HideRequestText();
        }

        if (!_uiSpeechBubble.gameObject.activeSelf)
        {
            _uiSpeechBubble.gameObject.SetActive(true);
        }
    }
    
    public void HideEmoji()
    {
        _uiSpeechBubble.gameObject.SetActive(false);
        _uiSpeechBubble.HideRequestImage();
        _uiSpeechBubble.HideRequestText();
    }

    public void ShowTopVFX(IObjectPoolable vfx)
    {
        if (_vfxTopRoot)
        {
            vfx.Transform.SetParent(_vfxTopRoot);
            vfx.Transform.localPosition = Vector3.zero;
        }
    }

}

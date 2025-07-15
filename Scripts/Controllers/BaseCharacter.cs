using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CharacterController))]
public abstract class BaseCharacter : MonoBehaviour
{
    [SerializeField] protected AudioSource _thisAudioSource;
    [SerializeField] protected CharacterController _thisCharacterController;
    [SerializeField] protected Animator _thisAnimator;
    [SerializeField] protected GameObject _thisGameObejct;
    [SerializeField] protected Transform _thisTransform;
    public CharacterTray ThisTray { get; protected set; }
    
    [Header("=== Animation Node Key Hash ===")]
    protected static int _DEFAULT_IDLE = Animator.StringToHash("Default_Idle");
    protected static int _DEFAULT_MOVE = Animator.StringToHash("Default_Move");
    protected static int _STACK_IDLE = Animator.StringToHash("Stack_Idle");
    protected static int _STACK_MOVE = Animator.StringToHash("Stack_Move");

    public CharacterMoveStateType CharacterMoveStateType { get; protected set; } = CharacterMoveStateType.Idle;
    public CharacterStackType CharacterStackType { get; protected set; } = CharacterStackType.Idle;
    public TrayPickItemType PickItemType { get; protected set; } = TrayPickItemType.None;
    
    [Range(1f, 5f)]
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected float _rotateSpeed;
    [SerializeField] protected float _grvaity = -10f;
    
    protected float _verticalVelocity = 0f;
    
    protected virtual void Awake()
    {
        if (!_thisAudioSource)
        {
            _thisAudioSource = GetComponent<AudioSource>();
        }
        
        if (!_thisCharacterController)
        {
            _thisCharacterController = GetComponent<CharacterController>();
        }

        if (!_thisAnimator)
        {
            DebugLog.CustomLog($"{this.gameObject.name}에 애니메이터가 없습니다!", Color.red);
        }

        if (!_thisGameObejct)
        {
            _thisGameObejct = this.gameObject;
        }

        if (!_thisTransform)
        {
            _thisTransform = this.transform;
        }

        ThisTray = GetComponentInChildren<CharacterTray>();
        ThisTray.Connect(this);
    }

    public void ChangeMoveState(CharacterMoveStateType type)
    {
        if (CharacterMoveStateType == type) return;
        CharacterMoveStateType = type;
        AnimationHandle();
    }

    public void ChangeStackState(CharacterStackType type)
    {
        if (CharacterStackType == type) return;
        CharacterStackType = type;
        AnimationHandle();
    }

    public void PlaySFXAudio(AudioClip clip, bool isOneShot = true, float volume = 1f)
    {
        if (!clip)
        {
            return;
        }

        _thisAudioSource.volume = volume;
        if (isOneShot)
        {
            _thisAudioSource.PlayOneShot(clip);
        }
        else
        {
            _thisAudioSource.clip = clip;
            _thisAudioSource.Play();
        }
    }

    protected virtual void AnimationHandle()
    {
        switch (CharacterMoveStateType)
        {
            case CharacterMoveStateType.Idle :
                if (CharacterStackType is CharacterStackType.Idle)
                {
                    PlayAnimation(_DEFAULT_IDLE, 0.01f);
                }
                else
                {
                    PlayAnimation(_STACK_IDLE, 0.01f);
                }
                break;
            case CharacterMoveStateType.Move : 
                if (CharacterStackType is CharacterStackType.Idle)
                {
                    PlayAnimation(_DEFAULT_MOVE, 0.01f);
                }
                else
                {
                    PlayAnimation(_STACK_MOVE, 0.01f);
                }
                break;
        }
    }
    
    protected virtual void PlayAnimation(int hash, float fadeTime)
    {
        _thisAnimator.CrossFade(hash, fadeTime);
    }

    protected abstract void Move();
    protected abstract void Rotate();
    
    protected void Gravity()
    {
        if (_thisCharacterController.isGrounded && _verticalVelocity < 0)
        {
            _verticalVelocity = 0f; // 땅에 닿으면 Y 속도 초기화
        }
        else
        {
            _verticalVelocity += _grvaity * Time.deltaTime; // 중력 적용
        }
    }
}

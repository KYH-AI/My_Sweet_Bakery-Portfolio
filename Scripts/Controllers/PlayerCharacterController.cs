using UnityEngine;

public class PlayerCharacterController : BaseCharacter
{
    private static PlayerCharacterController _instance;
    public static PlayerCharacterController Instance 
    {
        get
        {
            if (_instance == null)
            {
                // Scene에서 기존 인스턴스 찾기
                _instance = FindObjectOfType<PlayerCharacterController>();
            }
            return _instance;
        }
        private set
        {
            _instance = value;
        }
    }
    
    private PlayerMoneyAudioSource _moneyAudioSource;
    private TutorialArrowController _tutorialArrowController;
    private Vector3 _moveDir;

    protected override void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        base.Awake();
        _tutorialArrowController = GetComponentInChildren<TutorialArrowController>();
        if (_tutorialArrowController) _tutorialArrowController.gameObject.SetActive(false);
        _moneyAudioSource = GetComponentInChildren<PlayerMoneyAudioSource>();
    }

    private void Start()
    {
        GameManager.Instance.EventManager.AddEvent(EventType.Money, MoneyPlayAudioSource);
    }

    private void Update()
    {
        Move();
        Rotate();
        Gravity();
    }

    protected override void Move()
    {
        Vector2 dir = GameManager.Instance.InputManager.JoystickDir;
        _moveDir = new Vector3(dir.x, 0f, dir.y);
        _moveDir = (Quaternion.Euler(0, 45, 0) * _moveDir).normalized;

        // 수평 이동 + 수직 이동 결합
        Vector3 finalMove = _moveDir * (Time.deltaTime * _moveSpeed);
        finalMove.y = _verticalVelocity * Time.deltaTime;

        _thisCharacterController.Move(finalMove);

        if (_moveDir != Vector3.zero)
        {
            ChangeMoveState(CharacterMoveStateType.Move);
        }
        else
        {
            ChangeMoveState(CharacterMoveStateType.Idle);
        }

        /*
        if (_moveDir != Vector3.zero)
        {
            _thisCharacterController.Move(finalMove);
        }
        */
    }

    protected override void Rotate()
    {
        if (_moveDir != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(_moveDir);
            _thisTransform.rotation = Quaternion.RotateTowards(_thisTransform.rotation, 
                                                                lookRotation, 
                                                                Time.deltaTime * _rotateSpeed);
        }
    }

    public void SetTutorialTarget(Transform target)
    {
        if (!_tutorialArrowController.gameObject.activeSelf)
        {
            _tutorialArrowController.gameObject.SetActive(true);
        }
        _tutorialArrowController.SetTarget(target);
    }

    public void MoneyPlayAudioSource()
    {
        _moneyAudioSource.PlayCoinSound();
    }
    
    private void OnDestroy()
    {
        GameManager.Instance.EventManager.RemoveEvent(EventType.Money, MoneyPlayAudioSource);
        _instance = null;
    }
}

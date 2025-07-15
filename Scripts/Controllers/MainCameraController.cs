using DG.Tweening;
using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    [Header("Player Transform Target")]
    [SerializeField] private Transform _target;

    [Header("Camera offset")] 
    private Vector3 _offSet;
    
    public static Camera MainCamera { get; private set; }
    public static bool AutoAim { get; set; } = true;
    public static Transform MainCameraTransform => MainCamera.transform;
    

    public void Init()
    {
        MainCamera = Camera.main;
        _offSet = this.transform.position - _target.position;
    }

    private void LateUpdate()
    {
        if (!AutoAim) return;
        this.transform.position = _offSet + _target.position;
    }

    public void LookAtTarget(Vector3 pos, float moveDuration = 1.5f, float intervalDuration = 2f)
    {
        LockCamera();
        Sequence seq = DOTween.Sequence();
        seq.Append(this.transform.DOMove(pos + _offSet, moveDuration));  // 이동
        seq.AppendInterval(intervalDuration);                                   // 3초 대기
        seq.Append(this.transform.DOMove(_target.position + _offSet, moveDuration)); // 복귀
        seq.OnComplete(UnLockCamera);
    }
    
    private void LockCamera()
    {
        AutoAim = false;
        GameManager.Instance.UIManager.BlockUI<UI_Joystick>();
    }

    private void UnLockCamera()
    {
        AutoAim = true;
        GameManager.Instance.UIManager.UnblockUI<UI_Joystick>();
    }
}

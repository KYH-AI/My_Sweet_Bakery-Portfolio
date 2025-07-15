using UnityEngine;

public class TutorialArrowController : MonoBehaviour
{
    [Header("방향 설정")] 
    [SerializeField] private SpriteRenderer _arrowSprite;
    [SerializeField] private  Transform _centerObject;      // A 오브젝트 (중심)
    [ReadOnly][SerializeField]  private Transform _targetObject;      // C 오브젝트 (목표)
    [SerializeField] private float _moveSpeed = 1000f;       // 원 위에서 이동 속도 (도/초)
    [SerializeField] private float _minTargetDistance = 1f; // 목표의 최소 거리 
    
    private float _radius;               // A와 B 사이의 고정 반지름
    private Transform _previousTarget;   // 이전 목표 추적용
    
     private void Awake()
    {
        // 시작할 때 A와 B 사이의 거리를 반지름으로 저장
        if (_centerObject != null)
        {
            _radius = Vector3.Distance(transform.position, _centerObject.position);
        }
    }
    
    void Update()
    {
        if (!_centerObject || !_targetObject) return;
        
        // 화살표 비활성화 버전
        /*
        if (CheckDistance())
        {
            return;
        }
        */
        
        // 목적지 변경 시 즉시 화살표 변경
        CheckTargetChange();
        
        // 반지름 유지하면서 목표 방향으로 위치 이동
        UpdatePosition();
        
        // 나침반처럼 화살표는 항상 목표를 바라봄
        UpdateRotation();
        
        // 이전 목표 업데이트
        _previousTarget = _targetObject;
    }

    private bool CheckDistance()
    {
        // 목표와 중심 사이의 거리 확인
        float distanceToCenter = Vector3.Distance(_targetObject.position, _centerObject.position);
        
        if (distanceToCenter < _minTargetDistance)
        {
            // 목표가 너무 가까우면 화살표 숨기기
            _arrowSprite.enabled = false;
            return true;
        }
   
         // 목표가 충분히 멀면 화살표 보이기
        _arrowSprite.enabled = true;
        return false;
    }
    
    private void UpdatePosition()
    {
        // 현재 B의 각도 계산 (A 기준)
        Vector3 currentOffset = transform.position - _centerObject.position;
        float currentAngle = Mathf.Atan2(currentOffset.x, currentOffset.z) * Mathf.Rad2Deg;
        
        // C가 있는 방향의 각도 계산 (A 기준)
        Vector3 targetOffset = _targetObject.position - _centerObject.position;
        float targetAngle = Mathf.Atan2(targetOffset.x, targetOffset.z) * Mathf.Rad2Deg;
        
        // 가장 가까운 방향으로 회전
        float angleDifference = Mathf.DeltaAngle(currentAngle, targetAngle);
        float angleStep = _moveSpeed * Time.deltaTime;
        
        // 목표 각도에 가까우면 정확히 맞춤
        if (Mathf.Abs(angleDifference) < angleStep)
        {
            currentAngle = targetAngle;
        }
        else
        {
            // 짧은 방향으로 회전
            currentAngle += Mathf.Sign(angleDifference) * angleStep;
        }
        
        // 새로운 위치 계산 (원 위의 점, 반지름 유지)
        float radian = currentAngle * Mathf.Deg2Rad;
        Vector3 newPosition = _centerObject.position + new Vector3(
            Mathf.Sin(radian) * _radius,
            currentOffset.y, // Y축 높이 유지
            Mathf.Cos(radian) * _radius
        );
        
        transform.position = newPosition;
    }
    
    private void UpdateRotation()
    {
        Vector3 lookDirection = _targetObject.position - _centerObject.position;
        if (lookDirection != Vector3.zero)
        {
            float yRotation = Mathf.Atan2(lookDirection.x, lookDirection.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(90f, yRotation, 0f);
        }
    }
    
    private void CheckTargetChange()
    {
        // 목표가 변경되었는지 확인
        if (_previousTarget != _targetObject)
        {
            // 새로운 목표로 즉시 회전 시작
            if (!_targetObject)
            {
                // 즉시 새로운 목표 방향으로 위치 이동
                Vector3 targetOffset = _targetObject.position - _centerObject.position;
                float targetAngle = Mathf.Atan2(targetOffset.x, targetOffset.z) * Mathf.Rad2Deg;
                
                // 새로운 위치 계산 (즉시 목표 방향으로 이동)
                float radian = targetAngle * Mathf.Deg2Rad;
                Vector3 currentOffset = transform.position - _centerObject.position;
                Vector3 newPosition = _centerObject.position + new Vector3(
                    Mathf.Sin(radian) * _radius,
                    currentOffset.y, // Y축 높이 유지
                    Mathf.Cos(radian) * _radius
                );
                transform.position = newPosition;
                
                // 즉시 새로운 목표를 바라보도록 회전
                Vector3 lookDirection = _targetObject.position - transform.position;
                if (lookDirection != Vector3.zero)
                {
                    float yRotation = Mathf.Atan2(lookDirection.x, lookDirection.z) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(90f, yRotation, 0f);
                }
            }
        }
    }
    
    // 외부에서 목표를 변경할 수 있는 공용 메서드
    public void SetTarget(Transform newTarget)
    {
        _targetObject = newTarget;
        if (_targetObject == null)
        {
            this.gameObject.SetActive(false);
        }
    }
}

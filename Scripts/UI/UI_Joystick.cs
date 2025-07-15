using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Joystick : UIBase, IPointerUpHandler, IPointerDownHandler, IDragHandler
{
    [Header("튜토리얼 이미지")] 
    [SerializeField] private GameObject _tutorialJoyStickImage;
    
    [Header("Joystick Settings")]
    [SerializeField] private GameObject _joyStickBackGround;
    [SerializeField] private GameObject _joyStickMain;
    [SerializeField] private float _joyStickRaidus = 5.0f;
    
    private Vector2 _touch = Vector2.zero;

    // 캐싱된 피봇 오프셋
    private Vector2 _bgPivotOffset;
    private Vector2 _mainPivotOffset;
    
    public void Start()
    {
        _joyStickRaidus = _joyStickBackGround.GetComponent<RectTransform>().sizeDelta.y / 3;
        CalculatePivotOffsets();
    }
    
    private void CalculatePivotOffsets()
    {
        RectTransform bgRect = _joyStickBackGround.GetComponent<RectTransform>();
        RectTransform mainRect = _joyStickMain.GetComponent<RectTransform>();
        
        // 피봇 오프셋 계산 (중심으로 보정하기 위해 더할 값)
        _bgPivotOffset = new Vector2(
            (0.5f - bgRect.pivot.x) * bgRect.sizeDelta.x,     
            (0.5f - bgRect.pivot.y) * bgRect.sizeDelta.y     
        );
    
        _mainPivotOffset = new Vector2(
            (0.5f - mainRect.pivot.x) * mainRect.sizeDelta.x,
            (0.5f - mainRect.pivot.y) * mainRect.sizeDelta.y
        );
        
      //  Debug.Log($"BG Pivot Offset: {_bgPivotOffset}");
      //  Debug.Log($"Main Pivot Offset: {_mainPivotOffset}");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
        if (_tutorialJoyStickImage && _tutorialJoyStickImage.activeSelf)
        {
            _tutorialJoyStickImage.SetActive(false);
            _tutorialJoyStickImage = null;
        }
        
        _joyStickBackGround.transform.position = eventData.position - _bgPivotOffset;
        _joyStickMain.transform.position = eventData.position - _mainPivotOffset;
        _touch = eventData.position;
        Show();
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        // 1. 방향 벡터  계산
        Vector2 dir = eventData.position - _touch;
       
        // 2. 거리 계산 및 제한
        float moveDistance = Mathf.Min(dir.magnitude, _joyStickRaidus);
        
        // 3. 방향 벡터 정규화 
        Vector2 moveDir = dir.normalized;
        
        // 4. UI 최종 이동 위치 계산
       Vector2 movePosition = _touch + moveDir * moveDistance;
       _joyStickMain.transform.position = movePosition - _mainPivotOffset;

        GameManager.Instance.InputManager.JoystickDir = moveDir;
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        _joyStickMain.transform.position = _touch - _mainPivotOffset;
        GameManager.Instance.InputManager.JoystickDir = Vector2.zero;
        Hide();
    }
}

using UnityEngine;

public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{

    private static T _Instacne;
    private static readonly object _Lock = new object();
    

    /// <summary>
    /// 이 싱글턴이 씬 전환 시 파괴되지 않아야 하면 true,  
    /// 그렇지 않고 “씬 내에서만 존재”해야 하면 false를 반환하도록 서브클래스가 구현
    /// </summary>
    protected abstract bool IsPersistent { get; }
    
    /// <summary>
    /// 싱글턴 인스턴스 반환
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_Instacne != null) return _Instacne;

            lock (_Lock)
            {
                if (_Instacne != null) return _Instacne;
                
                // 2) 씬에 붙어 있는 기존 인스턴스 검색
                _Instacne = FindObjectOfType<T>();
                if (_Instacne != null) return _Instacne;

                // 3) 씬에도 없으면 새로 생성
                var go = new GameObject(typeof(T).Name + " (Singleton)");
                _Instacne = go.AddComponent<T>();
                // Awake() 안에서 IsPersistent를 보고 DontDestroyOnLoad 처리됨
                return _Instacne;
            }
        }
    }

    
    /// <summary>
    /// Awake 단계에서 싱글톤 인스턴스 설정
    /// </summary>
    private void Awake()
    {
        if (_Instacne == null)
        {
            _Instacne = this as T;
            if(IsPersistent) DontDestroyOnLoad(gameObject);
            OnSingletonAwake();
        }
        else if (_Instacne != this)
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// OnDestroy 시점에 자신이 등록된 싱글턴이라면 _instance를 null로 만듭니다.
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (_Instacne == this)
            _Instacne = null;
    }
    
    /// <summary>
    /// 싱글턴이 Awake 직후에 추가 초기화가 필요하면 서브클래스에서 override
    /// </summary>
    protected virtual void OnSingletonAwake() { }
    
}

using System;
using UnityEngine;

[DefaultExecutionOrder(-10000)]
public class GameManager : MonoBehaviourSingleton<GameManager>
{
    protected override bool IsPersistent { get; } = false;
    public InputManager InputManager { get; private set; }
    public MainCameraController MainCamera { get; private set; }
    public ObjectPoolManager ObjectPoolManager { get; private set; }
    public ResourceManager ResourceManager { get; private set; }
    public CustomerManager CustomerManager { get; private set; }
    public UIManager UIManager { get; private set;  }
    public EventManager EventManager { get; private set; }

    private long _money = 0;
    public long Money
    {
        get => _money;
        set
        {
            _money = Math.Max(0, value);
            EventManager?.CallBackEvent(EventType.Money);
        }
    }
    public const int MONEY_VALUE = 10;
    
    protected override void OnSingletonAwake()
    {
        this.InputManager = new InputManager();
        if (Camera.main != null) this.MainCamera = Camera.main.GetComponent<MainCameraController>();
        this.ObjectPoolManager = GetComponentInChildren<ObjectPoolManager>();
        this.ResourceManager = GetComponentInChildren<ResourceManager>();
        this.CustomerManager = GetComponentInChildren<CustomerManager>();
        this.UIManager = GetComponentInChildren<UIManager>();
        this.EventManager = new EventManager();
        
        // 데이터
        ObjectPoolManager.Init();
        UIManager.Init();
        ResourceManager.Init();
        
        // 콘텐츠
        MainCamera.Init();
        CustomerManager.Init();
    }
}

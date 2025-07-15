using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Transform _uiRoot;
    private Dictionary<Type, UIBase> _allUiList;
    
    public Canvas OverlayCanvas { get; private set; }
    public RectTransform OverlayCanvasRectTransform { get; private set; }
    
    public void Init()
    {
        OverlayCanvas = _uiRoot.GetComponent<Canvas>();
        OverlayCanvasRectTransform = OverlayCanvas.GetComponent<RectTransform>();
        
        _allUiList = new Dictionary<Type, UIBase>(_uiRoot.childCount);
        foreach (Transform ui in _uiRoot)
        {
            if (ui.TryGetComponent(out UIBase uiBase))
            {
                _allUiList.Add(uiBase.GetType(), uiBase);
                uiBase.Init();
            }
        }
    }

    public void BlockUI<T>() where T : UIBase
    {
        Type type = typeof(T);
        if (_allUiList.ContainsKey(type))
        {
            _allUiList[type].BlockControl();
        }
    }

    public void UnblockUI<T>() where T : UIBase
    {
        Type type = typeof(T);
        if (_allUiList.ContainsKey(type))
        {
            _allUiList[type].UnlockControl();
        }
    }
}

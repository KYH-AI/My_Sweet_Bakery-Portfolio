using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    private CanvasGroup _thisCanvasGroup;
    protected bool _isHidden = false;

    
    public virtual void Init()
    {
        _thisCanvasGroup = GetComponent<CanvasGroup>();
    }

    public virtual void Hide()
    {
        if (_isHidden) return;
        _thisCanvasGroup.alpha = 0f;
        _isHidden = true;
    }

    public virtual void Show()
    {
        if (!_isHidden) return;
        _thisCanvasGroup.alpha = 1f;
        _isHidden = false;
    }

    public virtual void BlockControl()
    {
        _thisCanvasGroup.blocksRaycasts = false;
    }
    
    public virtual void UnlockControl()
    {
        _thisCanvasGroup.blocksRaycasts = true;
    }
}

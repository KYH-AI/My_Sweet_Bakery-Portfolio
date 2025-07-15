using DG.Tweening;
using UnityEngine;

public class PingPongScaleShake : MonoBehaviour
{
    private void OnEnable()
    {
        this.transform
            .DOScale(  this.transform.localScale + new Vector3(0.15f, 0.15f, 0.15f), duration: 1f)
            .SetEase(Ease.InOutBack)
            .SetLoops(-1, LoopType.Yoyo);
    }
}

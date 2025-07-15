using DG.Tweening;
using UnityEngine;

public class TutorialArrowMark : MonoBehaviour
{
    private void OnEnable()
    {
        this.transform
            .DOMoveY(this.transform.position.y - 0.5f, 0.45f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}

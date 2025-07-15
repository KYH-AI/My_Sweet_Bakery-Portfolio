using DG.Tweening;
using UnityEngine;

public class ScaleShake : MonoBehaviour
{
    private void OnEnable()
    {
        /*
        * DOPunchScale 매개변수:
        * punch: 펀치 강도 (Vector3) - 얼마나 확대할지
        */
        this.transform.DOPunchScale( punch: new Vector3(0.5f, 0.5f, 0.5f), duration: 0.45f);
    }
}

using DG.Tweening;
using UnityEngine;

public class PaperBag : MonoBehaviour, IObjectPoolable
{
    [SerializeField] private Transform _breadRoot;
    private static string _TriggerCloseAnimation = "tBagClose";

    [SerializeField] private int _poolSize;
    public int PoolSize => _poolSize;

    public PoolRootType GetRootType()
    {
        return PoolRootType.TrayItem;
    }

    public void OnSpawn()
    {
        this.gameObject.SetActive(true);
    }

    public void OnDespawn()
    {
        this.gameObject.SetActive(false);
    }

    public void PaperBagCloseAnimation()
    {
        GetComponent<Animator>().SetTrigger(_TriggerCloseAnimation);
    }

    public void AddBread(IObjectPoolable bread)
    {
        bread.Transform.DOJump(_breadRoot.position, 3.5f, 1, 0.5f).OnComplete(() =>
        {
            GameManager.Instance.ObjectPoolManager.ReturnPool(bread);
        });
    }
    
    public GameObject GameObject => this.gameObject;
    public Transform Transform => this.transform;
}

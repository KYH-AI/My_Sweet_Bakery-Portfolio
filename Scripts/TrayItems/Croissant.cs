using UnityEngine;

public class Croissant : MonoBehaviour, IObjectPoolable
{
    [SerializeField] private int _poolSize;
    public int PoolSize => _poolSize;
    
    public PoolRootType GetRootType() => PoolRootType.TrayItem;
    
    public void OnSpawn()
    {
        this.gameObject.SetActive(true);
    }
    
    public void OnDespawn()
    {
        this.gameObject.SetActive(false);
    }
    
    public GameObject GameObject => this.gameObject;
    public Transform Transform => this.transform;
}

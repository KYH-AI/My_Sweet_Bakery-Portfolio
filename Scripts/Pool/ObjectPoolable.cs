using UnityEngine;

public interface IObjectPoolable 
{
    public int PoolSize { get; }
    public PoolRootType GetRootType();
    public void OnSpawn();
    public void OnDespawn();
    public GameObject GameObject { get; }
    public Transform Transform { get; }
}

using UnityEngine;

public class VFX_Emoji : MonoBehaviour, IObjectPoolable
{
    [SerializeField] private int _poolSize;
    public int PoolSize => _poolSize;
    
    public PoolRootType GetRootType()
    {
        return PoolRootType.Particle;
    }

    public void OnSpawn()
    {
        this.gameObject.SetActive(true);
        GetComponent<ParticleSystem>().Play();
    }

    public void OnDespawn()
    {
        this.gameObject.SetActive(false);
        GetComponent<ParticleSystem>().Stop();
    }
    
    private void OnParticleSystemStopped()
    {
        GameManager.Instance.ObjectPoolManager.ReturnPool(this);
    }
    
    
    public GameObject GameObject => this.gameObject;
    public Transform Transform => this.transform;
}

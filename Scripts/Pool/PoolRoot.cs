using UnityEngine;

public class PoolRoot : MonoBehaviour
{
    [SerializeField] private PoolRootType _rootType;
    public PoolRootType GetRootType => _rootType;
}

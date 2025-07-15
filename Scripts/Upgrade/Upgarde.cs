using UnityEngine;

public abstract class Upgarde : MonoBehaviour
{
    private void Awake()
    {
        Init();
    }

    protected abstract void Init();
    public abstract void RemoveLegacyObject();
    public abstract void Upgrade();
    public abstract BaseUpgradeable GetCurrentObject();
    public abstract int GetCurrentObjectLevel();
}

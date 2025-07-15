using System.Collections.Generic;
using UnityEngine;

public class ResourceLoad
{
    private static Dictionary<PoolRootType, string> _resourcePath = new Dictionary<PoolRootType, string>()
    {
        {PoolRootType.TrayItem, "TrayItems/"},
        {PoolRootType.Particle, "Particles/"},
        {PoolRootType.Character, "Characters/"},
    };

    public static string GetResourcesPath(PoolRootType poolType)
    {
        return _resourcePath[poolType];
    }

    public static T FindResource<T>(string path) where T : UnityEngine.Object
    {
        T resource = default(T);
        resource = UnityEngine.Resources.Load<T>(path);
        if(resource == null) DebugLog.CustomLog($"Failed to load {typeof(T).Name} from path: {path}", Color.red);
        return resource;
    }

    public static T[] FindResources<T>(string path) where T : UnityEngine.Object
    {
        T[] resources = UnityEngine.Resources.LoadAll<T>(path);
        if(resources == null) DebugLog.CustomLog($"Failed to load {typeof(T).Name} from path: {path}", Color.red);
        return resources;
    }

    public static T Instantiate<T>(T prefab, Transform parent = null) where T : UnityEngine.Object
    {
        T gameObject = null;
        if (parent)
        {
            gameObject = UnityEngine.Object.Instantiate(prefab, parent);
        }
        else
        {
            gameObject = UnityEngine.Object.Instantiate(prefab);
        }
        return gameObject;
    }
}

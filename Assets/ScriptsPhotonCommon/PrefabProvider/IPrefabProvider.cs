using UnityEngine;

namespace ScriptsPhotonCommon.PrefabProvider
{
    public interface IPrefabProvider
    {
        GameObject GetPrefab(string name);
    }
}
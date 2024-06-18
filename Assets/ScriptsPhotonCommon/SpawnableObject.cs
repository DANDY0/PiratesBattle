using UnityEngine;

namespace ScriptsPhotonCommon
{
    public class SpawnableObject : MonoBehaviour
    {
        [SerializeField] private string _key;

        public string Key => _key;
        
        private void Reset() => _key = gameObject.name;
    }
}
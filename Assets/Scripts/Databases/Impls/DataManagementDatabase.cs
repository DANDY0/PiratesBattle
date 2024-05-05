using Models;
using UnityEngine;

namespace Databases.Impls
{
    [CreateAssetMenu(menuName = "Databases/DataManagementDatabase", fileName = "DataManagementDatabase")]
    class DataManagementDatabase : ScriptableObject, IDataManagementDatabase
    {
        [SerializeField] private DataManagementVo _settings;

        public DataManagementVo Settings => _settings;
    }
}
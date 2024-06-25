using Enums;
using Models;
using UnityEngine;

namespace Databases.Impls
{
    [CreateAssetMenu(menuName = "Databases/ExampleDatabase", fileName = "ExampleDatabase")]
    public class ExampleDatabase : SpreadsheetDatabase, IExampleDatabase
    {
        [SerializeField] private ExampleSettingsVo _exampleSettings;

        public override ESpreadsheetDataType Name => ESpreadsheetDataType.First;
        public override SpreadsheetDataVo Data => _exampleSettings;
        public ExampleSettingsVo ExampleSettings => _exampleSettings;
    }
}
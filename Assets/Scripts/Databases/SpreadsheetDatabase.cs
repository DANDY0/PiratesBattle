using Enums;
using Models;
using UnityEngine;

namespace Databases
{
    public abstract class SpreadsheetDatabase : ScriptableObject, ISpreadsheetDatabase
    {
        [SerializeField] private bool _refreshAtStart = true;
        public bool RefreshAtStart => _refreshAtStart;
        public abstract ESpreadsheetDataType Name { get; }
        public abstract SpreadsheetDataVo Data { get; }
    }
}
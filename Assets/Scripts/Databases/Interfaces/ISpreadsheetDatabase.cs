using Enums;
using Models;

namespace Databases.Interfaces
{
    public interface ISpreadsheetDatabase
    {
        public bool RefreshAtStart { get; }
        public ESpreadsheetDataType Name { get; }
        public SpreadsheetDataVo Data { get; }

    }
}
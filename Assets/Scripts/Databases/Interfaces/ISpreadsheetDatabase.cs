using Enums;
using Models;

namespace Databases
{
    public interface ISpreadsheetDatabase
    {
        public bool RefreshAtStart { get; }
        public ESpreadsheetDataType Name { get; }
        public SpreadsheetDataVo Data { get; }

    }
}
using System.Threading.Tasks;
using Enums;
using Models.Data;

namespace Services.Spreadsheets
{
    public interface ISpreadsheetsService       
    {
        void FillSpreadsheetsInfo();
        Task StartLoadSpreadsheetsData();
        SpreadsheetInfo GetSpreadsheetByType(ESpreadsheetDataType type);
        void SetupDatabases();
    }
}
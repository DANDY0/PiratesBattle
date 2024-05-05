using Models;

namespace Databases
{
    public interface IDataManagementDatabase
    {
        DataManagementVo Settings { get; }
    }
}
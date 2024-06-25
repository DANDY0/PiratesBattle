using Models;

namespace Databases
{
    public interface ISettingsDatabase
    {
        SettingsVo Settings { get; }
    }
}
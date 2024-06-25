using Models;

namespace Databases
{
    public interface ILocalizationDatabase
    {
        LocalizationSettingsVo Settings { get; }
    }
}
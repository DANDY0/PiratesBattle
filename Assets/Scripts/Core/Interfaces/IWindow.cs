using static Utils.Enumerators;

namespace Core.Interfaces
{
    public interface IWindow
    {
        EWindow Name { get; }
        void Open();
        void Close();
    }
}
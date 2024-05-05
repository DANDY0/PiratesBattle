using Enums;

namespace Core.Interfaces
{
    public interface IWindow
    {
        EWindow Name { get; }
        void Open();
        void Close();
    }
}
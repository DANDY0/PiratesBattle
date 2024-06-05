using Enums;

namespace Services.Window
{
    public interface IWindowService
    {
        void ClearWindows();
        void RegisterWindow(Core.Abstracts.Window window, bool isFocusable, int orderNumber, bool isDontDestroyOnLoad);
        void Open(EWindow windowName);
        void Close(EWindow windowName);
        void SortBySiblingIndex();
    }
}
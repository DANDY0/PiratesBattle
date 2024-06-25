using System.Collections.Generic;
using System.Linq;
using Enums;
using Models;
using Unity.VisualScripting;
using Utils.Extensions;

namespace Services.Window
{
    public class WindowService : IWindowService
    {
        private readonly Dictionary<EWindow, WindowVo> _windows = new();

        private EWindow _activeWindow;
        
        public void ClearWindows()
        {
            var windowsToKeep = _windows.Where(kvp => kvp.Value.IsDontDestroyOnLoad).ToList();

            _windows.Clear();

            foreach (var window in windowsToKeep) 
                _windows.Add(window.Key, window.Value);
            BindExtensions.WindowsCount = 0;
        }

        public void RegisterWindow(Core.Abstracts.Window window, bool isFocusable, int orderNumber,
            bool isDontDestroyOnLoad) => _windows.Add(window.Name,
            new WindowVo
            {
                Window = window, 
                IsFocusable = isFocusable, 
                OrderNumber = orderNumber,
                IsDontDestroyOnLoad = isDontDestroyOnLoad
            });

        public void Open(EWindow windowName)
        {
            SortBySiblingIndex();

            var windowVo = _windows[windowName];
            windowVo.Window.Open();
            Close(_activeWindow);
            
            _activeWindow = windowName;
        }

        public void Close(EWindow windowName)
        {
            var windowVo = _windows[windowName];
            windowVo.Window.Close();
        }

        public void SortBySiblingIndex()
        {
            foreach (var windowVo in _windows.Values)
                windowVo.Window.transform.SetSiblingIndex(windowVo.OrderNumber);
        }
    }
}
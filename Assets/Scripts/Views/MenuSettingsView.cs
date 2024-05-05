using Core.Abstracts;
using Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class MenuSettingsView : Window
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _quitButton;
        
        public override EWindow Name => EWindow.MenuSettings;

        public Button PlayButton => _playButton;
        public Button QuitButton => _quitButton;
    }
}
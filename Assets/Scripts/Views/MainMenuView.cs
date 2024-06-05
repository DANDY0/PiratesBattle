using Core.Abstracts;
using Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class MainMenuView : Window
    {
        [SerializeField] private Button _playButton1;
        [SerializeField] private Button _playButton2;
        [SerializeField] private Button _quitButton;
        
        public override EWindow Name => EWindow.MenuSettings;

        public Button PlayButton1 => _playButton1;
        public Button PlayButton2 => _playButton2;
        public Button QuitButton => _quitButton;
    }
}
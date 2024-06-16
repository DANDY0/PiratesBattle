using Core.Abstracts;
using Enums;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Views
{
    public class MainMenuView : Window
    {
        [SerializeField] private Button _playButton1;
        [SerializeField] private Button _playButton2;
        [SerializeField] private Button _quitButton;
        [SerializeField] private SelectedCharacterPanel _selectedCharacterPanel;
        [SerializeField] private MenuProfilePanel _menuProfilePanel;

        public override EWindow Name => EWindow.MainMenu;

        public Button PlayButton1 => _playButton1;
        public Button PlayButton2 => _playButton2;
        public Button QuitButton => _quitButton;
        public SelectedCharacterPanel SelectedCharacterPanel => _selectedCharacterPanel;
        public MenuProfilePanel MenuProfilePanel => _menuProfilePanel;

    }
}
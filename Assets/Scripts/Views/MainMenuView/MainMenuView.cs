using Core.Abstracts;
using Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class MainMenuView : Window
    {
        [SerializeField] private Button _playButton1;
        [SerializeField] private Button _playButton2;
        [SerializeField] private Button _quitButton;
        [SerializeField] private ChooseCharacterPanel _chooseCharacterPanel;
        [SerializeField] private MenuProfilePanel _menuProfilePanel;

        public override EWindow Name => EWindow.MenuSettings;

        public Button PlayButton1 => _playButton1;
        public Button PlayButton2 => _playButton2;
        public Button QuitButton => _quitButton;
        public ChooseCharacterPanel ChooseCharacterPanel => _chooseCharacterPanel;
        public MenuProfilePanel MenuProfilePanel => _menuProfilePanel;

    }
}
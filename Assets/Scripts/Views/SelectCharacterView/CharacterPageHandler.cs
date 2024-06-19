using Core.Abstracts;
using Enums;
using Services.Window;
using UnityEngine;

namespace Views
{
    public class CharacterPageHandler : Handler<CharacterPagePanel>
    {
        private readonly CharactersListHandler _charactersListHandler;
        private readonly IWindowService _windowService;

        public CharacterPageHandler
        (
            IWindowService windowService,
            CharactersListHandler charactersListHandler
        )
        {
            _windowService = windowService;
            _charactersListHandler = charactersListHandler;
        }
        
        protected override void Initialize()
        {
            Debug.Log("CharacterPageHandler initialized");
            View.SelectButton.onClick.AddListener(SelectClickHandler);
            View.BackButton.onClick.AddListener(BackButtonClick);
        }

        private void SelectClickHandler()
        {
            // logic for saving current character
            
            _windowService.Open(EWindow.MainMenu);
        }

        private void BackButtonClick()
        {
            _charactersListHandler.Show();
            Hide();
        }
    }
}
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
            View.BackButton.onClick.AddListener(BackButtonClick);
        }
        
        private void BackButtonClick()
        {
            _charactersListHandler.Show();
            Hide();
        }
    }
}
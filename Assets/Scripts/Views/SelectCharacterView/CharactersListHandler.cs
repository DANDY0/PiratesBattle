using Core.Abstracts;
using Enums;
using Services.Window;
using UnityEngine;

namespace Views
{
    public class CharactersListHandler: Handler<CharactersListPanel>
    {
        private readonly IWindowService _windowService;

        public CharactersListHandler
        (
            IWindowService windowService
        )
        {
            _windowService = windowService;
        }

        protected override void Initialize()
        {
            Debug.Log("CharactersListHandler initialized");
            // CharacterElementView characterElementView = View.CharactersCollection.AddItem();
            
            View.BackButton.onClick.AddListener(BackButtonClick);
        }
        private void BackButtonClick()
        {
            _windowService.Open(EWindow.MainMenu);
        }
    }
}
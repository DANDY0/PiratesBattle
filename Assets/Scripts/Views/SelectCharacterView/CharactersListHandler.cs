using Core.Abstracts;
using Databases;
using Enums;
using Services.Window;
using UnityEngine;

namespace Views
{
    public class CharactersListHandler: Handler<CharactersListPanel>
    {
        private readonly IWindowService _windowService;
        private readonly ICharactersVisualDatabase _charactersVisualDatabase;
        // private readonly CharacterPageHandler _characterPageHandler;

        public CharactersListHandler
        (
            IWindowService windowService,
            ICharactersVisualDatabase charactersVisualDatabase,
            SelectCharacterView selectCharacterView
            )
        {
            _windowService = windowService;
            _charactersVisualDatabase = charactersVisualDatabase;
            // _characterPageHandler = characterPageHandler;
        }

        protected override void Initialize()
        {
            Debug.Log("CharactersListHandler initialized");
            View.BackButton.onClick.AddListener(BackButtonClick);

            FillCharactersList();
        }

        private void FillCharactersList()
        {
            foreach (var characterData in _charactersVisualDatabase.CharactersDataData.CharactersData)
            {
                CharacterElementView characterElementView = View.CharactersCollection.AddItem();
                characterElementView.SetUp(characterData);
                characterElementView.Button.onClick.AddListener(SelectCharacter);
            }
            
        }

        private void SelectCharacter()
        {
            // _characterPageHandler.Show();
            Hide();
        }
        
        private void BackButtonClick()
        {
            _windowService.Open(EWindow.MainMenu);
        }
    }
}
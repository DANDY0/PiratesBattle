using Core.Abstracts;
using Databases;
using Enums;
using Services.Data;
using Services.Window;
using UnityEngine;
using Utils;

namespace Views
{
    public class CharactersListHandler: Handler<CharactersListPanel>
    {
        private readonly IWindowService _windowService;
        private readonly IDataService _dataService;
        private readonly ICharactersVisualDatabase _charactersVisualDatabase;

        private readonly SelectCharacterView _selectCharacterView;

        public Enumerators.Character SelectedCharacter { get; private set; }

        public CharactersListHandler
        (
            IWindowService windowService,
            IDataService dataService,
            ICharactersVisualDatabase charactersVisualDatabase,
            SelectCharacterView selectCharacterView
        )
        {
            _windowService = windowService;
            _dataService = dataService;
            _charactersVisualDatabase = charactersVisualDatabase;
            _selectCharacterView = selectCharacterView;
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
                characterElementView.Button.onClick.AddListener(()=>SelectCharacter(characterData.Character));
            }
        }

        private void SelectCharacter(Enumerators.Character character)
        {
            SelectedCharacter = character;
            
            var characterVisualData = _charactersVisualDatabase.CharactersDataData.CharactersData.Find(
                c=>c.Character == SelectedCharacter);
            
            _selectCharacterView.CharacterPagePanel.SetCharacterImage(characterVisualData.FullImage);
            _selectCharacterView.EnableCharacterPagePanel(true);
            _selectCharacterView.EnableCharactersListPanel(false);
        }
        
        private void BackButtonClick()
        {
            _windowService.Open(EWindow.MainMenu);
        }
    }
}
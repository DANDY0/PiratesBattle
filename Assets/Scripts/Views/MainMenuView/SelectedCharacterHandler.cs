using Core.Abstracts;
using Enums;
using Services.Data;
using Services.Window;
using UnityEngine;

namespace Views
{
    public class SelectedCharacterHandler: Handler<SelectedCharacterPanel>
    {
        private readonly IDataService _dataService;
        private readonly IWindowService _windowService;

        public SelectedCharacterHandler
        (
            IDataService dataService,
            IWindowService windowService
        )
        {
            _dataService = dataService;
            _windowService = windowService;
        }

        protected override void Initialize()
        {
            Debug.Log("SelectedCharacterHandler initialized");
            
            View.CharacterButton.onClick.AddListener(CharacterButtonClick);
        }

        private void CharacterButtonClick()
        {
            _windowService.Open(EWindow.SelectCharacter);
        }
    }
}
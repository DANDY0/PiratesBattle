﻿using Core.Abstracts;
using Databases.Interfaces;
using Photon.PhotonUnityNetworking.Code.Common;
using Services.Data;
using Services.Window;
using UnityEngine;
using Utils;
using Views.MainMenuView;
using Views.SelectCharacterView;
using static Utils.Enumerators;

namespace Controllers.MainMenu
{
    public class CharacterPageHandler : Handler<CharacterPagePanel>
    {
        private readonly IWindowService _windowService;
        private readonly IDataService _dataService;
        private readonly ICharactersVisualDatabase _charactersVisualDatabase;
        private readonly CharactersListHandler _charactersListHandler;
        private readonly SelectCharacterView _selectCharacterView;
        private readonly MainMenuView _mainMenuView;

        public CharacterPageHandler
        (
            IWindowService windowService,
            IDataService dataService,
            ICharactersVisualDatabase charactersVisualDatabase,
            CharactersListHandler charactersListHandler,
            SelectCharacterView selectCharacterView,
            MainMenuView mainMenuView
        )
        {
            _windowService = windowService;
            _dataService = dataService;
            _charactersVisualDatabase = charactersVisualDatabase;
            _charactersListHandler = charactersListHandler;
            _selectCharacterView = selectCharacterView;
            _mainMenuView = mainMenuView;
        }
        
        protected override void Initialize()
        {
            View.SelectButton.onClick.AddListener(SelectClickHandler);
            View.BackButton.onClick.AddListener(BackButtonClick);
        }

        private void SelectClickHandler()
        {
            var selectedCharacter = _charactersListHandler.SelectedCharacter;
            var characterVisualData = _charactersVisualDatabase.CharactersDataData.CharactersData.Find(
                c=>c.Character == selectedCharacter);
            
            _dataService.CachedUserLocalData.SelectedCharacter = selectedCharacter;
            _mainMenuView.SelectedCharacterPanel.SetCharacterImage(characterVisualData.FullImage, selectedCharacter.ToString());
            
            _windowService.Open(EWindow.MainMenu);
            _selectCharacterView.EnableCharacterPagePanel(false);
        }

        private void BackButtonClick()
        {
            _charactersListHandler.Show();
            _selectCharacterView.EnableCharacterPagePanel(false);
        }
    }
}
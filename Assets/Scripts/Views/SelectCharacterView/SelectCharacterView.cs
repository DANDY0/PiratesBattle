using Core.Abstracts;
using Enums;
using UnityEngine;

namespace Views
{
    public class SelectCharacterView: Window
    {
        [SerializeField] private CharactersListPanel _charactersListPanel;
        [SerializeField] private CharacterPagePanel _characterPagePanel;
        
        public override EWindow Name => EWindow.SelectCharacter;
        
        public CharacterPagePanel CharacterPagePanel => _characterPagePanel;
        public CharactersListPanel CharactersListPanel => _charactersListPanel;

    }
}
using Collections;
using Core.Abstracts;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class CharactersListPanel: View
    { 
        [SerializeField] private Button _backButton;
        [SerializeField] private CharactersCollection _charactersCollection;
        
        public Button BackButton => _backButton;
        public CharactersCollection CharactersCollection => _charactersCollection;

    }
}
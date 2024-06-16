using Core.Abstracts;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class CharacterPagePanel: View
    {
        [SerializeField] private Button _backButton;
        public Button BackButton => _backButton;

    }
}
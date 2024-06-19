using Core.Abstracts;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class CharacterPagePanel: View
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _selectButton;

        public Button BackButton => _backButton;
        public Button SelectButton => _selectButton;

    }
}
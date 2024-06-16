using System;
using Core.Abstracts;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class SelectedCharacterPanel: View
    {
        [SerializeField] private Button _characterButton;

        public Button CharacterButton => _characterButton;
        
        private void Start()
        {
            Debug.Log("start");
        }
    }
    
}
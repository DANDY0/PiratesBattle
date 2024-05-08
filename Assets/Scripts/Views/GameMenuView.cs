using Core.Abstracts;
using Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class GameMenuView: Window
    {
        [SerializeField] private Button _leaveButton;
        
        public override EWindow Name => EWindow.GameMain;

        public Button LeaveButton => _leaveButton;
    }
}
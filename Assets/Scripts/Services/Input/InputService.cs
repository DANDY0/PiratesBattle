using System;
using SimpleInputNamespace;
using UnityEngine;
using Views;
using Zenject;

namespace Services.Input
{
    public class InputService : IInputService
    {
        private readonly GameMenuView _gameMenuView;
        public event Action FireStartedEvent;
        public event Action FireFinishedEvent;

        protected const string MoveHorizontal = "Horizontal";
        protected const string MoveVertical = "Vertical";
        protected const string LookHorizontal = "ShootHorizontal";
        protected const string LookVertical = "ShootVertical";
        protected const string Fire = "Fire";
        
        public Vector2 MoveAxis => GetSimpleInputAxis(MoveHorizontal, MoveVertical);
        public Vector2 LookAxis => GetSimpleInputAxis(LookHorizontal, LookVertical);

        public bool IsAttackPressedDown() => SimpleInput.GetButtonDown(Fire);

        private Vector2 GetSimpleInputAxis(string horizontal, string vertical) => 
            new Vector2(SimpleInput.GetAxis(horizontal), SimpleInput.GetAxis(vertical));
    }
}
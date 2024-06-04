using System;
using UnityEngine;
using Zenject;

namespace Services.Input
{
    public class InputService : IInputService, IInitializable
    {
        private InputHandler _mainInput;

        public Vector2 MoveAxis => _mainInput.MoveAxis;
        public Vector2 LookAxis => _mainInput.LookAxis;

        public bool IsAttackPressedDown() => _mainInput.IsAttackPressedDown();

        public void Initialize()
        {
            if (Application.isEditor)
                _mainInput = new StandaloneInputService();
            else
                _mainInput = new MobileInputService();
        }
    }
}
using UnityEngine;

namespace Services.Input
{
    public interface IInputService
    {
        public Vector2 MoveAxis { get; }
        public Vector2 LookAxis { get; }
        public bool IsAttackPressedDown();
    }
}
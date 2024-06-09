using System;
using UnityEngine;

namespace Services.Input
{
    public interface IInputService
    {
        event Action StartFiringEvent;
        event Action StopFiringEvent;
        event Action SetAutoFireEvent;
        event Action SetPreciseFireEvent;
        
        public Vector2 MoveAxis { get; }
        public Vector2 LookAxis { get; }
        
        bool IsFiring { get;  }
        bool IsAutoFiring { get; }
        bool IsPreciseFiring { get; }
    }
}
using UnityEngine;

namespace Services.Input
{
    public class MobileInputService : InputHandler
    {
        public override Vector2 MoveAxis => GetSimpleInputAxis(MoveHorizontal, MoveVertical);
        public override Vector2 LookAxis => GetSimpleInputAxis(LookHorizontal, LookVertical);
    }
}
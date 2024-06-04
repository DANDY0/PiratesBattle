using UnityEngine;

namespace Services.Input
{
    public class StandaloneInputService : InputHandler
    {
        public override Vector2 MoveAxis
        {
            get
            {
                Vector2 axis = GetSimpleInputAxis(MoveHorizontal, MoveVertical);
                if (axis == Vector2.zero)
                    axis = UnityAxis(MoveHorizontal, MoveVertical);
                return axis;
            }
        }

        public override Vector2 LookAxis
        {
            get
            {
                Vector2 axis = GetSimpleInputAxis(LookHorizontal, LookVertical);
                if (axis == Vector2.zero)
                    axis = UnityAxis(LookHorizontal, LookVertical);
                return axis;
            }
        }

        private static Vector2 UnityAxis(string horizontal, string vertical) => 
            new Vector2(UnityEngine.Input.GetAxis(horizontal), UnityEngine.Input.GetAxis(vertical));
    }
}
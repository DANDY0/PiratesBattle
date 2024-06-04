using UnityEngine;
using Logger = Utils.Logger;

namespace Services.Input
{
    public abstract class InputHandler
    {
        protected const string MoveHorizontal = "Horizontal";
        protected const string MoveVertical = "Vertical";
        protected const string LookHorizontal = "ShootHorizontal";
        protected const string LookVertical = "ShootVertical";
        private const string Button = "Fire";

        public abstract Vector2 MoveAxis { get; }
        public abstract Vector2 LookAxis { get; }
        public bool IsAttackPressedDown() => SimpleInput.GetButtonDown(Button);

        protected Vector2 GetSimpleInputAxis(string horizontal, string vertical)
        {
            return new Vector2(SimpleInput.GetAxis(horizontal), SimpleInput.GetAxis(vertical));
        }
    }
}
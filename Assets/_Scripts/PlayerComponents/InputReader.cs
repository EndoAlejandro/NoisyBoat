using UnityEngine;

namespace PlayerComponents
{
    public class InputReader
    {
        public bool Sonar => _input?.Boat.Sonar.IsPressed() ?? false;

        public Vector3 Move
        {
            get
            {
                Vector2 input = _input?.Boat.Move.ReadValue<Vector2>() ?? Vector2.zero;
                return new Vector3(input.x, 0f, input.y);
            }
        }

        private readonly BoatInputActions _input = new BoatInputActions();

        public void Enable() => _input.Boat.Enable();
        public void Disable() => _input.Boat.Disable();
    }
}
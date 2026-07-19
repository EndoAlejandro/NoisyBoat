using UnityEngine;

namespace PlayerComponents
{
    public static class InputReader
    {
        public static bool Sonar => Input.Boat.Sonar.IsPressed();

        public static Vector3 Move
        {
            get
            {
                Vector2 input = Input?.Boat.Move.ReadValue<Vector2>() ?? Vector2.zero;
                return new Vector3(input.x, 0f, input.y);
            }
        }

        private static readonly BoatInputActions Input = new BoatInputActions();

        public static void Enable() => Input.Boat.Enable();
        
        public static void Disable()
        {
            Input.Boat.Disable();
            Input.Dispose();
        }
    }
}
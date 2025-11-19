using UnityEngine;

namespace PlayerComponents
{
    public class PlayerVisuals : MonoBehaviour
    {
        [Header("Dependencies")]
        [Tooltip("The Transform that will be tilted (e.g., the visual model or cockpit).")]
        public Transform visualTransform;


        [Header("Roll Parameters (Sideways Tilt)")]
        [Tooltip("The factor by which the sideways velocity is multiplied to determine the maximum tilt angle (Roll).")]
        public float rollIntensity = 2.0f;

        [Tooltip("The maximum angle (in degrees) the visual model can tilt on the Z-axis (Roll).")]
        public float maxRollAngle = 30f;

        [Header("Pitch Parameters (Forward/Backward Tilt)")]
        [Tooltip("The factor by which the forward velocity is multiplied to determine the maximum tilt angle (Pitch).")]
        public float pitchIntensity = 0.5f;

        [Tooltip("The maximum angle (in degrees) the visual model can tilt on the X-axis (Pitch).")]
        public float maxPitchAngle = 10f;

        [Header("Smoothing")]
        [Tooltip("The speed at which the visual object interpolates to the target rotation.")]
        public float tiltSmoothness = 5f;

        private Rigidbody _rigidbody;
        
        void Awake()
        {
            // 1. Get the Rigidbody component
            _rigidbody = GetComponent<Rigidbody>();

            if (visualTransform == null)
            {
                Debug.LogError("The visualTransform field must be assigned in the Inspector!");
                enabled = false;
            }
        }

        // FixedUpdate is used for physics-related operations, ensuring smooth motion.
        void FixedUpdate()
        {
            if (visualTransform == null) return;

            // 1. Convert the Rigidbody's world velocity into local space.
            // localVelocity.x: Sideways (left/right) -> Used for Roll (Z)
            // localVelocity.z: Forward/Backward      -> Used for Pitch (X)
            Vector3 localVelocity = transform.InverseTransformDirection(_rigidbody.linearVelocity);

            // --- A. Roll Calculation (Z-axis, Banking) ---

            // Calculate the Roll angle based on sideways velocity (X)
            float targetRoll = localVelocity.x * rollIntensity;
            targetRoll = Mathf.Clamp(targetRoll, -maxRollAngle, maxRollAngle);

            // --- B. Pitch Calculation (X-axis, Acceleration/Deceleration) ---

            // Calculate the Pitch angle based on forward velocity (Z)
            // If localVelocity.z is positive (accelerating forward), pitchTilt should be negative (nosing down).
            // If localVelocity.z is negative (braking/reversing), pitchTilt should be positive (nosing up).
            float targetPitch = localVelocity.z * pitchIntensity;
            targetPitch = Mathf.Clamp(targetPitch, -maxPitchAngle, maxPitchAngle);

            // We use the negative of the velocity for a natural pitch visualization: 
            // Positive velocity -> Nose down (negative X rotation).
            // Negative velocity -> Nose up (positive X rotation).
            float finalPitch = -targetPitch;


            // 4. Create the Target Rotation
            // Combine the calculated Roll (Z) and Pitch (X) angles. Yaw (Y) remains 0.
            Quaternion targetRotation = Quaternion.Euler(
                finalPitch, // Pitch on X-axis (Forward/Backward tilt)
                0,          // No Yaw change on Y
                -targetRoll // Roll on Z-axis (Sideways bank, negative often needed for visual correctness)
            );

            // 5. Smoothly Interpolate the Visual Transform's Rotation
            // Use Quaternion.Slerp for smooth, non-linear rotation blending.
            visualTransform.localRotation = Quaternion.Slerp(
                visualTransform.localRotation,
                targetRotation,
                Time.fixedDeltaTime * tiltSmoothness
            );
        }
    }
}
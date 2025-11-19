using UnityEngine;

public class VisualBuoyancy : MonoBehaviour
{
    [Header("Base Settings")]
    [Tooltip("The initial height where the object should float.")]
    public float baseWaterLevel = 0f;

    [Tooltip("How much the object should be submerged below the surface line.")]
    public float submergenceDepth = 0.5f;

    [Header("Wave Parameters")]
    [Tooltip("The maximum height the waves reach.")]
    public float amplitude = 0.5f;

    [Tooltip("The horizontal distance between wave peaks.")]
    public float waveLength = 10f;

    [Tooltip("How fast the waves travel.")]
    public float waveSpeed = 2f;

    [Header("Tilt & Smoothness")]
    [Tooltip("Factor determining how aggressively the object tilts to match the wave slope.")]
    public float tiltFactor = 5f;

    [Tooltip("Speed at which the object transitions to the new wave position/rotation.")]
    public float smoothness = 4f;
    
    // Internal state to store the object's original local position for the visual offset
    private Vector3 initialLocalPosition;
    
    void Start()
    {
        // Store the initial local offset if the visual component is a child
        initialLocalPosition = transform.localPosition;
    }

    void Update()
    {
        // Get the current World Position of the object
        Vector3 currentPosition = transform.position;

        // 1. Calculate the Target Wave Height and Surface Normal (Slope)
        Vector3 targetWavePosition;
        Quaternion targetRotation;

        // Calculate wave properties at the object's current X and Z coordinates
        CalculateWaveProperties(currentPosition.x, currentPosition.z, out targetWavePosition, out targetRotation);

        // --- Position Update ---
        
        // Calculate the final target Y position: 
        // WaveHeight + Base Level - Submergence Offset
        float finalTargetY = targetWavePosition.y + baseWaterLevel - submergenceDepth;

        Vector3 targetPosition = new Vector3(
            currentPosition.x,
            finalTargetY,
            currentPosition.z
        );

        // Smoothly interpolate to the target Y position
        transform.position = Vector3.Lerp(currentPosition, targetPosition, Time.deltaTime * smoothness);

        // --- Rotation Update ---

        // Smoothly interpolate to the target wave surface rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothness * 0.5f);
    }

    /// <summary>
    /// Calculates the wave height and the rotation needed to align with the wave surface.
    /// </summary>
    /// <param name="x">World X position.</param>
    /// <param name="z">World Z position.</param>
    /// <param name="wavePos">Output: The calculated wave height (Y).</param>
    /// <param name="waveRot">Output: The calculated rotation (Pitch and Roll).</param>
    private void CalculateWaveProperties(float x, float z, out Vector3 wavePos, out Quaternion waveRot)
    {
        // Use two different sine waves traveling in different directions to create complexity (like ocean waves)
        
        // Wave 1: Traveling mostly along the Z axis
        float wave1Y = amplitude * Mathf.Sin(
            (x / waveLength) + (z / waveLength) + (Time.time * waveSpeed)
        );
        
        // Wave 2: Traveling mostly along the X axis
        float wave2Y = (amplitude / 2f) * Mathf.Sin(
            (x / (waveLength / 2f)) - (z / (waveLength * 1.5f)) + (Time.time * waveSpeed * 1.5f)
        );

        // Total wave height
        float totalY = wave1Y + wave2Y;
        wavePos = new Vector3(x, totalY, z);

        // --- Calculate Rotation (Surface Normal) ---

        // Calculate the slope (gradient) of the wave at this point to determine tilt.
        // We do this by sampling the wave height slightly ahead on X and Z axes.
        float offset = 0.1f;
        
        // Get height slightly ahead on X
        float heightXPlus = GetWaveHeight(x + offset, z);
        // Get height slightly ahead on Z
        float heightZPlus = GetWaveHeight(x, z + offset);

        // Create the normal vector components
        // The slope is (Height_Difference / Offset_Distance)
        Vector3 normal = new Vector3(
            (heightXPlus - totalY) / offset, // X component (Pitch)
            1f,                              // Y component (Always pointing up)
            (heightZPlus - totalY) / offset  // Z component (Roll)
        ).normalized;

        // Use Quaternion.LookRotation to create a rotation that faces in the direction of the normal.
        // By multiplying the normal's Y component (1f) by the tiltFactor, we control how much the object aligns with the slope.
        waveRot = Quaternion.LookRotation(
            new Vector3(normal.x * tiltFactor, 
                normal.y, 
                normal.z * tiltFactor)
        );
    }

    /// <summary>
    /// Helper function to get the combined wave height at a specific world coordinate.
    /// </summary>
    private float GetWaveHeight(float x, float z)
    {
        float wave1Y = amplitude * Mathf.Sin(
            (x / waveLength) + (z / waveLength) + (Time.time * waveSpeed)
        );
        
        float wave2Y = (amplitude / 2f) * Mathf.Sin(
            (x / (waveLength / 2f)) - (z / (waveLength * 1.5f)) + (Time.time * waveSpeed * 1.5f)
        );
        
        return wave1Y + wave2Y;
    }
}
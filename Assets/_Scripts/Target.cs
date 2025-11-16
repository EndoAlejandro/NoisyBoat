using Shapes;
using UnityEngine;

public class Target : ImmediateModeShapeDrawer
{
    private Color _color;
    private float _currentSize;
    
    public override void DrawShapes(Camera cam)
    {
        using (Draw.Command(cam))
        {
            Draw.LineGeometry = LineGeometry.Volumetric3D;
            Draw.ThicknessSpace = ThicknessSpace.Meters;
            Draw.Thickness = .1f;

            Draw.Matrix = transform.localToWorldMatrix;
            Draw.Ring(Vector3.zero, Quaternion.Euler(Vector3.right * 90f), _currentSize, _color);
        }
    }
}

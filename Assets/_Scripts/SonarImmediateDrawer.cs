using PlayerComponents;
using Shapes;
using UnityEngine;

public abstract class SonarImmediateDrawer : ImmediateModeShapeDrawer
{
    [Header("Sonar")]
    [SerializeField, ColorUsage(true, false)]
    protected Color _baseColor;

    [SerializeField] protected float _range;
    [SerializeField] private float _debugOffset = 2f;

    [SerializeField] private AudioSource _source;
    
    protected float alpha;

    protected virtual void Update()
    {
        var distance = Vector3.Distance(Player.Instance.transform.position, transform.position);
        alpha = 1f - distance / _range;
        alpha = Mathf.Clamp(alpha, 0f, 1f);
    }

    public void Detect() => _source.Play();

    private void OnDrawGizmos()
    {
        if (Player.Instance == null) return;

        var offset = Vector3.up * _debugOffset;
        var position = Player.Instance?.transform?.position ?? Vector3.zero;
        Gizmos.DrawLine(position, transform.position + offset);
    }
}
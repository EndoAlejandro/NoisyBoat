using System;
using PlayerComponents;
using Shapes;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class SonarImmediateDrawer : ImmediateModeShapeDrawer
{
    public Player Player { get; protected set; }


    [field: Header("Sonar")]
    [field: SerializeField]
    [field: ColorUsage(true, false)]
    public Color BaseColor { get; private set; } = Color.white;


    [SerializeField] protected float _detectionRange;
    [SerializeField] private float _debugOffset = 2f;
    [SerializeField] private AudioSource _source;

    protected float alpha;

    public bool CanBeDetected { get; protected set; } = true;

    protected virtual void Awake()
    {
        Player.OnSpawn += PlayerOnSpawn;
        Player.OnDead += PlayerOnDead;
    }

    private void OnDestroy()
    {
        Player.OnSpawn -= PlayerOnSpawn;
        Player.OnDead -= PlayerOnDead;
    }

    private void PlayerOnDead() => Player = null;
    private void PlayerOnSpawn(Player player) => Player = player;

    protected virtual void Update()
    {
        if (Player == null) return;

        var distance = Vector3.Distance(Player?.transform?.position ?? transform.position, transform.position);
        alpha = 1f - distance / _detectionRange;
        alpha = Mathf.Clamp(alpha, 0f, 1f);
    }

    public void Detect() => _source.Play();

    private void OnDrawGizmos()
    {
        if (Player == null) return;

        var offset = Vector3.up * _debugOffset;
        var position = Player?.transform?.position ?? Vector3.zero;
        Gizmos.DrawLine(position, transform.position + offset);
    }
}
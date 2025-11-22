using System;
using Enemies;
using PlayerComponents;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    public static event Action AllSharksCaptured;

    [SerializeField] private Shark _sharkPrefab;
    [SerializeField] private float _spawnRange = 25f;

    private int _sharksToCapture = 3;
    private Player _player;

    private void Awake()
    {
        Shark.OnCaged += SharkOnCaged;
        Player.OnSpawn += PlayerOnSpawn;
    }

    private void OnDestroy()
    {
        Shark.OnCaged -= SharkOnCaged;
        Player.OnSpawn -= PlayerOnSpawn;
    }

    private void PlayerOnSpawn(Player player)
    {
        _player = player;
    }

    private void SharkOnCaged()
    {
        _sharksToCapture--;

        if (_sharksToCapture > 0)
        {
            var random2 = Random.insideUnitCircle;
            var position = random2.normalized * _spawnRange;
            var shark = Instantiate(_sharkPrefab, position, Quaternion.identity);
            shark.Setup(_player);
        }
        else
        {
            AllSharksCaptured?.Invoke();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _spawnRange);
    }
}
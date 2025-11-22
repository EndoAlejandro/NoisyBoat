using PlayerComponents;
using UnityEngine;

public class AudioListenerController : MonoBehaviour
{
    private void Awake()
    {
        Player.OnSpawn += PlayerOnSpawn;
        Player.OnDead += PlayerOnDead;
    }

    private void OnDestroy()
    {
        Player.OnSpawn -= PlayerOnSpawn;
        Player.OnDead -= PlayerOnDead;
    }
    
    private void Reset()
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    private void PlayerOnSpawn(Player player)
    {
        transform.SetParent(player.transform);
        Reset();
    }

    private void PlayerOnDead()
    {
        transform.SetParent(null);
        Reset();
    }
}
// 
using UnityEngine;
using DG.Tweening;

public class PowerUp : MonoBehaviour, IPoolable
{
    [SerializeField] private float      scrollSpeed      = 1.5f;
    [SerializeField] private GameObject collectBurstPrefab;

    private void Update()
    {
        transform.Translate(Vector3.down * scrollSpeed * Time.deltaTime, Space.World);
        if (ScreenBounds.IsBelowScreen(transform.position))
            PoolManager.Instance.Return(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerShooter>(out var shooter)) return;
        if (other.TryGetComponent<PlayerHealth>(out var health) && health.IsInvincible) return;

        shooter.ActivatePowerup();
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.powerupCollect);

        if (collectBurstPrefab != null)
            Instantiate(collectBurstPrefab, transform.position, Quaternion.identity);

        PoolManager.Instance.Return(gameObject);
    }

    public void OnGetFromPool()  { }
    public void OnReturnToPool() { DOTween.Kill(transform); }
}

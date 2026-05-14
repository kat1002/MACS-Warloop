using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour, IPoolable
{
    [SerializeField] private float  speed         = 10f;
    [SerializeField] private int    damage        = 25;
    [SerializeField] private bool   isEnemyBullet = false;
    [SerializeField] private bool   isPowered     = false;

    public void Setup(float spd, int dmg, bool enemy)
    {
        speed         = spd;
        damage        = dmg;
        isEnemyBullet = enemy;
    }

    private void Update()
    {
        Vector3 dir = isEnemyBullet ? Vector3.down : Vector3.up;
        transform.Translate(dir * speed * Time.deltaTime, Space.World);

        if (ScreenBounds.IsBelowScreen(transform.position) ||
            ScreenBounds.IsAboveScreen(transform.position))
            PoolManager.Instance.Return(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isEnemyBullet && other.TryGetComponent<EnemyHealth>(out var enemy))
        {
            enemy.TakeDamage(damage, isPowered);
            AudioManager.Instance?.PlaySFX(AudioManager.Instance.enemyHit);
            PoolManager.Instance.Return(gameObject);
        }
        else if (isEnemyBullet && other.TryGetComponent<PlayerHealth>(out var player))
        {
            player.TakeDamage();
            PoolManager.Instance.Return(gameObject);
        }
    }

    public void OnGetFromPool()  { }
    public void OnReturnToPool() { }
}

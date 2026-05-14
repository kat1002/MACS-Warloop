using UnityEngine;
using DG.Tweening;

public class EnemyHealth : MonoBehaviour, IPoolable
{
    [Header("Stats")]
    [SerializeField] private int   maxHp      = 100;
    [SerializeField] private int   damage     = 25;
    [SerializeField] private int   killPoints = 25;

    [Header("Refs")]
    [SerializeField] private GameObject powerUpPrefab;
    [SerializeField] private GameObject poweredHitVfxPrefab;
    [SerializeField] private Material   hitMaterial;

    public int Damage => damage;

    private static readonly int FlashId = Shader.PropertyToID("_Flash");

    private int            currentHp;
    private SpriteRenderer sr;
    private Color          originalColor;
    private Material       originalMaterial;
    private Material       hitMatInstance;
    private Tween          hitTween;

    private void Awake()
    {
        sr            = GetComponent<SpriteRenderer>();
        originalColor = sr != null ? sr.color : Color.white;
        if (sr != null) originalMaterial = sr.sharedMaterial;

        if (hitMaterial != null) hitMatInstance = new Material(hitMaterial);
    }

    public void TakeDamage(int amount, bool powered = false)
    {
        currentHp -= amount;
        FlashHit();
        if (powered) SpawnPoweredHitVfx();
        if (currentHp <= 0) Die();
    }

    private void SpawnPoweredHitVfx()
    {
        if (poweredHitVfxPrefab == null) return;
        var vfx = PoolManager.Instance.Get(poweredHitVfxPrefab);
        vfx.transform.position = transform.position;
    }

    private void FlashHit()
    {
        if (sr == null) return;
        hitTween?.Kill();
        if (hitMatInstance != null)
        {
            sr.material = hitMatInstance;
            hitMatInstance.SetFloat(FlashId, 1f);
            hitTween = DOTween.To(
                () => hitMatInstance.GetFloat(FlashId),
                x  => hitMatInstance.SetFloat(FlashId, x),
                0f, 0.13f)
                .OnComplete(() => { if (sr != null) sr.material = originalMaterial; });
        }
        else
        {
            sr.DOColor(Color.white, 0.05f).OnComplete(() => sr?.DOColor(originalColor, 0.08f));
        }
    }

    private void Die()
    {
        hitTween?.Kill();
        if (sr != null) sr.material = originalMaterial;

        EventManager.EmitEnemyDefeated(killPoints, transform.position);
        GameManager.Instance?.AddScore(killPoints, transform.position);

        if (Random.value <= 0.07f && powerUpPrefab != null)
        {
            var p = PoolManager.Instance.Get(powerUpPrefab);
            p.transform.position = transform.position;
        }

        GetComponent<EnemyDeathVFX>()?.Play();
        PoolManager.Instance.Return(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerHealth>(out var player))
            player.TakeDamage();
    }

    public void OnGetFromPool()
    {
        currentHp = maxHp;
        hitTween?.Kill();
        if (sr != null)
        {
            sr.color    = originalColor;
            sr.material = originalMaterial;
        }
        hitMatInstance?.SetFloat(FlashId, 0f);
    }

    public void OnReturnToPool()
    {
        hitTween?.Kill();
        if (sr != null) sr.material = originalMaterial;
        DOTween.Kill(sr);
    }
}

using UnityEngine;
using DG.Tweening;

public class EnemyController : MonoBehaviour, IPoolable
{
    [Header("Stats")]
    [SerializeField] private float moveSpeed    = 2f;
    [SerializeField] private float fireInterval = 2f;

    [Header("Skins")]
    [SerializeField] private Sprite[] skins;

    [Header("Refs")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private LayerMask  playerLayer;

    private SpriteRenderer sr;
    private float          halfH;
    private float          nextFireTime;

    private void Awake()
    {
        sr    = GetComponent<SpriteRenderer>();
        halfH = sr != null ? sr.bounds.extents.y : 0.16f;
    }

    private void Update()
    {
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime, Space.World);

        if (ScreenBounds.IsBelowScreen(transform.position))
        {
            PoolManager.Instance.Return(gameObject);
            return;
        }

        if (ScreenBounds.IsFullyVisible(transform.position, halfH) && Time.time >= nextFireTime)
            TryFire();
    }

    private void TryFire()
    {
        var hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, playerLayer);
        if (hit.collider == null) return;

        nextFireTime = Time.time + fireInterval;
        if (bulletPrefab == null) return;

        var b = PoolManager.Instance.Get(bulletPrefab); 
        b.transform.SetPositionAndRotation(transform.position, Quaternion.Euler(0f, 0f, 180f));
        b.GetComponent<Bullet>()?.Setup(7f, 25, true);
    }

    public void OnGetFromPool()
    {
        nextFireTime = 0f;

        if (skins != null && skins.Length > 0 && sr != null)
            sr.sprite = skins[Random.Range(0, skins.Length)];

        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }

    public void OnReturnToPool()
    {
        DOTween.Kill(transform);
    }
}

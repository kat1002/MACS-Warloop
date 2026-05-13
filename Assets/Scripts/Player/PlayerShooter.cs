using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class PlayerShooter : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] public GameObject normalBulletPrefab;
    [SerializeField] public GameObject poweredBulletPrefab;

    [Header("Settings")]
    [SerializeField] private float fireRate        = 4f;
    [SerializeField] private float powerupDuration = 10f;

    public bool PowerupActive => powerLevel > 0;
    public int  PowerLevel    => powerLevel;

    private InputSystem_Actions input;
    private SpriteRenderer      sr;
    private float               fireTimer;
    private int                 powerLevel;
    private Coroutine           powerupCoroutine;
    private Sequence            flickerSequence;

    private void Awake()
    {
        input = new InputSystem_Actions();
        sr    = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()  => input.Player.Enable();
    private void OnDisable() => input.Player.Disable();

    private void Update()
    {
        fireTimer -= Time.deltaTime;
        if (input.Player.Attack.IsPressed() && fireTimer <= 0f)
        {
            Fire();
            fireTimer = 1f / fireRate;
        }
    }

    private void Fire()
    {
        var prefab = PowerupActive ? poweredBulletPrefab : normalBulletPrefab;
        if (prefab == null) return;
        var bullet = PoolManager.Instance.Get(prefab);
        bullet.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.playerShoot);
    }

    public void ActivatePowerup()
    {
        if (powerupCoroutine != null) StopCoroutine(powerupCoroutine);
        StopFlicker();
        powerupCoroutine = StartCoroutine(PowerupRoutine());
        EventManager.EmitPowerUpCollected();
    }

    private void StopFlicker()
    {
        flickerSequence?.Kill();
        flickerSequence = null;
        if (sr != null) { var c = sr.color; c.a = 1f; sr.color = c; }
    }

    private IEnumerator PowerupRoutine()
    {
        powerLevel = Mathf.Min(powerLevel + 1, 2);
        SkinManager.Instance?.ApplySprite(sr, powerLevel);

        yield return new WaitForSeconds(powerupDuration - 3f);

        if (sr != null)
        {
            flickerSequence = DOTween.Sequence()
                .Append(sr.DOFade(0.15f, 0.10f))
                .Append(sr.DOFade(1.00f, 0.10f))
                .SetLoops(-1);
        }

        yield return new WaitForSeconds(3f);

        StopFlicker();
        powerLevel = 0;
        SkinManager.Instance?.ApplySprite(sr, 0);
        powerupCoroutine = null;
    }
}

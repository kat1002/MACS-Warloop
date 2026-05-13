using System.Collections;
using UnityEngine;
using DG.Tweening;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float recoveryTime = 3f;

    private enum State { Idle, Recovery, Dead }
    private State state = State.Idle;

    public bool IsInvincible => state != State.Idle;

    private static readonly int FlashId = Shader.PropertyToID("_Flash");

    private SpriteRenderer sr;
    private Collider2D     col;
    private Tween          flashTween;
    private Color          originalColor;
    private Material       originalMaterial;
    private Material       hitMatInstance;

    private void Awake()
    {
        sr  = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        if (sr != null)
        {
            originalColor    = sr.color;
            originalMaterial = sr.sharedMaterial;
        }

        var shader = Shader.Find("Shader Graphs/HitShader");
        if (shader != null) hitMatInstance = new Material(shader);
    }

    public void TakeDamage()
    {
        if (state != State.Idle) return;
        if (GameManager.Instance == null) return;

        state       = State.Recovery;
        col.enabled = false;

        GameManager.Instance.LoseLife();
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.playerHit);

        Camera.main.transform.DOShakePosition(0.2f, strength: 0.3f, vibrato: 12);
        transform.DOPunchPosition(new Vector3(0f, -0.4f, 0f), 0.25f, 8, 0.5f);

        StartFlash();

        if (GameManager.Instance.Lives <= 0)
        {
            state = State.Dead;
            StartCoroutine(DeathSequence());
            return;
        }

        Invoke(nameof(EndRecovery), recoveryTime);
    }

    private void StartFlash()
    {
        flashTween?.Kill();
        if (hitMatInstance != null)
        {
            sr.material = hitMatInstance;
            hitMatInstance.SetFloat(FlashId, 0f);
            flashTween = DOTween.Sequence()
                .Append(DOTween.To(
                    () => hitMatInstance.GetFloat(FlashId),
                    x  => hitMatInstance.SetFloat(FlashId, x),
                    1f, 0.08f).SetEase(Ease.Linear))
                .AppendInterval(0.06f)
                .Append(DOTween.To(
                    () => hitMatInstance.GetFloat(FlashId),
                    x  => hitMatInstance.SetFloat(FlashId, x),
                    0f, 0.22f).SetEase(Ease.OutCubic))
                .AppendInterval(0.08f)
                .SetLoops(-1);
        }
        else
        {
            flashTween = DOTween.Sequence()
                .Append(sr.DOColor(Color.white, 0.1f))
                .Append(sr.DOColor(originalColor, 0.1f))
                .SetLoops(-1);
        }
    }

    private void EndRecovery()
    {
        flashTween?.Kill();
        if (hitMatInstance != null) hitMatInstance.SetFloat(FlashId, 0f);
        if (sr != null)
        {
            sr.color    = originalColor;
            sr.material = originalMaterial;
        }
        col.enabled = true;
        state       = State.Idle;
    }

    private IEnumerator DeathSequence()
    {
        flashTween?.Kill();
        if (hitMatInstance != null) hitMatInstance.SetFloat(FlashId, 0f);
        if (sr != null) sr.material = originalMaterial;
        GetComponent<PlayerDeathVFX>()?.Play();
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}

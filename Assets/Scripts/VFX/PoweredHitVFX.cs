using System.Collections;
using UnityEngine;

public class PoweredHitVFX : MonoBehaviour, IPoolable
{
    [SerializeField] private Sprite[] frames;
    [SerializeField] private float    fps = 12f;

    private SpriteRenderer sr;
    private Coroutine      anim;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void OnGetFromPool()
    {
        if (anim != null) StopCoroutine(anim);
        anim = StartCoroutine(Animate());
    }

    public void OnReturnToPool()
    {
        if (anim == null) return;
        StopCoroutine(anim);
        anim = null;
    }

    private IEnumerator Animate()
    {
        float delay = 1f / fps;
        foreach (var frame in frames)
        {
            if (sr != null) sr.sprite = frame;
            yield return new WaitForSeconds(delay);
        }
        anim = null;
        PoolManager.Instance.Return(gameObject);
    }
}

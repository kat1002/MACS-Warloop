using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class BulletTrail : MonoBehaviour, IPoolable
{
    [SerializeField] private TrailRenderer trail;

    public void OnGetFromPool()
    {
        trail.emitting = false;
        trail.Clear();
        StartCoroutine(ReEnableEmitting());
    }

    private System.Collections.IEnumerator ReEnableEmitting()
    {
        yield return null; // one frame: caller sets spawn position before we start recording
        trail.Clear();
        trail.emitting = true;
    }

    public void OnReturnToPool()
    {
        StopAllCoroutines();
        trail.emitting = false;
        trail.Clear();
    }
}

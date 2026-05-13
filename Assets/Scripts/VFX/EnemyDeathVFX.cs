using UnityEngine;

public class EnemyDeathVFX : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float      scaleMultiplier = 1f;

    public void Play()
    {
        if (explosionPrefab == null) return;

        var vfx = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        vfx.transform.localScale = Vector3.one * scaleMultiplier;
    }
}

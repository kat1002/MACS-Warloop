using TMPro;
using UnityEngine;
using DG.Tweening;

public class FloatingText : MonoBehaviour, IPoolable
{
    [SerializeField] private TextMeshPro tmp;
    [SerializeField] private float floatDistance = 1.5f;
    [SerializeField] private float duration      = 1f;

    private void Awake()
    {
        if (tmp == null) tmp = GetComponent<TextMeshPro>();
    }

    public void Show(int amount, Vector3 worldPos)
    {
        transform.position = worldPos;
        tmp.text  = $"+{amount}";
        tmp.color = GetColor(amount);
        tmp.alpha = 1f;

        DOTween.Kill(transform);
        DOTween.Kill(tmp);

        transform.DOMove(worldPos + Vector3.up * floatDistance, duration)
            .SetEase(Ease.OutQuad);
        tmp.DOFade(0f, duration)
            .SetDelay(duration * 0.5f)
            .OnComplete(() => PoolManager.Instance.Return(gameObject));
    }

    private static Color GetColor(int amount)
    {
        if (amount >= 500) return new Color(1f, 0.3f, 0.3f); // red - large
        if (amount >= 100) return new Color(1f, 0.6f, 0.1f); // orange - mid
        if (amount >= 25)  return new Color(1f, 1f, 0.2f);   // yellow - small
        return Color.white;                                    // white - survival
    }

    public void OnGetFromPool()  { }
    public void OnReturnToPool()
    {
        DOTween.Kill(transform);
        DOTween.Kill(tmp);
    }
}

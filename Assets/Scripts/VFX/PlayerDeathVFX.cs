using UnityEngine;
using DG.Tweening;

// Attach to Player. Spawns spinning fragments on final death.
public class PlayerDeathVFX : MonoBehaviour
{
    [SerializeField] private Sprite fragmentSprite;
    [SerializeField] private int    fragmentCount = 4;

    public void Play()
    {
        for (int i = 0; i < fragmentCount; i++)
        {
            var frag = new GameObject("Frag");
            frag.transform.position = transform.position;

            var sr     = frag.AddComponent<SpriteRenderer>();
            sr.sprite  = fragmentSprite != null ? fragmentSprite
                       : GetComponent<SpriteRenderer>()?.sprite;
            sr.sortingOrder = 10;

            float angle = i * (360f / fragmentCount);
            var   dir   = Quaternion.Euler(0, 0, angle) * Vector2.up;
            float dist  = Random.Range(1.5f, 3f);

            DOTween.Sequence()
                .Append(frag.transform.DOMove(transform.position + dir * dist, 0.8f).SetEase(Ease.OutQuad))
                .Join(frag.transform.DORotate(new Vector3(0, 0, Random.Range(180f, 540f)), 0.8f))
                .Join(sr.DOFade(0f, 0.8f).SetDelay(0.3f))
                .OnComplete(() => Destroy(frag));
        }
    }
}

using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ShadowSync : MonoBehaviour
{
    private SpriteRenderer shadowSr;
    private SpriteRenderer parentSr;

    private void Awake()
    {
        shadowSr = GetComponent<SpriteRenderer>();
        parentSr = transform.parent != null ? transform.parent.GetComponent<SpriteRenderer>() : null;
    }

    private void LateUpdate()
    {
        if (parentSr == null || shadowSr == null) return;
        if (shadowSr.sprite != parentSr.sprite)
            shadowSr.sprite = parentSr.sprite;
    }
}

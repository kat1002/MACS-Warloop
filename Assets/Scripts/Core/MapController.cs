using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MapController : MonoBehaviour
{
    private void Start()
    {
        var sprites = Resources.LoadAll<Sprite>("Maps");
        if (sprites == null || sprites.Length == 0)
        {
            Debug.LogWarning("MapController: no sprites found in Resources/Maps");
            return;
        }

        GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
    }
}

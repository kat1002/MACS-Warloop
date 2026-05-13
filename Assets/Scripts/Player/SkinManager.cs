using UnityEngine;

public class SkinManager : MonoBehaviour
{
    public static SkinManager Instance { get; private set; }

    private const string SkinKey = "SelectedSkin";

    [SerializeField] private SkinData[] skins;

    public SkinData[] Skins => skins;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (skins == null || skins.Length == 0)
            skins = Resources.LoadAll<SkinData>("Skins");
    }

    public int GetSelectedIndex()
        => skins != null ? Mathf.Clamp(PlayerPrefs.GetInt(SkinKey, 0), 0, skins.Length - 1) : 0;

    public void SetSelectedIndex(int index)
    {
        if (skins == null) return;
        PlayerPrefs.SetInt(SkinKey, Mathf.Clamp(index, 0, skins.Length - 1));
    }

    public SkinData GetSelected()
        => skins != null && skins.Length > 0 ? skins[GetSelectedIndex()] : null;

    public void ApplySprite(SpriteRenderer sr, int level)
    {
        if (sr == null) return;
        var skin   = GetSelected();
        var sprite = skin?.GetSprite(level);
        if (sprite != null) sr.sprite = sprite;
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class SkinSelectPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image           shipPreview;
    [SerializeField] private TextMeshProUGUI shipNameText;
    [SerializeField] private Button          leftBtn;
    [SerializeField] private Button          rightBtn;
    [SerializeField] private Button          selectBtn;
    [SerializeField] private Button          backBtn;
    [SerializeField] private Transform       dotsContainer;

    private int           currentIndex;
    private List<Image>   dots = new List<Image>();

    private void Awake()
    {
        BuildDots();

        leftBtn?.onClick.AddListener(() => Navigate(-1));
        rightBtn?.onClick.AddListener(() => Navigate(1));
        selectBtn?.onClick.AddListener(OnSelect);
        backBtn?.onClick.AddListener(Hide);
    }

    private void BuildDots()
    {
        var mgr   = SkinManager.Instance;
        int count = mgr != null && mgr.Skins != null ? mgr.Skins.Length : 8;
        for (int i = 0; i < count; i++)
        {
            var go  = new GameObject("Dot_" + i);
            go.transform.SetParent(dotsContainer, false);
            var img = go.AddComponent<Image>();
            img.rectTransform.sizeDelta = new Vector2(14f, 14f);
            dots.Add(img);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        currentIndex = SkinManager.Instance != null ? SkinManager.Instance.GetSelectedIndex() : 0;
        Refresh();
        transform.localScale = Vector3.zero;
        transform.DOScale(1f, 0.28f).SetEase(Ease.OutBack);
    }

    public void Hide()
    {
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.uiClick);
        transform.DOScale(0f, 0.2f).SetEase(Ease.InBack)
            .OnComplete(() => gameObject.SetActive(false));
    }

    private void Navigate(int dir)
    {
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.uiClick);
        var skins = SkinManager.Instance?.Skins;
        if (skins == null || skins.Length == 0) return;

        currentIndex = (currentIndex + dir + skins.Length) % skins.Length;
        shipPreview.transform.DOKill();
        shipPreview.transform.DOScale(0.7f, 0.07f).OnComplete(() =>
        {
            Refresh();
            shipPreview.transform.DOScale(1f, 0.15f).SetEase(Ease.OutBack);
        });
    }

    private void OnSelect()
    {
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.uiClick);
        SkinManager.Instance?.SetSelectedIndex(currentIndex);
        shipPreview.transform.DOKill();
        shipPreview.transform.localScale = Vector3.one;
        shipPreview.transform.DOScale(1.4f, 0.18f).SetEase(Ease.OutBack)
            .OnComplete(() => shipPreview.transform.DOScale(1f, 0.12f).SetEase(Ease.InOutQuad));
    }

    private void Refresh()
    {
        var skins = SkinManager.Instance?.Skins;
        if (skins == null || skins.Length == 0) return;

        var skin = skins[Mathf.Clamp(currentIndex, 0, skins.Length - 1)];

        if (shipPreview != null && skin.PreviewSprite != null)
        {
            shipPreview.sprite = skin.PreviewSprite;
            shipPreview.rectTransform.sizeDelta = new Vector2(160f, 160f);
        }

        if (shipNameText != null)
            shipNameText.text = skin.skinName;

        RefreshDots();
    }

    private void RefreshDots()
    {
        for (int i = 0; i < dots.Count; i++)
        {
            if (dots[i] == null) continue;
            dots[i].color = i == currentIndex
                ? Color.white
                : new Color(1f, 1f, 1f, 0.3f);
        }
    }
}

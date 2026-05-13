using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MainMenuController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SkinSelectPanel     skinSelectPanel;
    [SerializeField] private SettingsPanel       settingsPanel;
    [SerializeField] private TextMeshProUGUI     highScoreText;
    [SerializeField] private Button              playButton;
    [SerializeField] private Button              skinsButton;
    [SerializeField] private Button              settingsButton;

    [Header("Animation targets")]
    [SerializeField] private RectTransform       titleRect;
    [SerializeField] private CanvasGroup         buttonsGroup;

    private void Start()
    {
        int hs = PlayerPrefs.GetInt("HighScore", 0);
        if (highScoreText != null)
            highScoreText.text = hs > 0 ? $"BEST  {hs:N0}" : "";

        playButton?.onClick.AddListener(OnPlay);
        skinsButton?.onClick.AddListener(OnSkins);
        settingsButton?.onClick.AddListener(OnSettings);

        // Entrance
        if (titleRect != null)
        {
            titleRect.localScale = Vector3.zero;
            titleRect.DOScale(1f, 0.45f).SetEase(Ease.OutBack).SetDelay(0.1f);
        }
        if (buttonsGroup != null)
        {
            buttonsGroup.alpha = 0f;
            buttonsGroup.DOFade(1f, 0.35f).SetDelay(0.45f);
        }
    }

    private void OnPlay()
    {
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.uiClick);
        DOTween.KillAll();
        SceneManager.LoadScene("Game");
    }

    private void OnSkins()
    {
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.uiClick);
        skinSelectPanel?.Show();
    }

    private void OnSettings()
    {
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.uiClick);
        settingsPanel?.Show();
    }
}

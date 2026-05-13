using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI multiplierText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image[]         lifeIcons;

    [Header("Pause / Settings")]
    [SerializeField] private SettingsPanel   settingsPanel;
    [SerializeField] private Button          pauseButton;

    [Header("VFX")]
    [SerializeField] private Image           flashPanel;

    [Header("Floating Text")]
    [SerializeField] private GameObject      floatingTextPrefab;

    [Header("Game Over")]
    [SerializeField] private GameObject      gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private Button          restartButton;
    [SerializeField] private Button          gameOverQuitButton;
    [SerializeField] private Button          settingsQuitButton;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnEnable()
    {
        EventManager.OnScoreChanged      += HandleScoreChanged;
        EventManager.OnLivesChanged      += HandleLivesChanged;
        EventManager.OnMultiplierChanged += HandleMultiplierChanged;
        EventManager.OnGameOver          += HandleGameOver;
        EventManager.OnPauseChanged      += HandlePauseChanged;
        EventManager.OnFloatingScore     += HandleFloatingScore;
        EventManager.OnPowerUpCollected  += HandlePowerUpCollected;
    }

    private void OnDisable()
    {
        EventManager.OnScoreChanged      -= HandleScoreChanged;
        EventManager.OnLivesChanged      -= HandleLivesChanged;
        EventManager.OnMultiplierChanged -= HandleMultiplierChanged;
        EventManager.OnGameOver          -= HandleGameOver;
        EventManager.OnPauseChanged      -= HandlePauseChanged;
        EventManager.OnFloatingScore     -= HandleFloatingScore;
        EventManager.OnPowerUpCollected  -= HandlePowerUpCollected;
    }

    private void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        pauseButton?.onClick.AddListener(() =>
        {
            AudioManager.Instance?.PlaySFX(AudioManager.Instance.uiClick);
            GameManager.Instance?.TogglePause();
        });

        restartButton?.onClick.AddListener(() =>
        {
            AudioManager.Instance?.PlaySFX(AudioManager.Instance.uiClick);
            GameManager.Instance?.RestartGame();
        });

        gameOverQuitButton?.onClick.AddListener(() => { 
            AudioManager.Instance?.PlaySFX(AudioManager.Instance.uiClick);
            GameManager.Instance?.QuitGame();
        });
        settingsQuitButton?.onClick.AddListener(() => {
            AudioManager.Instance?.PlaySFX(AudioManager.Instance.uiClick);
            GameManager.Instance?.QuitGame();
        });
    }

    private void Update()
    {
        if (timerText == null || GameManager.Instance == null || GameManager.Instance.IsGameOver) return;
        float t = GameManager.Instance.SurvivalTime;
        int   m = (int)(t / 60f);
        int   s = (int)(t % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", m, s);
    }

    private void HandleScoreChanged(int score, int multiplier)
    {
        if (scoreText == null) return;
        scoreText.text = score.ToString("N0");
        scoreText.transform.DOKill();
        scoreText.transform.localScale = Vector3.one;
        scoreText.transform.DOPunchScale(Vector3.one * 0.25f, 0.2f, 5, 0.5f);

        if (multiplierText != null)
            multiplierText.text = multiplier > 1 ? $"x{multiplier}" : "";
    }

    private void HandleLivesChanged(int lives)
    {
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            bool shouldBeActive = i < lives;
            if (!shouldBeActive && lifeIcons[i].gameObject.activeSelf)
            {
                var icon = lifeIcons[i];
                icon.transform.DOShakePosition(0.3f, 10f, 15)
                    .OnComplete(() => icon.gameObject.SetActive(false));
            }
            else if (shouldBeActive)
            {
                lifeIcons[i].gameObject.SetActive(true);
            }
        }
    }

    private void HandleMultiplierChanged(int multiplier)
    {
        if (multiplierText == null || multiplier <= 1) return;
        multiplierText.transform.DOKill();
        multiplierText.transform.localScale = Vector3.one;
        multiplierText.transform.DOPunchScale(Vector3.one * 0.5f, 0.3f, 8, 0.5f);
    }

    private void HandleGameOver(int score, int highScore)
    {
        if (gameOverPanel == null) return;
        gameOverPanel.SetActive(true);
        if (finalScoreText != null) finalScoreText.text = $"Score: {score:N0}";
        if (highScoreText  != null) highScoreText.text  = $"Best:  {highScore:N0}";

        gameOverPanel.transform.localScale = Vector3.zero;
        gameOverPanel.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
    }

    private void HandlePauseChanged(bool isPaused)
    {
        if (isPaused) settingsPanel?.Show();
        else          settingsPanel?.Hide();
    }

    private void HandleFloatingScore(int amount, Vector3 worldPos)
    {
        if (floatingTextPrefab == null) return;
        PoolManager.Instance.Get<FloatingText>(floatingTextPrefab)?.Show(amount, worldPos);
    }

    private void HandlePowerUpCollected()
    {
        if (flashPanel == null) return;
        flashPanel.DOKill();
        flashPanel.color = new Color(0.4f, 0.85f, 1f, 0.55f);
        flashPanel.DOFade(0f, 0.35f).SetEase(Ease.OutQuad).SetUpdate(true);
    }


}

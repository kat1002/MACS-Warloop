using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private int startingLives = 3;

    public int   Score        { get; private set; }
    public int   HighScore    { get; private set; }
    public int   Lives        { get; private set; }
    public int   Multiplier   { get; private set; } = 1;
    public bool  IsGameOver   { get; private set; }
    public bool  IsPaused     { get; private set; }
    public float SurvivalTime { get; private set; }

    private float noDamageTimer;
    private float survivalTimer;

    private const string HighScoreKey     = "HighScore";
    private const float  SurvivalInterval = 5f;
    private const int    SurvivalPoints   = 10;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        Lives     = startingLives;
        HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        ScreenBounds.Calculate(Camera.main);

        EventManager.EmitLivesChanged(Lives);
        EventManager.EmitScoreChanged(Score, Multiplier);

        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (playerPrefab == null) return;

        var spawnPos  = new Vector3(0f, ScreenBounds.MinY - 2.5f, -2f);
        var targetPos = new Vector3(0f, ScreenBounds.MinY + 1.8f,  -2f);

        var go = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        go.transform.localScale = new Vector3(0.3f, 0.3f, 1f);

        var controller = go.GetComponent<PlayerController>();
        var shooter    = go.GetComponent<PlayerShooter>();
        var col        = go.GetComponent<Collider2D>();
        if (controller != null) controller.enabled = false;
        if (shooter    != null) shooter.enabled    = false;
        if (col        != null) col.enabled        = false;

        go.transform.DOScale(Vector3.one, 0.55f).SetEase(Ease.OutBack).SetDelay(0.1f);
        go.transform.DOMoveY(targetPos.y, 1.1f).SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                if (controller != null) controller.enabled = true;
                if (shooter    != null) shooter.enabled    = true;
                if (col        != null) col.enabled        = true;
            });
    }

    private void OnDestroy() => EventManager.ClearAll();

    private void Update()
    {
        if (IsGameOver) return;

        SurvivalTime  += Time.deltaTime;
        survivalTimer += Time.deltaTime;
        if (survivalTimer >= SurvivalInterval)
        {
            survivalTimer -= SurvivalInterval;
            int earned = SurvivalPoints * Multiplier;
            Score += earned;
            SaveHighScore();
            EventManager.EmitFloatingScore(earned, Vector3.zero);
            EventManager.EmitScoreChanged(Score, Multiplier);
        }

        noDamageTimer += Time.deltaTime;
        RefreshMultiplier();
    }

    private void RefreshMultiplier()
    {
        int next = noDamageTimer >= 30f ? 5
                 : noDamageTimer >= 10f ? 4
                 : noDamageTimer >=  5f ? 2 : 1;

        if (next == Multiplier) return;
        Multiplier = next;
        EventManager.EmitMultiplierChanged(Multiplier);
        EventManager.EmitScoreChanged(Score, Multiplier);
    }

    public void AddScore(int basePoints, Vector3 worldPos)
    {
        int earned = basePoints * Multiplier;
        Score += earned;
        SaveHighScore();
        EventManager.EmitFloatingScore(earned, worldPos);
        EventManager.EmitScoreChanged(Score, Multiplier);
    }

    public void OnPlayerDamaged()
    {
        noDamageTimer = 0f;
        Multiplier    = 1;
        EventManager.EmitPlayerDamaged();
        EventManager.EmitMultiplierChanged(Multiplier);
        EventManager.EmitScoreChanged(Score, Multiplier);
    }

    public void LoseLife()
    {
        if (IsGameOver) return;
        Lives = Mathf.Max(0, Lives - 1);
        OnPlayerDamaged();
        EventManager.EmitLivesChanged(Lives);
        if (Lives <= 0) TriggerGameOver();
    }

    private void TriggerGameOver()
    {
        IsGameOver = true;
        EventManager.EmitGameOver(Score, HighScore);
    }

    private void SaveHighScore()
    {
        if (Score <= HighScore) return;
        HighScore = Score;
        PlayerPrefs.SetInt(HighScoreKey, HighScore);
    }

    public void TogglePause()
    {
        if (IsGameOver) return;
        IsPaused = !IsPaused;
        Time.timeScale = IsPaused ? 0f : 1f;
        EventManager.EmitPauseChanged(IsPaused);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        EventManager.EmitGameRestart();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        EventManager.EmitGameQuit();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}

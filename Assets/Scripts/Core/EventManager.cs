using UnityEngine;

/// <summary>
/// Central event bus. All game systems publish and subscribe here.
/// Static events — no instance reference required.
/// </summary>
public static class EventManager
{
    // ── Score & Multiplier ─────────────────────────────────────────
    public static event System.Action<int, int>     OnScoreChanged;       // score, multiplier
    public static event System.Action<int>          OnMultiplierChanged;  // new multiplier
    public static event System.Action<int, Vector3> OnFloatingScore;      // amount, worldPos

    // ── Lives ──────────────────────────────────────────────────────
    public static event System.Action<int>          OnLivesChanged;       // remaining lives
    public static event System.Action               OnPlayerDamaged;

    // ── Game State ─────────────────────────────────────────────────
    public static event System.Action<int, int>     OnGameOver;           // score, highScore
    public static event System.Action               OnGameRestart;
    public static event System.Action               OnGameQuit;

    // ── Enemies ───────────────────────────────────────────────────
    public static event System.Action<int, Vector3> OnEnemyDefeated;      // points, worldPos
    public static event System.Action<WaveManager.Difficulty> OnWaveTierChanged;

    // ── PowerUp ───────────────────────────────────────────────────
    public static event System.Action               OnPowerUpCollected;

    // ── Pause ─────────────────────────────────────────────────────
    public static event System.Action<bool>         OnPauseChanged;       // isPaused

    // ── Skin ──────────────────────────────────────────────────────
    public static event System.Action<int>          OnPlayerSkinChanged;  // skin index

    // ── Emit helpers ──────────────────────────────────────────────
    public static void EmitScoreChanged(int score, int multiplier)
        => OnScoreChanged?.Invoke(score, multiplier);

    public static void EmitMultiplierChanged(int multiplier)
        => OnMultiplierChanged?.Invoke(multiplier);

    public static void EmitFloatingScore(int amount, Vector3 worldPos)
        => OnFloatingScore?.Invoke(amount, worldPos);

    public static void EmitLivesChanged(int lives)
        => OnLivesChanged?.Invoke(lives);

    public static void EmitPlayerDamaged()
        => OnPlayerDamaged?.Invoke();

    public static void EmitGameOver(int score, int highScore)
        => OnGameOver?.Invoke(score, highScore);

    public static void EmitGameRestart()
        => OnGameRestart?.Invoke();

    public static void EmitGameQuit()
        => OnGameQuit?.Invoke();

    public static void EmitEnemyDefeated(int points, Vector3 worldPos)
        => OnEnemyDefeated?.Invoke(points, worldPos);

    public static void EmitWaveTierChanged(WaveManager.Difficulty tier)
        => OnWaveTierChanged?.Invoke(tier);

    public static void EmitPowerUpCollected()
        => OnPowerUpCollected?.Invoke();

    public static void EmitPauseChanged(bool isPaused)
        => OnPauseChanged?.Invoke(isPaused);

    public static void EmitPlayerSkinChanged(int index)
        => OnPlayerSkinChanged?.Invoke(index);

    public static void ClearAll()
    {
        OnScoreChanged      = null;
        OnMultiplierChanged = null;
        OnFloatingScore     = null;
        OnLivesChanged      = null;
        OnPlayerDamaged     = null;
        OnGameOver          = null;
        OnGameRestart       = null;
        OnGameQuit          = null;
        OnEnemyDefeated     = null;
        OnWaveTierChanged   = null;
        OnPowerUpCollected  = null;
        OnPauseChanged      = null;
        OnPlayerSkinChanged = null;
    }
}

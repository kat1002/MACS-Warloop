using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    public enum Difficulty { Easy, Medium, Hard }

    [System.Serializable]
    public class EnemyTier
    {
        public GameObject small;
        public GameObject mid;
        public GameObject large;
    }

    [Header("Enemy Prefabs")]
    [SerializeField] private EnemyTier enemies;

    private WaveData[] waves;

    [Header("Timing")]
    [SerializeField] private float waveInterval    = 4f;
    [SerializeField] private float tierUpgradeTime = 30f;

    [Header("Spawn")]
    [SerializeField] private float spawnY = 1.5f;

    public Difficulty CurrentDifficulty { get; private set; } = Difficulty.Easy;

    private float difficultyTimer;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        waves = Resources.LoadAll<WaveData>("Waves");
    }

    private void Start() => StartCoroutine(SpawnLoop());

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.IsGameOver) return;
        difficultyTimer += Time.deltaTime;

        Difficulty next = difficultyTimer >= tierUpgradeTime * 2 ? Difficulty.Hard
                        : difficultyTimer >= tierUpgradeTime     ? Difficulty.Medium
                        : Difficulty.Easy;

        if (next != CurrentDifficulty)
        {
            CurrentDifficulty = next;
            EventManager.EmitWaveTierChanged(CurrentDifficulty);
        }
    }

    private IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(2f);
        while (true)
        {
            if (GameManager.Instance != null && !GameManager.Instance.IsGameOver)
                SpawnWave();
            yield return new WaitForSeconds(waveInterval);
        }
    }

    private void SpawnWave()
    {
        var wave = PickWave();
        if (wave == null || wave.entries == null || wave.entries.Length == 0) return;

        float halfWidth = wave.fullWidth * 0.5f;
        float centerX   = Random.Range(ScreenBounds.MinX + halfWidth, ScreenBounds.MaxX - halfWidth);
        float spawnYPos = ScreenBounds.MaxY + spawnY;

        foreach (var entry in wave.entries)
        {
            var   prefab  = GetPrefab(entry.type);
            if (prefab == null) continue;
            float size    = GetEnemySize(entry.type);
            float targetX = centerX + entry.position.x;

            if (targetX - size < ScreenBounds.MinX || targetX + size > ScreenBounds.MaxX) continue;

            Vector3 pos = new Vector3(targetX, spawnYPos - entry.position.y, -2f);

            if (Physics2D.OverlapCircle(pos, size * 1.1f, LayerMask.GetMask("Enemy")) != null) continue;

            var e = PoolManager.Instance.Get(prefab);
            e.transform.position = pos;
            Physics2D.SyncTransforms();
        }
    }

    private WaveData PickWave()
    {
        var pool = new List<WaveData>();
        foreach (var w in waves)
            if (w != null && w.difficulty == CurrentDifficulty)
                pool.Add(w);

        // fallback: any wave if none match current difficulty
        if (pool.Count == 0)
            foreach (var w in waves)
                if (w != null) pool.Add(w);

        return pool.Count > 0 ? pool[Random.Range(0, pool.Count)] : null;
    }

    private GameObject GetPrefab(WaveData.EnemyType type) => type switch
    {
        WaveData.EnemyType.Mid   => enemies.mid,
        WaveData.EnemyType.Large => enemies.large,
        _                        => enemies.small,
    };

    private float GetEnemySize(WaveData.EnemyType type) => type switch
    {
        WaveData.EnemyType.Mid   => 0.7f,
        WaveData.EnemyType.Large => 1.0f,
        _                        => 0.5f,
    };
}

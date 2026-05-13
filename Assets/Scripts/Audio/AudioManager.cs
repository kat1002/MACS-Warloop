using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips")]
    public AudioClip menuMusic;
    public AudioClip gameplayMusic;
    public AudioClip playerShoot;
    public AudioClip playerHit;
    public AudioClip enemyHit;
    public AudioClip enemyDeath;
    public AudioClip powerupCollect;
    public AudioClip gameOver;
    public AudioClip uiClick;

    private const string MasterVolKey = "VolMaster";
    private const string MusicVolKey  = "VolMusic";
    private const string SfxVolKey    = "VolSFX";

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        ApplyVolumes();
        SceneManager.sceneLoaded += OnSceneLoaded;
        SubscribeEvents();
        PlaySceneMusic(SceneManager.GetActiveScene());
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        UnsubscribeEvents();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UnsubscribeEvents();
        SubscribeEvents();
        PlaySceneMusic(scene);
    }

    private void PlaySceneMusic(Scene scene)
    {
        if (scene.buildIndex == 0) PlayMusic(menuMusic);
        else if (scene.buildIndex == 1) PlayMusic(gameplayMusic);
    }

    // Named handlers so we can properly unsubscribe
    private void Handle_PlayerDamaged()                  => PlaySFX(playerHit);
    private void Handle_EnemyDefeated(int _, Vector3 __) => PlaySFX(enemyDeath);
    private void Handle_PowerUpCollected()               => PlaySFX(powerupCollect);
    private void Handle_GameOver(int _, int __)          { StopMusic(); PlaySFX(gameOver); }
    private void Handle_GameRestart()                    => PlayMusic(gameplayMusic);
    private void Handle_GameQuit()                       => StopMusic();

    private void SubscribeEvents()
    {
        EventManager.OnPlayerDamaged    += Handle_PlayerDamaged;
        EventManager.OnEnemyDefeated    += Handle_EnemyDefeated;
        EventManager.OnPowerUpCollected += Handle_PowerUpCollected;
        EventManager.OnGameOver         += Handle_GameOver;
        EventManager.OnGameRestart      += Handle_GameRestart;
        EventManager.OnGameQuit         += Handle_GameQuit;
    }

    private void UnsubscribeEvents()
    {
        EventManager.OnPlayerDamaged    -= Handle_PlayerDamaged;
        EventManager.OnEnemyDefeated    -= Handle_EnemyDefeated;
        EventManager.OnPowerUpCollected -= Handle_PowerUpCollected;
        EventManager.OnGameOver         -= Handle_GameOver;
        EventManager.OnGameRestart      -= Handle_GameRestart;
        EventManager.OnGameQuit         -= Handle_GameQuit;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic() => musicSource.Stop();

    public void SetMasterVolume(float v)
    {
        v = Mathf.Clamp01(v);
        AudioListener.volume = v;
        PlayerPrefs.SetFloat(MasterVolKey, v);
    }

    public void SetMusicVolume(float v)
    {
        v = Mathf.Clamp01(v);
        musicSource.volume = v;
        PlayerPrefs.SetFloat(MusicVolKey, v);
    }

    public void SetSFXVolume(float v)
    {
        v = Mathf.Clamp01(v);
        sfxSource.volume = v;
        PlayerPrefs.SetFloat(SfxVolKey, v);
    }

    public float GetMasterVolume() => PlayerPrefs.GetFloat(MasterVolKey, 1f);
    public float GetMusicVolume()  => PlayerPrefs.GetFloat(MusicVolKey,  1f);
    public float GetSFXVolume()    => PlayerPrefs.GetFloat(SfxVolKey,    1f);

    private void ApplyVolumes()
    {
        AudioListener.volume = GetMasterVolume();
        musicSource.volume   = GetMusicVolume();
        sfxSource.volume     = GetSFXVolume();
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class SettingsPanel : MonoBehaviour
{
    [Header("Master")]
    [SerializeField] private Slider             masterSlider;
    [SerializeField] private TextMeshProUGUI    masterValueText;
    [SerializeField] private Button             masterMinus;
    [SerializeField] private Button             masterPlus;

    [Header("Music")]
    [SerializeField] private Slider             musicSlider;
    [SerializeField] private TextMeshProUGUI    musicValueText;
    [SerializeField] private Button             musicMinus;
    [SerializeField] private Button             musicPlus;

    [Header("SFX")]
    [SerializeField] private Slider             sfxSlider;
    [SerializeField] private TextMeshProUGUI    sfxValueText;
    [SerializeField] private Button             sfxMinus;
    [SerializeField] private Button             sfxPlus;

    [Header("Navigation")]
    [SerializeField] private Button             backBtn;

    private const float Step = 0.1f;

    private void Awake()
    {
        masterSlider?.onValueChanged.AddListener(v => ApplyMaster(v));
        musicSlider?.onValueChanged.AddListener(v  => ApplyMusic(v));
        sfxSlider?.onValueChanged.AddListener(v    => ApplySFX(v));

        masterMinus?.onClick.AddListener(() => StepSlider(masterSlider, -Step));
        masterPlus?.onClick.AddListener(()  => StepSlider(masterSlider,  Step));
        musicMinus?.onClick.AddListener(()  => StepSlider(musicSlider,  -Step));
        musicPlus?.onClick.AddListener(()   => StepSlider(musicSlider,   Step));
        sfxMinus?.onClick.AddListener(()    => StepSlider(sfxSlider,    -Step));
        sfxPlus?.onClick.AddListener(()     => StepSlider(sfxSlider,     Step));
        backBtn?.onClick.AddListener(Hide);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        RefreshFromAudioManager();
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    public void Hide()
    {
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.uiClick);
        transform.DOScale(Vector3.zero, 0.18f).SetEase(Ease.InBack).SetUpdate(true)
            .OnComplete(() => gameObject.SetActive(false));
    }

    private void RefreshFromAudioManager()
    {
        var am = AudioManager.Instance;
        if (am == null) return;

        if (masterSlider != null) { masterSlider.SetValueWithoutNotify(am.GetMasterVolume()); UpdateLabel(masterValueText, am.GetMasterVolume()); }
        if (musicSlider  != null) { musicSlider.SetValueWithoutNotify(am.GetMusicVolume());   UpdateLabel(musicValueText,  am.GetMusicVolume()); }
        if (sfxSlider    != null) { sfxSlider.SetValueWithoutNotify(am.GetSFXVolume());       UpdateLabel(sfxValueText,    am.GetSFXVolume()); }
    }

    private void StepSlider(Slider slider, float delta)
    {
        if (slider == null) return;
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.uiClick);
        slider.value = Mathf.Clamp01(slider.value + delta);
    }

    private void ApplyMaster(float v)
    {
        AudioManager.Instance?.SetMasterVolume(v);
        UpdateLabel(masterValueText, v);
    }

    private void ApplyMusic(float v)
    {
        AudioManager.Instance?.SetMusicVolume(v);
        UpdateLabel(musicValueText, v);
    }

    private void ApplySFX(float v)
    {
        AudioManager.Instance?.SetSFXVolume(v);
        UpdateLabel(sfxValueText, v);
    }

    private void UpdateLabel(TextMeshProUGUI label, float v)
    {
        if (label != null) label.text = Mathf.RoundToInt(v * 100f) + "%";
    }
}

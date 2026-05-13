using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class VignetteController : MonoBehaviour
{
    [SerializeField] private Color vignetteColor = new Color(0.08f, 0f, 0.18f);

    private Volume   volume;
    private Vignette vignette;
    private Tween    tween;

    // Index = multiplier-1:  x1=0  x2=0.18  x4=0.30  x5=0.42
    private static readonly float[] Intensities = { 0f, 0.18f, 0.18f, 0.30f, 0.42f };

    private void Start()
    {
        volume = GetComponent<Volume>();
        if (volume == null)
        {
            volume          = gameObject.AddComponent<Volume>();
            volume.isGlobal = true;
            volume.priority = 5f;
        }

        var profile = ScriptableObject.CreateInstance<VolumeProfile>();
        volume.profile = profile;

        vignette = profile.Add<Vignette>(true);
        vignette.color.Override(vignetteColor);
        vignette.intensity.Override(0f);
        vignette.smoothness.Override(0.4f);
    }

    private void OnEnable()  => EventManager.OnMultiplierChanged += OnMultiplierChanged;
    private void OnDisable() => EventManager.OnMultiplierChanged -= OnMultiplierChanged;

    private void OnMultiplierChanged(int multiplier)
    {
        if (vignette == null) return;
        int   idx    = Mathf.Clamp(multiplier - 1, 0, Intensities.Length - 1);
        float target = Intensities[idx];

        tween?.Kill();
        tween = DOTween.To(
            () => vignette.intensity.value,
            x  => vignette.intensity.Override(x),
            target, 0.5f);
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/*-----------------------------------------------------------------------------------------------------------
Ten kod:
Odpowiada za zarz¹dzanie muzyk¹ i efektami dŸwiêkowymi (SFX).
----------------------------------------------------------------------------------------------------------- */

public class AudioManager : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider sfxSlider, musicSlider;
    const string MIXER_SFX = "SFX";
    const string MIXER_MUSIC = "Music";

    [SerializeField] private TextMeshProUGUI sfxPercent;
    [SerializeField] private TextMeshProUGUI musicPercent;

    private void Awake()
    {
        // Rejestracja zdarzeñ dla suwaków
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
    }

    private void Start()
    {

        musicSlider.value = PlayerPrefs.GetFloat(MIXER_MUSIC, 1f);
        sfxSlider.value = PlayerPrefs.GetFloat(MIXER_SFX, 1f);


        UpdateVolumeTexts();
    }

    private void SetMusicVolume(float value)
    {
        mixer.SetFloat(MIXER_MUSIC, Mathf.Log10(value) * 20);
        UpdateVolumeTexts();
    }

    private void SetSFXVolume(float value)
    {
        mixer.SetFloat(MIXER_SFX, Mathf.Log10(value) * 20);
        UpdateVolumeTexts();
    }

    private void OnDisable()
    {

        PlayerPrefs.SetFloat(MIXER_MUSIC, musicSlider.value);
        PlayerPrefs.SetFloat(MIXER_SFX, sfxSlider.value);
    }


    private void UpdateVolumeTexts()
    {
        sfxPercent.text = $"{(sfxSlider.value * 100):F0}%";
        musicPercent.text = $"{(musicSlider.value * 100):F0}%";
    }
}
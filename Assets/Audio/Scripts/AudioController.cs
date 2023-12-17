using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;


    private void Start()
    {
        HandleInitialValues();

        masterVolumeSlider.onValueChanged.AddListener(HandleMasterVolumeChanged);
        musicVolumeSlider.onValueChanged.AddListener(HandleMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(HandleSfxVolumeChanged);
    }

    private void HandleInitialValues()
    {
        float persistedMasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float persistedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float persistedSfxVolume = PlayerPrefs.GetFloat("SoundEffectsVolume", 1f);

        masterVolumeSlider.value = persistedMasterVolume;
        musicVolumeSlider.value = persistedMusicVolume;
        sfxVolumeSlider.value = persistedSfxVolume;

        HandleMasterVolumeChanged(persistedMasterVolume);
        HandleMusicVolumeChanged(persistedMusicVolume);
        HandleSfxVolumeChanged(persistedSfxVolume);
    }

    private void HandleMasterVolumeChanged(float volume)
    {
        PlayerPrefs.SetFloat("MasterVolume", volume);
        audioMixer.SetFloat("MasterVolume", LinearToDecibel(volume));
    }

    private void HandleMusicVolumeChanged(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        audioMixer.SetFloat("MusicVolume", LinearToDecibel(volume));
    }

    private void HandleSfxVolumeChanged(float volume)
    {
        PlayerPrefs.SetFloat("SoundEffectsVolume", volume);
        audioMixer.SetFloat("SoundEffectsVolume", LinearToDecibel(volume));
    }

    private float LinearToDecibel(float linear)
    {
        if (linear <= 0) return -80;

        return Mathf.Log10(linear) * 20;
    }
}

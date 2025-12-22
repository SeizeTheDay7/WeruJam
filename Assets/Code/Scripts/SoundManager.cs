using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] AudioMixerGroup bgm;
    [SerializeField] AudioMixerGroup sfx;
    [SerializeField] AudioMixerGroup scream;

    void Start()
    {
        float bgmVolume = PlayerPrefs.GetFloat("bgmVolume", 1f);
        bgm.audioMixer.SetFloat("bgmVolume", Mathf.Log10(bgmVolume) * 20);

        float sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 1f);
        sfx.audioMixer.SetFloat("sfxVolume", Mathf.Log10(sfxVolume) * 20);
        scream.audioMixer.SetFloat("screamVolume", Mathf.Log10(sfxVolume) * 20);
    }

    public void SetBGMVolumeFromUI(Slider slider)
    {
        bgm.audioMixer.SetFloat("bgmVolume", Mathf.Log10(slider.value) * 20);
        PlayerPrefs.SetFloat("bgmVolume", slider.value);
    }

    public void SetSFXVolumeFromUI(Slider slider)
    {
        sfx.audioMixer.SetFloat("sfxVolume", Mathf.Log10(slider.value) * 20);
        scream.audioMixer.SetFloat("screamVolume", Mathf.Log10(slider.value) * 20);
        PlayerPrefs.SetFloat("sfxVolume", slider.value);
    }

    public void MuteAllExceptScream()
    {
        bgm.audioMixer.SetFloat("bgmVolume", -80f);
        sfx.audioMixer.SetFloat("sfxVolume", -80f);
    }
}
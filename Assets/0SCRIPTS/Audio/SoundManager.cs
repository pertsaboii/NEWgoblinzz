using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public AudioMixer masterMixer;
    [HideInInspector] public float SFXVol, UIVol, MasterVol, MusicVol;

    [SerializeField] private AudioSource UISounds, SFXSounds;
    public AudioSource musicSounds;
    public AudioSource musicSounds2;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
    private void Start()
    {
        UISounds.ignoreListenerPause = true;
        musicSounds.volume = 1;
        LoadAudioValues();
    }
    public void LoadAudioValues()
    {
        ChangeSFXVolume(SFXVol);
        ChangeMusicVolume(MusicVol);
        ChangeUIVolume(UIVol);
        ChangeMasterVolume(MasterVol);
    }
    // äänien toisto
    public void PlaySFXSound(AudioClip clip)
    {
        SFXSounds.PlayOneShot(clip);
    }
    public void PlayUISound(AudioClip clip)
    {
        UISounds.PlayOneShot(clip);
    }
    public void PlayMusicSound(AudioClip clip, int musicSoundSource)
    {
        if (musicSoundSource == 0) musicSounds.PlayOneShot(clip);
        else musicSounds2.PlayOneShot(clip);
    }

    // voluumien säätö
    public void ChangeSFXVolume(float value)
    {
        masterMixer.SetFloat("SFXVol", value);
        SFXVol = value;
    }
    public void ChangeMasterVolume(float value)
    {
        masterMixer.SetFloat("MasterVol", value);
        MasterVol = value;
    }
    public void ChangeUIVolume(float value)
    {
        masterMixer.SetFloat("UIVol", value);
        UIVol = value;
    }
    public void ChangeMusicVolume(float value)
    {
        masterMixer.SetFloat("MusicVol", value);
        MusicVol = value;
    }
    public void FadeMusic(float fadeTime, bool soundIncrease, int musicSource)
    {
        if (soundIncrease == false) StartCoroutine(MusicFadeOut(fadeTime, musicSource));
        else StartCoroutine(MusicFadeIn(fadeTime, musicSource));
    }
    IEnumerator MusicFadeOut(float fadeTime, int musicSource)
    {
        AudioSource source = musicSounds;
        if (musicSource == 1) source = musicSounds2;
        float fadeMult = fadeTime;
        while (fadeTime >= 0)
        {
            source.volume -= .05f / fadeMult;
            fadeTime -= .05f;
            if (source.volume <= 0.08f)
            {
                source.Stop();
                break;
            }
            yield return new WaitForSecondsRealtime(.05f);
        }
    }
    IEnumerator MusicFadeIn(float fadeTime, int musicSource)
    {
        musicSounds.volume = 0;
        float fadeMult = fadeTime;
        while (fadeTime >= 0)
        {
            musicSounds.volume += .05f / fadeMult;
            fadeTime -= .05f;
            yield return new WaitForSeconds(.05f);
        }
        musicSounds.volume = 1;
    }
    public IEnumerator DayFade()
    {
        gamemanager.musicPlayer.PlayMusic();

        musicSounds.volume = 0;
        float fadeTime = 3;
        while (fadeTime >= 0)
        {
            musicSounds.volume += .05f / 3;
            musicSounds2.volume -= .08f / 3;
            fadeTime -= .05f;
            if (musicSounds.volume >= 1) break;
            yield return new WaitForSeconds(.05f);
        }
        musicSounds.volume = 1;
        musicSounds2.Stop();
        musicSounds2.volume = 1;
    }
}

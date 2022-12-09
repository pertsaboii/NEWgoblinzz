using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    /*
    OHJE:
    - tämä script toistaa taustamusiikkia/ambienssia
    - toimii myös pelkällä looppaavalla klipillä, jos on intro erikseen niin toistaa ensin intron kerran ja sen jälkeen alkaa looppaamaan looppia
    */

    [SerializeField] private AudioClip musicIntroClip;
    [SerializeField] private AudioClip musicLoopClip;
    [SerializeField] private AudioClip secondaryMusicIntroClip;
    [SerializeField] private AudioClip secondaryMusicLoopClip;

    private void Awake()
    {
        SoundManager.Instance.musicSounds.clip = musicLoopClip;
        PlayMusic();
    }

    // sceneen kuuluvan musiikin toisto
    public void PlayMusic()
    {
        SoundManager.Instance.musicSounds.volume = 1;

        if (musicIntroClip != null)
        {
            SoundManager.Instance.musicSounds.PlayOneShot(musicIntroClip);
            SoundManager.Instance.musicSounds.PlayScheduled(AudioSettings.dspTime + musicIntroClip.length);
            Debug.Log(AudioSettings.dspTime);
        }
        else if (musicLoopClip != null) SoundManager.Instance.musicSounds.Play();
    }
    public void PlaySecondaryMusic()
    {
        SoundManager.Instance.musicSounds2.clip = secondaryMusicLoopClip;

        if (secondaryMusicIntroClip != null)
        {
            SoundManager.Instance.musicSounds2.PlayOneShot(secondaryMusicIntroClip);
            SoundManager.Instance.musicSounds2.PlayScheduled(AudioSettings.dspTime + secondaryMusicIntroClip.length);
        }
        else if (secondaryMusicLoopClip != null) SoundManager.Instance.musicSounds2.Play();
    }
}

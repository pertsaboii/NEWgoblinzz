using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    private Slider slider;
    [SerializeField] private bool UIVolumeSlider, masterVolumeSlider, SFXVolumeSlider, musicVolumeSlider;
    private void OnEnable()
    {
        slider = GetComponent<Slider>();

        if (masterVolumeSlider)
        {
            slider.onValueChanged.AddListener(value => SoundManager.Instance.ChangeMasterVolume(value));
            slider.value = SoundManager.Instance.MasterVol;
            //SoundManager.Instance.ChangeMasterVolume(slider.value);
        }
        else if (UIVolumeSlider)
        {
            slider.onValueChanged.AddListener(value => SoundManager.Instance.ChangeUIVolume(value));
            slider.value = SoundManager.Instance.UIVol;
            //SoundManager.Instance.ChangeUIVolume(slider.value);
        }
        else if (SFXVolumeSlider)
        {
            slider.onValueChanged.AddListener(value => SoundManager.Instance.ChangeSFXVolume(value));
            slider.value = SoundManager.Instance.SFXVol;
            //SoundManager.Instance.ChangeSFXVolume(slider.value);
        }
        else if (musicVolumeSlider)
        {
            slider.onValueChanged.AddListener(value => SoundManager.Instance.ChangeMusicVolume(value));
            slider.value = SoundManager.Instance.MusicVol;
            //SoundManager.Instance.ChangeMusicVolume(slider.value);
        }
    }
}

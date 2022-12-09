using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Video;

public class InfoPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text cardName;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private VideoPlayer videoPlayer;

    public void InfoPanelOn(string name, string text, VideoClip clip)
    {
        SoundManager.Instance.PlayUISound(gamemanager.assetBank.FindSound(AssetBank.Sound.ButtonClicked));
        gameObject.transform.DOScale(Vector3.one, .3f).SetEase(Ease.OutSine);
        cardName.text = name;
        descriptionText.text = text;
        videoPlayer.clip = clip;
        videoPlayer.Play();
    }
    public void InfoPanelOff()
    {
        videoPlayer.Stop();
        gameObject.transform.DOScale(Vector3.zero, .3f).SetEase(Ease.OutSine);
    }

}

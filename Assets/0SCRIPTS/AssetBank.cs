using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetBank : MonoBehaviour
{
    public enum FXType
    {
        BatteringRamHit,
        GobboUnitDeath,
        BuildingDeath,
        BasicEnemyDeath,
        WanderingDeath,
        TrollUnitDeath,
        GroundDustWave
    }

    public enum Sound
    {
        ResourcePlusOne,
        CardSelected,
        InsufficientFunds,
        ButtonClicked,
        NewCardDrawn,
        MoneyGained,
        GameStartedJingle,
        NewHighScoreJingle
    }

    public FX[] FXArray;
    public SoundType[] SoundArray;

    [System.Serializable]
    public class FX
    {
        public ParticleSystem particleSystem;
        public FXType fXType;
    }
    [System.Serializable]
    public class SoundType
    {
        public AudioClip audioClip;
        public Sound sound;
    }
    public ParticleSystem FindFX(FXType type)
    {
        foreach (FX fx in FXArray)
        {
            if (fx.fXType == type)
            {
                return fx.particleSystem;
            }
        }
        return null;
    }
    public AudioClip FindSound(Sound sound)
    {
        foreach (SoundType soundType in SoundArray)
        {
            if (soundType.sound == sound)
            {
                return soundType.audioClip;
            }
        }
        return null;
    }



}

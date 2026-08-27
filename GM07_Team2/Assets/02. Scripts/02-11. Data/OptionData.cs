using System;

using UnityEngine;

[Serializable]
public class OptionData
{
    // Audio Field
    [SerializeField]
    private float _masterVolume;
    [SerializeField]
    private float _bgmVolume;
    [SerializeField]
    private float _sfxVolume;

    // Audio Property
    public float MasterVolume => _masterVolume;
    public float BGMVolume => _bgmVolume;
    public float SFXVolume => _sfxVolume;

    public OptionData(float masterVolume, float bgmVolume, float sfxVolume)
    {
        _masterVolume = masterVolume;
        _bgmVolume = bgmVolume;
        _sfxVolume = sfxVolume;
    }
}

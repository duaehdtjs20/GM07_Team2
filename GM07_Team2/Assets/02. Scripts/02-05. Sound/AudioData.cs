using System;

using UnityEngine;

[Serializable]
public class AudioData
{
    [SerializeField]
    private AudioClip _clip;
    [SerializeField]
    private EAudioType _audioType;
    [SerializeField]
    private float _volume = 1f;
    [SerializeField]
    private float _pitch = 1f;

    public AudioClip Clip => _clip;
    public EAudioType AudioType => _audioType;
    public float Volume => _volume;
    public float Pitch => _pitch;
}

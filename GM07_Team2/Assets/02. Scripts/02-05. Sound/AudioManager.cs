using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviourSingleton<AudioManager>
{
    [SerializeField]
    private AudioDataBase _database;

    [Header("Audio Fields")]
    [SerializeField]
    private AudioSource _bgmSource;
    [SerializeField]
    private AudioMixer _mixer;

    [SerializeField]
    private Transform _sfxParent;
    [SerializeField]
    private SFXPlayer _sfxPlayerPrefab;

    private Dictionary<EAudioType, AudioData> _audioDatas = new Dictionary<EAudioType, AudioData>();

    private Queue<SFXPlayer> _sfxPlayerPool = new Queue<SFXPlayer>();

    public AudioMixer Mixer => _mixer;

    protected override void Awake()
    {
        base.Awake();

        // 오디오 데이터 불러오기
        foreach (var audioData in _database.AudioDatas)
        {
            // 데이터가 없거나 클립이 비어있는 경우
            if (audioData == null || audioData.Clip == null)
            {
                continue;
            }
            var key = audioData.AudioType;
            if(!_audioDatas.TryAdd(key, audioData))
            {
                // 추가 실패한 경우 같은 키 값이 이미 존재
                Debug.LogWarning($"오디오 데이터 키 중복 [key:{key.ToString()}]");
            }
        }
    }

    public void TestPlayBGM()
    {
        PlayBGM(EAudioType.TestBGM);
    }
    public void TestPlaySFX()
    {
        PlaySFX(EAudioType.TestSFX);
    }

    public void PlayBGM(EAudioType type)
    {
        if (_bgmSource == null)
        {
            return;
        }
        // 오디오 데이터가 없음
        if(!_audioDatas.TryGetValue(type, out AudioData data))
        {
            Debug.LogWarning($"오디오 데이터가 없음 [type:{type.ToString()}]");
            return;
        }

        _bgmSource.Stop();
        _bgmSource.clip = data.Clip;
        _bgmSource.volume = data.Volume;
        _bgmSource.pitch = data.Pitch;
        _bgmSource.Play();
    }
    public void PlaySFX(EAudioType type)
    {
        if (_sfxPlayerPrefab == null)
        {
            return;
        }
        // 오디오 데이터가 없음
        if(!_audioDatas.TryGetValue(type, out AudioData data))
        {
            Debug.LogWarning($"오디오 데이터가 없음 [type:{type.ToString()}]");
            return;
        }
        SFXPlayer sfx;
        if (_sfxPlayerPool.Count <= 0)
        {
            sfx = Instantiate(_sfxPlayerPrefab, _sfxParent);
            sfx.Bind(_sfxPlayerPool);
        }
        else
        {
            sfx = _sfxPlayerPool.Dequeue();
        }
        if (sfx != null)
        {
            StartCoroutine(sfx.PlayCo(data));
        }
    }
    public void SetMasterVolume(float volume)
    {
        _mixer.SetFloat("Master", Mathf.Log10(volume) * 20f);
    }
    public void SetBGMVolume(float volume)
    {
        _mixer.SetFloat("BGM", Mathf.Log10(volume) * 20f);
    }
    public void SetSFXVolume(float volume)
    {
        _mixer.SetFloat("SFX", Mathf.Log10(volume) * 20f);
    }
}

using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "AudioDataBase", menuName = "AudioSO/AudioDataBase")]
public class AudioDataBase : ScriptableObject
{
    [SerializeField]
    private AudioData[] _audioDatas;

    public IReadOnlyList<AudioData> AudioDatas => _audioDatas;
}

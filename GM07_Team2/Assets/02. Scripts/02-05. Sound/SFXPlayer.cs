using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioSource _source;

    private Queue<SFXPlayer> _pool;

    public void Bind(Queue<SFXPlayer> pool)
    {
        _pool = pool;
    }
    public IEnumerator PlayCo(AudioData data)
    {
        _source.pitch = data.Pitch;
        _source.PlayOneShot(data.Clip, data.Volume);

        yield return new WaitForSeconds(data.Clip.length / _source.pitch);

        if (_pool != null)
        {
            _pool.Enqueue(this);
        }
    }
}

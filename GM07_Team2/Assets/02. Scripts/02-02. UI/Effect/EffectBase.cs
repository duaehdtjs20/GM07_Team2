using DG.Tweening;
using UnityEngine;

public abstract class EffectBase : MonoBehaviour
{
    protected Tween _tween;
    protected virtual void OnDisable()
    {
        Kill();
    }
    public virtual void Prepare() { }
    public abstract Tween Play();
    public virtual void Kill()
    {
        _tween?.Kill();
        _tween = null;
    }
}

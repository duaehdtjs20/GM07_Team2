using DG.Tweening;
using UnityEngine;

public abstract class EffectBase : MonoBehaviour
{
    protected RectTransform _rectTransform;
    protected Tween _tween;
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
    public abstract void Prepare();
    public abstract Tween Play();
    public virtual void Kill()
    {
        _tween?.Kill();
        _tween = null;
    }
}

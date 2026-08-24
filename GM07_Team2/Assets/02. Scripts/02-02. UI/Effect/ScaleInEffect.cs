using DG.Tweening;
using UnityEngine;

public class ScaleInEffect : EffectBase
{
    [SerializeField]
    private float _duration;

    private Vector2 _restScale;
    private bool _hasRestScale;

    public override void Prepare()
    {
        if (_rectTransform == null)
        {
            return;
        }
        if (!_hasRestScale)
        {
            _restScale = _rectTransform.localScale;
            _hasRestScale = true;
        }
        Kill();
        _rectTransform.localScale = _restScale * 0f;
    }
    public override Tween Play()
    {
        if (_rectTransform == null)
        {
            return null;
        }
        Prepare();
        _tween = _rectTransform.DOScale(_restScale, _duration).SetEase(Ease.OutBack);
        return _tween;
    }
}

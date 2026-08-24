using DG.Tweening;
using UnityEngine;

public class SlideInEffect : EffectBase
{
    [SerializeField]
    private Vector2 _startPosition;
    [SerializeField]
    private float _duration;

    private Vector2 _restPosition;
    private bool _hasRestPosition;

    public override void Prepare()
    {
        if(_rectTransform == null)
        {
            return;
        }
        if (!_hasRestPosition)
        {
            _restPosition = _rectTransform.anchoredPosition;
            _hasRestPosition = true;
        }
        Kill();
        _rectTransform.anchoredPosition = _restPosition + _startPosition;
    }
    public override Tween Play()
    {
        if(_rectTransform == null)
        {
            return null;
        }
        Prepare();
        _tween = _rectTransform.DOAnchorPos(_restPosition, _duration).SetEase(Ease.OutBack);
        return _tween;
    }
}

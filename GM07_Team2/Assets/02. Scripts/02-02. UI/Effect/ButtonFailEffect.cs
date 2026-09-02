using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonFailEffect : EffectBase
{
    private RectTransform _target;

    private Vector2 _restPosition;
    private bool _isPlaying;
    private void Awake()
    {
        _target = GetComponent<RectTransform>();
    }
    public override Tween Play()
    {
        Kill();
        _restPosition = _target.anchoredPosition;
        _isPlaying = true;

        _tween = _target.DOShakeAnchorPos(0.5f, new Vector2(10f, 0f)).SetUpdate(true).OnComplete(() =>
        {
            _isPlaying = false;
            _target.anchoredPosition = _restPosition;
            _tween = null;
        });

        return _tween;
    }
    public override void Kill()
    {
        _tween?.Kill();
        _tween = null;
        if (_isPlaying && _target != null)
        {
            _target.anchoredPosition = _restPosition;
        }
        _isPlaying = false;
    }
}

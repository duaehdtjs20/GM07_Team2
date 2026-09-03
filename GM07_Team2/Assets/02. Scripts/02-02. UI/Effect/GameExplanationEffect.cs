using DG.Tweening;
using UnityEngine;

public class ExplanationEffect : EffectBase
{
    private RectTransform _panel;
    private Vector2 _restPosition;
    private bool _hasCachedState;

    private void Awake()
    {
        _panel = GetComponent<RectTransform>();
    }
    public override void Prepare()
    {
        Kill();
        CacheState();

        _panel.anchoredPosition = Vector2.zero;
        _panel.localScale = Vector3.one;
    }
    public override Tween Play()
    {
        Prepare();
        Sequence sequence = DOTween.Sequence().SetUpdate(true);
        sequence.Append(_panel.DOScale(Vector3.one*1.1f,0.2f).SetEase(Ease.OutSine));
        sequence.AppendInterval(0.1f);
        sequence.Append(_panel.DOScale(Vector3.one, 0.2f).SetEase(Ease.InOutSine));
        sequence.Join(_panel.DOAnchorPos(_restPosition, 0.3f).SetEase(Ease.InOutCubic));
        _tween = sequence;
        return _tween;
    }
    private void CacheState()
    {
        if (_hasCachedState)
        {
            return;
        }
        _restPosition = _panel.anchoredPosition;
        _hasCachedState = true;
    }
}

using DG.Tweening;
using UnityEngine;

public class CloseUpgradePanelEffect : EffectBase
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

        _panel.anchoredPosition = _restPosition;
    }
    public override Tween Play()
    {
        Prepare();
        Sequence sequence = DOTween.Sequence().SetUpdate(true);
        sequence.Append(_panel.DOAnchorPos(_restPosition + Vector2.left * 2000f, 0.5f).SetEase(Ease.InCubic));
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

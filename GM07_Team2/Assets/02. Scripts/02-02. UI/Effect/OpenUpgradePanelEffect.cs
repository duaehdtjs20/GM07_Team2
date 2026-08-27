using DG.Tweening;
using UnityEngine;

public class OpenUpgradePanelEffect : EffectBase
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
        sequence.Append(_panel.DOAnchorPos(_restPosition + Vector2.right * 2000f, 1f).SetEase(Ease.OutBack));
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

using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class OpenEffect : EffectBase
{
    [Header("Panel")]
    [SerializeField]
    private RectTransform _upgradePanel;
    [SerializeField]
    private RectTransform _exitButton;
    [SerializeField]
    private RectTransform _startButton;

    private Vector2 _upgradeRestPosition;
    private Vector2 _buttonRestScale;
    private bool _hasCachedState;

    public override void Prepare()
    {
        Kill();
        CacheState();

        _upgradePanel.anchoredPosition = _upgradeRestPosition;
        _exitButton.localScale = _buttonRestScale;
        _startButton.localScale = _buttonRestScale;
    }
    public override Tween Play()
    {
        Prepare();
        Sequence sequence = DOTween.Sequence().SetUpdate(true);

        sequence.Append(_exitButton.DOScale(0f, 0.5f).SetEase(Ease.InBack));
        sequence.Join(_startButton.DOScale(0f, 0.5f).SetEase(Ease.OutBack));
        sequence.Join(_upgradePanel.DOAnchorPos(_upgradePanel.anchoredPosition + Vector2.left * 500, 0.5f).SetEase(Ease.OutBack));

        _tween = sequence;
        return _tween;
    }
    private void CacheState()
    {
        if (_hasCachedState)
        {
            return;
        }
        _upgradeRestPosition = _upgradePanel.anchoredPosition;
        _buttonRestScale = _exitButton.localScale;
        _hasCachedState = true;
    }
}

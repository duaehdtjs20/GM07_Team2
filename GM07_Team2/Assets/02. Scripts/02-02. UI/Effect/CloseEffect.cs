using DG.Tweening;
using UnityEngine;

public class CloseEffect : EffectBase
{
    [Header("Panel")]
    [SerializeField]
    private RectTransform _closePanel;
    [SerializeField]
    private RectTransform _nextButton;

    private Vector2 _closeRestPosition;
    private bool _hasCachedState;

    public override void Prepare()
    {
        Kill();
        CacheState();

        _closePanel.anchoredPosition = _closeRestPosition;
        _nextButton.localScale = Vector3.zero;
        _nextButton.gameObject.SetActive(true);
    }
    public override Tween Play()
    {
        Prepare();
        Sequence sequence = DOTween.Sequence().SetUpdate(true);

        sequence.Append(_nextButton.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack));
        sequence.Join(_closePanel.DOAnchorPos(_closeRestPosition + Vector2.down * 1000f, 1f).SetEase(Ease.OutBack));

        _tween = sequence;
        return _tween;
    }
    private void CacheState()
    {
        if (_hasCachedState)
        {
            return;
        }
        _closeRestPosition = _closePanel.anchoredPosition;
        _hasCachedState = true;
    }
}

using DG.Tweening;
using UnityEngine;

public class PreparingEffect : EffectBase
{
    [Header("Panel")]
    [SerializeField]
    private RectTransform _closePanel;
    [SerializeField]
    private RectTransform _nextButton;
    [SerializeField]
    private RectTransform _exitButton;
    [SerializeField]
    private RectTransform _upgradeButtons;
    [SerializeField]
    private RectTransform _startButton;

    private Vector2 _closeRestPosition;
    private Vector2 _upgradeRestPosition;
    private bool _hasCachedState;

    public override void Prepare()
    {
        Kill();
        CacheState();

        _closePanel.anchoredPosition = _closeRestPosition;
        _upgradeButtons.anchoredPosition = _upgradeRestPosition;
        _startButton.localScale = Vector2.zero;
        _exitButton.localScale = Vector2.zero;
        _upgradeButtons.gameObject.SetActive(true);
        _startButton.gameObject.SetActive(true);
        _exitButton.gameObject.SetActive(true);
    }
    public override Tween Play()
    {
        Prepare();
        Sequence sequence = DOTween.Sequence().SetUpdate(true);

        sequence.Append(_nextButton.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutBack));
        sequence.Join(_closePanel.DOAnchorPos(_closeRestPosition + Vector2.up * 1000f, 1f).SetEase(Ease.OutBack));
        sequence.Join(_upgradeButtons.DOAnchorPos(_upgradeRestPosition + Vector2.right * 500, 1f).SetEase(Ease.OutBack));
        sequence.Join(_startButton.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack));
        sequence.Join(_exitButton.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack));

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
        _upgradeRestPosition = _upgradeButtons.anchoredPosition;
        _hasCachedState = true;
    }
}

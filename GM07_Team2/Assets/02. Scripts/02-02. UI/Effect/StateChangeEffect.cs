using DG.Tweening;
using TMPro;
using UnityEngine;

public class StateChangeEffect : EffectBase
{
    private RectTransform _panel;
    private TMP_Text _text;
    private Vector2 _restPosition;
    private bool _hasCachedState;

    private void Awake()
    {
        _panel = GetComponent<RectTransform>();
        _text = GetComponentInChildren<TMP_Text>();
    }
    public override void Prepare()
    {
        CachePosition();
        Kill();
        _panel.anchoredPosition = _restPosition;
    }
    public Tween Play(string stateText, Color color)
    {
        Prepare();
        Sequence sequence = DOTween.Sequence().SetUpdate(true);
        sequence.Append(_panel.DOAnchorPosY(_restPosition.y + 300f, 0.3f).SetEase(Ease.OutQuad));
        sequence.AppendCallback(() =>
        {
            _text.text = stateText;
            _text.color = color;
        });
        sequence.Append(_panel.DOAnchorPos(_restPosition,0.3f).SetEase(Ease.OutQuad));
        sequence.OnComplete(() => _tween = null);
        _tween = sequence;
        return _tween;
    }
    public override Tween Play()
    {
        return Play(_text.text, _text.color);
    }
    private void CachePosition()
    {
        if (_hasCachedState)
        {
            return;
        }
        _restPosition = _panel.anchoredPosition;
        _hasCachedState = true;
    }
}

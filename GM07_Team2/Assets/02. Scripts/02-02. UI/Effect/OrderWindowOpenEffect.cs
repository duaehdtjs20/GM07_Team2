using DG.Tweening;
using UnityEngine;

public class OrderWindowOpenEffect : EffectBase
{
    [SerializeField]
    private RectTransform _panel;

    private Vector3 _restPosition;
    private Vector2 _restScale;
    private bool _hasCachedState;

    public Tween PlayFrom(Vector2 screenPosition)
    {
        Kill();
        CacheState();

        RectTransform parent = _panel.parent as RectTransform;
        Canvas canvas = _panel.GetComponentInParent<Canvas>();
        Camera camera = canvas.worldCamera;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(parent, screenPosition, camera, out Vector3 startPosition))
        {
            _panel.position = startPosition;
        }
        _panel.localScale = Vector3.zero;
        Sequence sequence = DOTween.Sequence().SetUpdate(true);
        sequence.Append(_panel.DOAnchorPos(_restPosition, 0.5f).SetEase(Ease.OutCubic));
        sequence.Join(_panel.DOScale(_restScale, 0.5f));
        _tween = sequence;
        return _tween;
    }
    public override void Prepare()
    {
        Kill();
        CacheState();
        _panel.anchoredPosition = Vector2.zero;
        _panel.localScale = Vector3.zero;
    }
    public override Tween Play()
    {
        Prepare();
        _tween = _panel.DOScale(_restScale,1f).SetEase(Ease.OutBack).SetUpdate(true);
        return _tween;
    }
    private void CacheState()
    {
        if (_hasCachedState)
        {
            return;
        }
        _restPosition = _panel.anchoredPosition;
        _restScale = Vector2.one;
        _hasCachedState = true;
    }
}

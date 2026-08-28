using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class TitleEffect : EffectBase
{
    [Header("Panel")]
    [SerializeField]
    private RectTransform _logo;
    [SerializeField]
    private List<RectTransform> _buttons = new();
    [SerializeField]
    private List<RectTransform> _sushiImages = new();

    private CanvasGroup _canvasGroup;
    private readonly List<Vector2> _sushiRestPositions = new();
    private bool _hasCachedState;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }
    public override void Prepare()
    {
        Kill();
        CacheState();

        _logo.localScale = Vector3.zero;
        for(int i=0; i < _buttons.Count; i++)
        {
            _buttons[i].localScale = Vector3.zero;
        }
        for(int i=0; i < _sushiImages.Count ; i++)
        {
            _sushiImages[i].anchoredPosition = _sushiRestPositions[i] + Vector2.up*1000f;
        }
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }
    public override Tween Play()
    {
        Prepare();
        Sequence sequence = DOTween.Sequence().SetUpdate(true);
        sequence.Append(_logo.DOScale(Vector3.one, 0.8f).SetEase(Ease.OutBack));
        bool addButton = false;
        for(int i=0;i< _buttons.Count ; i++)
        {
            Tween buttonTween = _buttons[i].DOScale(Vector3.one, 0.8f).SetEase(Ease.OutBack);
            if (!addButton)
            {
                sequence.Append(buttonTween);
                addButton = true;
            }
            else
            {
                sequence.Join(buttonTween);
            }
        }
        float startTime = sequence.Duration() - 0.5f;
        for(int i=0; i< _sushiImages.Count ; i++)
        {
            sequence.Insert(startTime + i * 0.1f, _sushiImages[i].DOAnchorPos(_sushiRestPositions[i], 0.25f).SetEase(Ease.OutBounce));
        }
        sequence.OnComplete(() =>
        {
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        });
        return sequence;
    }
    private void CacheState()
    {
        if (_hasCachedState)
        {
            return;
        }
        _sushiRestPositions.Clear();
        for(int i=0; i< _sushiImages.Count ; i++)
        {
            _sushiRestPositions.Add(_sushiImages[i].anchoredPosition);
        }
        _hasCachedState = true;
    }
}

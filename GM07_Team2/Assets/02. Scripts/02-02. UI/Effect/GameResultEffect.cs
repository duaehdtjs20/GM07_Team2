using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class GameResultEffect : EffectBase
{
    [SerializeField]
    private RectTransform _resultPanel;
    [SerializeField]
    private RectTransform _resultImage;
    [SerializeField]
    private List<RectTransform> _startImages = new();
    [SerializeField]
    private RectTransform _resultText;

    public override void Prepare()
    {
        Kill();

        _resultPanel.localScale = Vector3.one * 0.5f;
        _resultImage.localScale = Vector3.zero;
        _resultText.localScale = Vector3.zero;
        for (int i=0; i<_startImages.Count; i++)
        {
            _startImages[i].localScale = Vector3.zero;
            _startImages[i].localRotation = Quaternion.Euler(0f, 0f, -90f);
        }
    }
    public override Tween Play()
    {
        Prepare();

        Sequence sequence = DOTween.Sequence().SetUpdate(true);
        sequence.Append(_resultPanel.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack,0.6f));
        sequence.Join(_resultImage.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack));
        sequence.Join(_resultText.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack));
        for (int i = 0; i < _startImages.Count; i++)
        {
            RectTransform star = _startImages[i];
            float startTime = 0.2f + i * 0.2f;

            sequence.Insert(startTime, star.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack,0.3f));
            sequence.Insert(startTime, star.DOLocalRotate(Vector3.zero, 0.2f).SetEase(Ease.OutQuad));
        }
        _tween = sequence;
        return _tween;
    }
}

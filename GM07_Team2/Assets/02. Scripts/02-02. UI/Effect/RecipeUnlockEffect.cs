using DG.Tweening;
using System;
using UnityEngine;

public class RecipeUnlockEffect : EffectBase
{
    [SerializeField]
    private RectTransform _lockImage;

    private Vector2 _restPosition;
    private Quaternion _restRotation;

    public Tween Play(Action onComplete)
    public override Tween Play()
    {
        Kill();
        _restPosition = _lockImage.anchoredPosition;
        _restRotation = _lockImage.localRotation;

        Sequence sequence = DOTween.Sequence().SetUpdate(true);
        sequence.Append(_lockImage.DOShakeRotation(0.5f, new Vector3(0, 0, 20f)));
        sequence.Append(_lockImage.DOAnchorPosY(_restPosition.y - 20f, 0.3f).SetEase(Ease.InBack));
        sequence.OnComplete(() =>
        {
            _lockImage.anchoredPosition = _restPosition;
            _lockImage.localRotation = _restRotation;
            _tween = null;
            onComplete?.Invoke();
        });
        _tween = sequence;
        return _tween;
    }
    public override Tween Play()
    {
        return Play(null);
    }
}

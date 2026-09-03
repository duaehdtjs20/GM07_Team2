using DG.Tweening;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_typingText : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _text;
    [SerializeField]
    private Image _nextImage;
    [SerializeField]
    private float _tyingSpeed = 0.05f;

    private Tween _tween;
    private Tween _loop;
    private float _startAnchorPosX;

    public bool IsComplete => _text.maxVisibleCharacters >= _text.text.Length;

    private void Awake()
    {
        _startAnchorPosX = _nextImage.rectTransform.anchoredPosition.x;
    }
    private void OnEnable()
    {
        _text.maxVisibleCharacters = 0;
        _nextImage.gameObject.SetActive(false);
        _nextImage.rectTransform.anchoredPosition = new Vector2(_startAnchorPosX, 0.0f);

        _tween = DOTween.To(() => _text.maxVisibleCharacters,
            value => _text.maxVisibleCharacters = value,
            _text.text.Length,
            _tyingSpeed * _text.text.Length)
            .SetEase(Ease.Linear);

        _tween.onComplete += () => {
            _nextImage.gameObject.SetActive(true);
            _loop = _nextImage.rectTransform.DOAnchorPosX(_startAnchorPosX + 1.5f, 0.3f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            };
    }
    private void OnDisable()
    {
        DOTween.Kill(_tween);
        DOTween.Kill(_loop);
    }
    public void Skip()
    {
        _tween?.Complete();
    }
}

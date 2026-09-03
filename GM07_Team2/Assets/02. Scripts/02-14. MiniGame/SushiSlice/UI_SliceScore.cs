using DG.Tweening;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class UI_SliceScore : MonoBehaviour
{
    private Sequence _sequence;

    private void OnDisable()
    {
        _sequence?.Kill();
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        _sequence?.Kill();
    }

    public void Play(Vector3 position, int score)
    {
        TMP_Text text = GetComponent<TMP_Text>();
        RectTransform rect = text.rectTransform;
        rect.position = position;
        rect.localRotation = Quaternion.identity;
        rect.localScale = Vector3.one;
        rect.SetAsLastSibling();
        text.raycastTarget = false;
        text.text = score > 0 ? $"+{score}" : score.ToString();
        text.color = score >= 0 ? Color.green : Color.red;

        _sequence?.Kill();
        _sequence = DOTween.Sequence().SetUpdate(true);
        _sequence.Append(rect.DOAnchorPosY(rect.anchoredPosition.y + 50, 0.5f).SetEase(Ease.OutCubic));
        _sequence.Insert(0.5f * 0.35f, text.DOFade(0f, 0.5f * 0.65f));
        _sequence.OnComplete(() => Destroy(gameObject));
    }
}

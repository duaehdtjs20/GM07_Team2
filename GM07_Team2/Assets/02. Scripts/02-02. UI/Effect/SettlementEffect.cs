using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SettlementEffect : EffectBase
{
    [SerializeField]
    private List<TMP_Text> _texts = new();
    [SerializeField]
    private RectTransform _totalRevenue;
    [SerializeField]
    private TMP_Text _totalRevenueText;

    private List<Vector3> _restScales = new();
    private Vector3 _totalRestScale;
    private bool _hasCachedState;
    private int _totalRevenueValue;

    public void SetTotalRevenu(int totalRevenue)
    {
        _totalRevenueValue = totalRevenue;
    }
    override public void Prepare()
    {
        Kill();
        CacheState();
        for(int i = 0; i < _texts.Count; i++)
        {
            _texts[i].alpha = 0f;
            _texts[i].transform.localScale = _restScales[i] * 0.85f;
        }
        _totalRevenue.localScale = _totalRestScale * 0.85f;
        _totalRevenueText.text = "0";
    }
    override public Tween Play()
    {
        Prepare();
        Sequence sequence = DOTween.Sequence().SetUpdate(true);
        sequence.AppendInterval(0.3f);
        for(int i = 0; i < _texts.Count; i++)
        {
            TMP_Text text = _texts[i];
            float delay = sequence.Duration()+i*0.1f;
            sequence.Insert(delay, text.DOFade(1f, 0.3f));
            sequence.Insert(delay, text.transform.DOScale(_restScales[i], 0.3f).SetEase(Ease.OutBack));
        }
        sequence.AppendInterval(0.3f);
        sequence.Append(_totalRevenue.DOScale(_totalRestScale, 0.35f).SetEase(Ease.OutBack, 0.7f));
        sequence.Join(DOTween.To(() => 0, x => _totalRevenueText.text = $"{x:N0}", _totalRevenueValue, 0.35f).SetEase(Ease.InQuad));
        sequence.Append(_totalRevenue.DOPunchScale(Vector3.one * 0.15f, 0.35f, 5, 0.5f));
        
        sequence.OnComplete(() =>
        {
            _totalRevenue.localScale = _totalRestScale;
            _totalRevenueText.text = $"{_totalRevenueValue:N0}";
            _tween = null;
        });
        _tween = sequence;
        return _tween;
    }
    private void CacheState()
    {
        if (_hasCachedState)
        {
            return;
        }
        foreach (var text in _texts)
        {
            _restScales.Add(text.transform.localScale);
        }
        _totalRestScale = _totalRevenue.localScale;
        _hasCachedState = true;
    }
}

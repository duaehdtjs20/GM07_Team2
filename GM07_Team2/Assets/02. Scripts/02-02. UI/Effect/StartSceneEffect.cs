using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class StartSceneEffect : EffectBase
{
    [Header("Panels")]
    [SerializeField]
    private List<RectTransform> _topPanels;
    [SerializeField]
    private RectTransform _upgradePanel;
    [SerializeField]
    private List<RectTransform> _buttons;

    private readonly List<Vector2> _topRestPositions = new();
    private readonly List<Vector3> _buttonsRestScales = new();
    private Vector2 _upgradeRestPosition;
    private bool _hasCachedState;

    public override void Prepare()
    {
        Kill();
        CacheState();

        for (int i = 0; i < _topPanels.Count; i++)
        {
            _topPanels[i].anchoredPosition = _topRestPositions[i] + Vector2.up * 300f;
        }
        _upgradePanel.anchoredPosition = _upgradeRestPosition + Vector2.left * 300f;
        for (int i = 0; i < _buttons.Count; i++)
        {
            _buttons[i].localScale = _buttonsRestScales[i] * 0f;
        }
    }
    public override Tween Play()
    {
        Prepare();
        Sequence sequence = DOTween.Sequence().SetUpdate(true);

        for(int i=0;i<_topPanels.Count;i++)
        {
            sequence.Insert(0.5f, _topPanels[i].DOAnchorPos(_topRestPositions[i], 1f).SetEase(Ease.OutBack));
        }
        sequence.Insert(0.5f, _upgradePanel.DOAnchorPos(_upgradeRestPosition, 1f).SetEase(Ease.OutBack));
        for (int i = 0; i < _buttons.Count; i++)
        {
            sequence.Insert(0.5f, _buttons[i].DOScale(_buttonsRestScales[i], 1f).SetEase(Ease.OutBack));
        }

        _tween = sequence;
        return _tween;
    }
    private void CacheState()
    {
        if (_hasCachedState)
        {
            return;
        }

        _topRestPositions.Clear();
        foreach(RectTransform panel in _topPanels)
        {
            _topRestPositions.Add(panel.anchoredPosition);
        }
        _buttonsRestScales.Clear();
        foreach(RectTransform button in _buttons)
        {
            _buttonsRestScales.Add(button.localScale);
        }
        _upgradeRestPosition = _upgradePanel.anchoredPosition;
        _hasCachedState = true;
    }
}

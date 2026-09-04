using GM07.Order;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UI_SlicingGame : UI_MiniGameBase
{
    [Header("References")]
    [SerializeField] private UI_MiniGameResult _resultUI;
    [SerializeField] private RectTransform _barRect;
    [SerializeField] private RectTransform _indicatorRect;
    [SerializeField] private RectTransform _judgeZoneRect;
    [Header("Feedback (I_bar 자식으로 배치)")]
    [SerializeField] private RectTransform _feedbackRoot;
    [SerializeField] private Image _feedbackImage;
    [SerializeField] private Sprite _hitSprite;
    [SerializeField] private Sprite _missSprite;
    [SerializeField] private TMP_Text _feedbackText;
    [SerializeField] private string _hitMessage = "HIT!";
    [SerializeField] private string _missMessage = "MISS";
    [SerializeField] private float _feedbackDuration = 0.3f;
    [Header("Beat Transition")]
    [SerializeField] private float _beatTransitionDelay = 0.3f;
    [Header("Progress")]
    [SerializeField] private TMP_Text _progressText;
    [Header("Ingredient Cutting Visual")]
    [SerializeField] private Image _fishImage;
    [SerializeField] private IngredientStageSet[] _ingredientStageSets;
    [SerializeField] private Image _knifeImage;
    [SerializeField] private Vector2 _knifeStrikeOffset = new Vector2(0f, -40f);
    [SerializeField] private float _knifeStrikeDuration = 0.15f;
    [SerializeField] private int _knifeStrikeRepeatCount = 3;
    [SerializeField] private float _knifeStrikeRepeatGap = 0.08f;
    [Header("Grade Scaling")]
    [SerializeField] private int[] _beatCountsByGrade = new int[3] { 3, 4, 5 };
    [SerializeField] private float[] _sweepDurationByGrade = new float[3] { 1.2f, 1.0f, 0.8f };
    [SerializeField] private float[] _hitWindowByGrade = new float[3] { 0.16f, 0.12f, 0.08f };
    [Header("Target Zone")]
    [SerializeField] private float _targetMargin = 0.15f;
    [Header("Hit Judgement")]
    [Range(0f, 1f)]
    [SerializeField] private float _requiredOverlapRatio = 0.667f;
    [Header("Quality (성공개수 기준 등급 + 직원등급 보정)")]
    [SerializeField] private int[] _normalStartCountByGrade = new int[3] { 1, 1, 1 }; // 이 개수부터 Normal
    [SerializeField] private int[] _goodStartCountByGrade = new int[3] { 2, 3, 3 };   // 이 개수부터 Good
    [Header("Result")]
    [SerializeField] private float _resultDisplayDuration = 1.5f;
    [Header("Order")]
    [SerializeField]
    private Image _orderIcon;
    [SerializeField]
    private TMP_Text _menuName;
    [SerializeField]
    private GameObject _grade;
    // 재료(레시피)별 손질 단계 이미지 세트. ingredientKey를 order.Recipe.Data.IngredientIcon과 비교해서 매칭.
    [System.Serializable]
    private class IngredientStageSet
    {
        public Sprite ingredientKey;
        public Sprite[] stageSprites;
    }
    private OrderData _order;
    private Action<EQuality> _onCompleted;
    private int _currentGradeIndex;
    private int _totalBeats;
    private int _currentBeatIndex;
    private int _successCount;
    private int _currentCombo;
    private float _hitHalfWidth;
    private float _sweepDuration;
    private float _sweepTimer;
    private float _currentTargetNormalized;
    private bool _isPlaying;
    private Sprite[] _currentStageSprites;
    private Vector2 _knifeIdlePosition;
    private Coroutine _completeCoroutine;
    private Coroutine _feedbackCoroutine;
    private Coroutine _knifeCoroutine;
    private Coroutine _beatTransitionCoroutine;
    protected override void Awake()
    {
        base.Awake();
        if (_knifeImage != null)
        {
            _knifeIdlePosition = _knifeImage.rectTransform.anchoredPosition;
        }
    }
    public override void Open(OrderData order, Action<EQuality> onCompleted)
    {
        StopAllGameCoroutines();
        gameObject.SetActive(true);
        if (_knifeImage != null)
        {
            _knifeImage.rectTransform.anchoredPosition = _knifeIdlePosition;
        }
        _order = order;
        _onCompleted = onCompleted;
        _currentGradeIndex = (int)order.Recipe.Data.MenuGrade;
        _totalBeats = _beatCountsByGrade[_currentGradeIndex];
        _sweepDuration = _sweepDurationByGrade[_currentGradeIndex];
        _hitHalfWidth = _hitWindowByGrade[_currentGradeIndex];
        _currentBeatIndex = 0;
        _successCount = 0;
        _currentCombo = 0;
        _isPlaying = true;
        UpdateProgressText();
        _currentStageSprites = ResolveStageSprites(order.Recipe.Data.IngredientIcon);
        if (_resultUI != null)
        {
            _resultUI.gameObject.SetActive(false);
        }
        if (_feedbackRoot != null)
        {
            _feedbackRoot.gameObject.SetActive(false);
        }
        _menuName.text = order.Recipe.Data.Name;
        _orderIcon.sprite = order.Recipe.Data.Icon;
        if (_grade.TryGetComponent<Image>(out Image gradeImage))
        {
            gradeImage.color = GetGradeColor(order.Recipe.Data.MenuGrade);
            TMP_Text gradeText = _grade.GetComponentInChildren<TMP_Text>();
            gradeText.text = order.Recipe.Data.MenuGrade.ToString();
        }
        UpdateFishVisual();
        StartNextBeat();
        _effect.Play();
    }
    private Sprite[] ResolveStageSprites(Sprite ingredientIcon)
    {
        if (_ingredientStageSets == null || ingredientIcon == null)
        {
            return null;
        }
        foreach (IngredientStageSet set in _ingredientStageSets)
        {
            if (set != null && set.ingredientKey == ingredientIcon)
            {
                return set.stageSprites;
            }
        }
        return null;
    }
    private void StopAllGameCoroutines()
    {
        if (_completeCoroutine != null) { StopCoroutine(_completeCoroutine); _completeCoroutine = null; }
        if (_feedbackCoroutine != null) { StopCoroutine(_feedbackCoroutine); _feedbackCoroutine = null; }
        if (_knifeCoroutine != null) { StopCoroutine(_knifeCoroutine); _knifeCoroutine = null; }
        if (_beatTransitionCoroutine != null) { StopCoroutine(_beatTransitionCoroutine); _beatTransitionCoroutine = null; }
    }
    private void Update()
    {
        if (!_isPlaying)
        {
            return;
        }
        _sweepTimer += Time.deltaTime;
        float t = Mathf.PingPong(_sweepTimer / _sweepDuration, 1.0f);
        UpdateIndicatorVisual(t);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryJudgeInput();
        }
    }
    // 스페이스 키든 버튼 클릭이든 여기로 모여서 같은 판정을 탐
    private void TryJudgeInput()
    {
        if (!_isPlaying)
        {
            return;
        }
        float t = Mathf.PingPong(_sweepTimer / _sweepDuration, 1.0f);
        JudgeInput(t);
    }
    // 스페이스바 안내 이미지에 추가한 Button의 OnClick()에 이 함수를 연결
    public void OnSpaceButtonClicked()
    {
        TryJudgeInput();
    }
    private void StartNextBeat()
    {
        _sweepTimer = 0f;
        _currentTargetNormalized = UnityEngine.Random.Range(_targetMargin, 1f - _targetMargin);
        UpdateJudgeZoneVisual();
    }
    private float NormalizedToLocalX(float t)
    {
        float barWidth = _barRect != null ? _barRect.rect.width : 0f;
        return Mathf.Lerp(-barWidth * 0.5f, barWidth * 0.5f, t);
    }
    private void UpdateIndicatorVisual(float t)
    {
        if (_indicatorRect == null)
        {
            return;
        }
        float x = NormalizedToLocalX(t);
        _indicatorRect.anchoredPosition = new Vector2(x, _indicatorRect.anchoredPosition.y);
    }
    private void UpdateJudgeZoneVisual()
    {
        if (_judgeZoneRect == null || _barRect == null)
        {
            return;
        }
        float x = NormalizedToLocalX(_currentTargetNormalized);
        _judgeZoneRect.anchoredPosition = new Vector2(x, _judgeZoneRect.anchoredPosition.y);
        float zoneWidth = _hitHalfWidth * 2f * _barRect.rect.width;
        Vector2 size = _judgeZoneRect.sizeDelta;
        size.x = zoneWidth;
        _judgeZoneRect.sizeDelta = size;
    }
    private void UpdateProgressText()
    {
        if (_progressText == null)
        {
            return;
        }
        _progressText.text = $"{_currentBeatIndex} / {_totalBeats}\nCombo {_currentCombo}";
    }
    private bool CheckHit(float indicatorValue)
    {
        if (_barRect == null || _indicatorRect == null)
        {
            float distance = Mathf.Abs(indicatorValue - _currentTargetNormalized);
            return distance <= _hitHalfWidth;
        }
        float barWidth = _barRect.rect.width;
        float indicatorWidth = _indicatorRect.rect.width;
        float indicatorCenter = NormalizedToLocalX(indicatorValue);
        float targetCenter = NormalizedToLocalX(_currentTargetNormalized);
        float targetHalfWidth = _hitHalfWidth * barWidth;
        float indicatorLeft = indicatorCenter - indicatorWidth * 0.5f;
        float indicatorRight = indicatorCenter + indicatorWidth * 0.5f;
        float targetLeft = targetCenter - targetHalfWidth;
        float targetRight = targetCenter + targetHalfWidth;
        float overlap = Mathf.Min(indicatorRight, targetRight) - Mathf.Max(indicatorLeft, targetLeft);
        overlap = Mathf.Max(0f, overlap);
        float overlapRatio = indicatorWidth > 0f ? overlap / indicatorWidth : 0f;
        return overlapRatio >= _requiredOverlapRatio;
    }
    private void JudgeInput(float indicatorValue)
    {
        _isPlaying = false;
        bool isHit = CheckHit(indicatorValue);
        if (isHit)
        {
            _successCount++;
            _currentCombo++;
            PlayKnifeStrike();
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(EAudioType.Hit);
            }
        }
        else
        {
            _currentCombo = 0;
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(EAudioType.Miss);
            }
        }
        ShowFeedback(isHit, indicatorValue);
        UpdateFishVisual();
        _currentBeatIndex++;
        UpdateProgressText();
        if (_beatTransitionCoroutine != null)
        {
            StopCoroutine(_beatTransitionCoroutine);
        }
        _beatTransitionCoroutine = StartCoroutine(BeatTransitionCo());
    }
    private IEnumerator BeatTransitionCo()
    {
        yield return new WaitForSeconds(_beatTransitionDelay);
        _beatTransitionCoroutine = null;
        if (_currentBeatIndex >= _totalBeats)
        {
            FinishGame();
        }
        else
        {
            _isPlaying = true;
            StartNextBeat();
        }
    }
    private void ShowFeedback(bool isHit, float indicatorValue)
    {
        if (_feedbackRoot == null)
        {
            return;
        }
        float x = NormalizedToLocalX(indicatorValue);
        Vector2 pos = _feedbackRoot.anchoredPosition;
        pos.x = x;
        _feedbackRoot.anchoredPosition = pos;
        if (_feedbackImage != null)
        {
            _feedbackImage.sprite = isHit ? _hitSprite : _missSprite;
        }
        if (_feedbackText != null)
        {
            _feedbackText.text = isHit ? _hitMessage : _missMessage;
        }
        if (_feedbackCoroutine != null)
        {
            StopCoroutine(_feedbackCoroutine);
        }
        _feedbackRoot.gameObject.SetActive(true);
        _feedbackCoroutine = StartCoroutine(HideFeedbackCo());
    }
    private IEnumerator HideFeedbackCo()
    {
        yield return new WaitForSeconds(_feedbackDuration);
        _feedbackCoroutine = null;
        if (_feedbackRoot != null)
        {
            _feedbackRoot.gameObject.SetActive(false);
        }
    }
    private void UpdateFishVisual()
    {
        if (_fishImage == null || _currentStageSprites == null || _currentStageSprites.Length == 0)
        {
            return;
        }
        float ratio = _totalBeats > 0 ? (float)_successCount / _totalBeats : 0f;
        int stageIndex = Mathf.Clamp(Mathf.RoundToInt(ratio * (_currentStageSprites.Length - 1)), 0, _currentStageSprites.Length - 1);
        _fishImage.sprite = _currentStageSprites[stageIndex];
    }
    private void PlayKnifeStrike()
    {
        if (_knifeImage == null)
        {
            return;
        }
        if (_knifeCoroutine != null)
        {
            StopCoroutine(_knifeCoroutine);
        }
        _knifeCoroutine = StartCoroutine(KnifeStrikeCo());
    }
    private IEnumerator KnifeStrikeCo()
    {
        RectTransform knifeRect = _knifeImage.rectTransform;
        Vector2 idle = _knifeIdlePosition;
        Vector2 strike = idle + _knifeStrikeOffset;
        float half = Mathf.Max(0.01f, _knifeStrikeDuration * 0.5f);
        for (int repeat = 0; repeat < _knifeStrikeRepeatCount; repeat++)
        {
            float t = 0f;
            while (t < half)
            {
                t += Time.deltaTime;
                knifeRect.anchoredPosition = Vector2.Lerp(idle, strike, t / half);
                yield return null;
            }
            t = 0f;
            while (t < half)
            {
                t += Time.deltaTime;
                knifeRect.anchoredPosition = Vector2.Lerp(strike, idle, t / half);
                yield return null;
            }
            knifeRect.anchoredPosition = idle;
            if (repeat < _knifeStrikeRepeatCount - 1)
            {
                yield return new WaitForSeconds(_knifeStrikeRepeatGap);
            }
        }
        _knifeCoroutine = null;
    }
    private void FinishGame()
    {
        float successRatio = _totalBeats > 0 ? (float)_successCount / _totalBeats : 0f;
        float staffBonus = GetStaffBonus();
        EQuality quality = ScoreToQuality();
        if (_completeCoroutine != null)
        {
            StopCoroutine(_completeCoroutine);
            _completeCoroutine = null;
        }
        _completeCoroutine = StartCoroutine(CompleteAfterDelayCo(quality, successRatio, staffBonus));
    }
    // 다른 미니게임들과 동일하게 Staff.QualityBonus를 그대로 사용 (결과창 표시용)
    private float GetStaffBonus()
    {
        if (_order.Staff == null)
        {
            return 0f;
        }
        return _order.Staff.QualityBonus;
    }
    // 순수 성공개수로 매긴 등급(0=Fail,1=Normal,2=Good,3=Great)에서 직원등급만큼 보정.
    // Lv1: Fail만 방지(최소 Normal 보장). Lv2: 생 결과보다 정확히 한 칸 위(Good→Great도 가능).
    private EQuality ScoreToQuality()
    {
        int rawTier = GetRawTier();
        int staffLevel = GetStaffLevel();
        int finalTier;
        switch (staffLevel)
        {
            case 0: finalTier = rawTier; break;
            case 1: finalTier = Mathf.Max(rawTier, 1); break;
            default: finalTier = Mathf.Min(rawTier + 1, 3); break;
        }
        return TierToQuality(finalTier);
    }
    private int GetRawTier()
    {
        if (_successCount >= _totalBeats) { return 3; } // 진짜 100% 성공했을 때만 Great
        if (_successCount >= _goodStartCountByGrade[_currentGradeIndex]) { return 2; }
        if (_successCount >= _normalStartCountByGrade[_currentGradeIndex]) { return 1; }
        return 0;
    }
    // Staff.QualityBonus(0/0.1/0.2)를 등급 인덱스(0/1/2)로 역산. Staff.cs는 건드리지 않음.
    private int GetStaffLevel()
    {
        if (_order.Staff == null) { return 0; }
        return Mathf.RoundToInt(_order.Staff.QualityBonus / 0.1f);
    }
    private EQuality TierToQuality(int tier)
    {
        switch (tier)
        {
            case 0: return EQuality.Fail;
            case 1: return EQuality.Normal;
            case 2: return EQuality.Good;
            default: return EQuality.Great;
        }
    }
    private IEnumerator CompleteAfterDelayCo(EQuality quality, float score, float staffBonus)
    {
        if (_resultUI != null)
        {
            _resultUI.ApplyResult(quality, score, staffBonus);
            _resultUI.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(_resultDisplayDuration);
        if (_resultUI != null)
        {
            _resultUI.gameObject.SetActive(false);
        }
        _completeCoroutine = null;
        Action<EQuality> callback = _onCompleted;
        _onCompleted = null;
        callback?.Invoke(quality);
        gameObject.SetActive(false);
    }
}
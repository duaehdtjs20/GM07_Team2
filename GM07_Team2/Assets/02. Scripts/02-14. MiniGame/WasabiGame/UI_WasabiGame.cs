using GM07.Order;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_WasabiGame : UI_MiniGameBase
{
    private enum EWasabiState
    {
        None,
        Waiting,
        Filling,
        Completed,
    }
    [Header("Order")]
    [SerializeField]
    private TMP_Text _nemuName;
    [SerializeField]
    private Image _orderIcon;
    [Header("Timer")]
    [SerializeField]
    private TMP_Text _timer;
    [SerializeField]
    private float _timeLimit;
    [SerializeField]
    private float _completeDuration;
    [Header("Input")]
    [SerializeField]
    private Button _squeezeButton;
    [Header("Gauge")]
    [SerializeField]
    private Image _gaugeImage;
    [SerializeField]
    private float _fillSpeed;
    [Header("Wasabi")]
    [SerializeField]
    private Image _wasabiImage;
    [SerializeField]
    private Image _wasabiTubeImage;
    [SerializeField]
    private Sprite _defaultSprite;
    [SerializeField]
    private Sprite _squeezedSprite;
    [SerializeField]
    private Sprite _moreSqueezedSprite;
    [SerializeField]
    private GameObject _mosaicImage;
    [Header("Sprite Threshold")]
    [Range(0f, 1f)]
    [SerializeField]
    private float _squeezedSpriteThreshold = 0.2f;
    [Range(0f, 1f)]
    [SerializeField]
    private float _moreSqueezedSpriteThreshold = 0.7f;
    [Header("Wasabi Size")]
    [SerializeField]
    private Vector2 _clearSize;
    [SerializeField]
    private Vector2 _maxSize;
    [Header("Clear Range")]
    [Range(0f, 1f)]
    [SerializeField]
    private float _minimumClearFill = 0.25f;
    [Range(0f, 1f)]
    [SerializeField]
    private float _maximumClearFill = 0.9f;
    [Header("Target")]
    [Range(0f, 1f)]
    [SerializeField]
    private float _targetFill;
    [SerializeField]
    private float _maximumScoreDistance = 0.5f;
    [Header("Quality")]
    [Range(0f, 1f)]
    [SerializeField]
    private float _goodScore;
    [Range(0f, 1f)]
    [SerializeField]
    private float _greatScore;
    [Header("Result")]
    [SerializeField]
    private UI_MiniGameResult _resultUI;

    private OrderData _order;
    private Action<EQuality> _onCompleted;
    private EWasabiState _state = EWasabiState.None;
    private Coroutine _completeCoroutine;

    private float _remainingTime;
    private float _fillAmount;

    private void Update()
    {
        if (_state == EWasabiState.None || _state == EWasabiState.Completed)
        {
            return;
        }

        UpdateTimer();
        KeyboardInput();

        if (_state == EWasabiState.Filling)
        {
            FillGauge();
        }
    }
    private void OnDisable()
    {
        if(_completeCoroutine!= null)
        {
            StopCoroutine(_completeCoroutine);
            _completeCoroutine = null;
        }
    }
    public override void Open(OrderData order, Action<EQuality> onCompleted)
    {
        if (order == null || order.Recipe == null || order.Recipe.Data == null)
        {
            onCompleted?.Invoke(EQuality.Fail);
            return;
        }
        if (_completeCoroutine != null)
        {
            StopCoroutine(_completeCoroutine);
            _completeCoroutine = null;
        }
        _order = order;
        _onCompleted = onCompleted;
        _nemuName.text = order.Recipe.Data.Name;
        _orderIcon.sprite = order.Recipe.Data.Icon;
        _remainingTime = _timeLimit;
        _fillAmount = 0;
        _state = EWasabiState.Waiting;
        if (_gaugeImage != null)
        {
            _gaugeImage.fillAmount = 0f;
            _gaugeImage.gameObject.SetActive(order.Recipe.Data.MenuGrade == EMenuGrade.Low);
        }
        if (_wasabiImage != null)
        {
            _wasabiImage.rectTransform.sizeDelta = Vector2.zero;
        }
        if(_wasabiTubeImage != null)
        {
            _wasabiTubeImage.sprite = _defaultSprite;
        }
        RefreshTimer();
        gameObject.SetActive(true);
        if (_squeezeButton != null)
        {
            _squeezeButton.interactable = true;
        }
    }
    private void UpdateTimer()
    {
        _remainingTime -= Time.deltaTime;
        RefreshTimer();
        if (_remainingTime <= 0)
        {
            CompleteGame(EQuality.Fail);
        }
    }
    private void RefreshTimer()
    {
        if (_timer != null)
        {
            _timer.text = Mathf.CeilToInt(_remainingTime).ToString();
        }
    }
    private void KeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            BeginSqueeze();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            EndSqueeze();
        }
    }
    /// <summary>
    /// 버튼 또는 Space를 누른 순간 호출됩니다.
    /// </summary>
    public void BeginSqueeze()
    {
        if (_state != EWasabiState.Waiting)
        {
            return;
        }
        if(_order.Recipe.Data.MenuGrade == EMenuGrade.High)
        {
            _mosaicImage.SetActive(true);
        }
        _state = EWasabiState.Filling;
    }

    /// <summary>
    /// 버튼 또는 Space에서 손을 뗀 순간 호출됩니다.
    /// </summary>
    public void EndSqueeze()
    {
        if (_state != EWasabiState.Filling)
        {
            return;
        }
        if (_order.Recipe.Data.MenuGrade == EMenuGrade.High)
        {
            _mosaicImage.SetActive(false);
        }
        EvaluateResult();
    }
    private void FillGauge()
    {
        _fillAmount += _fillSpeed * Time.unscaledDeltaTime;
        _fillAmount = Mathf.Clamp01(_fillAmount);

        if (_gaugeImage != null)
        {
            _gaugeImage.fillAmount = _fillAmount;
        }

        RefreshWasabiUI();

        if (_fillAmount >= 1)
        {
            EvaluateResult();
        }
    }
    private void RefreshWasabiUI()
    {
        if(_wasabiTubeImage == null || _wasabiImage == null)
        {
            return;
        }

        if (_fillAmount >= _moreSqueezedSpriteThreshold)
        {
            _wasabiTubeImage.sprite = _moreSqueezedSprite;
        }
        else if (_fillAmount >= _squeezedSpriteThreshold)
        {
            _wasabiTubeImage.sprite = _squeezedSprite;
        }
        else
        {
            _wasabiTubeImage.sprite = _defaultSprite;
        }

        Vector2 currentSize;
        if (_fillAmount <= _targetFill)
        {
            float ratio = Mathf.InverseLerp(0f, _targetFill, _fillAmount);
            currentSize = Vector2.Lerp(Vector2.zero, _clearSize, ratio);
        }
        else
        {
            float ratio = Mathf.InverseLerp(_targetFill, 1f, _fillAmount);
            currentSize = Vector2.Lerp(_clearSize, _maxSize, ratio);
        }
        _wasabiImage.rectTransform.sizeDelta = currentSize;
    }
    private void EvaluateResult()
    {
        if (_state != EWasabiState.Filling)
        {
            return;
        }

        if (_fillAmount < _minimumClearFill || _fillAmount > _maximumClearFill)
        {
            CompleteGame( EQuality.Fail);
            return;
        }
        EQuality quality = CalculateQuality(out float totalScore);
        CompleteGame(quality, totalScore, GetStaffQualityBonus());
    }
    private void CompleteGame(EQuality quality,float totalScore=0, float staffBonus=0)
    {
        if (_state == EWasabiState.Completed)
        {
            return;
        }
        _state = EWasabiState.Completed;
        if (_squeezeButton != null)
        {
            _squeezeButton.interactable = false;
        }
        if (_completeCoroutine != null)
        {
            StopCoroutine(_completeCoroutine);
            _completeCoroutine = null;
        }
        _completeCoroutine = StartCoroutine(CompleteCoroutine(quality, totalScore, staffBonus));
    }
    private IEnumerator CompleteCoroutine(EQuality quality, float totalScore, float staffBonus)
    {
        if (_resultUI != null)
        {
            _resultUI.ApplyResult(quality, totalScore, staffBonus);
            _resultUI.gameObject.SetActive(true);
        }
        yield return new WaitForSecondsRealtime(_completeDuration);
        if (_resultUI != null)
        {
            _resultUI.gameObject.SetActive(false);
        }
        _completeCoroutine = null;
        Action<EQuality> callback =_onCompleted;
        _onCompleted = null;
        _order = null;
        callback?.Invoke(quality);
        gameObject.SetActive(false);
    }
    private EQuality CalculateQuality(out float score)
    {
        float distance = Mathf.Abs(_fillAmount - _targetFill);
        float baseScore = 1f - Mathf.Clamp01(distance / _maximumScoreDistance);
        float totalScore = baseScore + GetStaffQualityBonus();

        if (totalScore >= _greatScore)
        {
            score = totalScore;
            return EQuality.Great;
        }
        if (totalScore >= _goodScore)
        {
            score = totalScore;
            return EQuality.Good;
        }
        score = totalScore;
        return EQuality.Normal;
    }
    private float GetStaffQualityBonus()
    {
        if (_order == null || _order.Staff == null)
        {
            return 0f;
        }

        return _order.Staff.QualityBonus;
    }
}

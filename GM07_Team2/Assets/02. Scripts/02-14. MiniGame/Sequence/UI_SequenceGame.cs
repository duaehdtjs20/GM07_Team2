using GM07.Order;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_SequenceGame : UI_MiniGameBase
{
    private enum ESequenceState
    {
        None,
        Memorizing,
        Playing,
        Completed,
    }
    [Header("Order")]
    [SerializeField]
    private TMP_Text _menuName;
    [SerializeField]
    private Image _orderIcon;
    [SerializeField]
    private GameObject _grade;
    [Header("Plate")]
    [SerializeField]
    private List<UI_SequencePlate> _plates = new();
    [Header("Guide")]
    [SerializeField]
    private TMP_Text _guideText;
    [Header("Timer")]
    [SerializeField]
    private TMP_Text _timerText;
    [SerializeField]
    private float _timeLimit;
    [SerializeField]
    private float _completeDuration;
    [Header("Quality")]
    [SerializeField]
    private float _goodTimeRatio;
    [SerializeField]
    private float _greatTimeRatio;
    [Header("Result")]
    [SerializeField]
    private UI_MiniGameResult _resultUI;
    [Header("Level Setting")]
    [SerializeField]
    private List<SequenceLevel> _levelSettings = new();

    private readonly List<int> _sequence = new();
    private OrderData _order;
    private Action<EQuality> _onCompleted;
    private Coroutine _memorizeCoroutine;
    private Coroutine _completeCoroutine;
    private ESequenceState _state;
    private int _inputIndex;
    private float _remainingTime;
    private SequenceLevel _currentLevel;
    private int _activePlateCount;

    private void Update()
    {
        if (_state != ESequenceState.Playing) return;
        _remainingTime -= Time.deltaTime;
        RefreshTimer();
        if (_remainingTime <= 0f)
        {
            _remainingTime = 0f;
            CompleteGame(EQuality.Fail);
        }
    }
    private void OnDisable()
    {
        if (_memorizeCoroutine != null)
        {
            StopCoroutine(_memorizeCoroutine);
            _memorizeCoroutine = null;
        }
        if(_completeCoroutine != null )
        {
            StopCoroutine(_completeCoroutine);
            _completeCoroutine = null;
        }
        _state = ESequenceState.None;
    }
    public override void Open(OrderData order, Action<EQuality> onCompleted)
    {
        if (order == null || order.Recipe == null || order.Recipe.Data == null || _plates.Count == 0)
        {
            onCompleted?.Invoke(EQuality.Fail);
            return;
        }
        if (_memorizeCoroutine != null)
        {
            StopCoroutine(_memorizeCoroutine);
            _memorizeCoroutine = null;
        }
        if (_completeCoroutine != null)
        {
            StopCoroutine(_completeCoroutine);
            _completeCoroutine = null;
        }
        _order = order;
        _onCompleted = onCompleted;
        _inputIndex = 0;
        _remainingTime = _timeLimit;
        _state = ESequenceState.Memorizing;
        _currentLevel = GetLevel(order.Recipe.Data.MenuGrade);
        _activePlateCount = _currentLevel.PlateCount;

        if (_menuName != null) _menuName.text = order.Recipe.Data.Name;
        if (_orderIcon != null) _orderIcon.sprite = order.Recipe.Data.Icon;
        if (_resultUI != null) _resultUI.gameObject.SetActive(false);
        if (_grade.TryGetComponent<Image>(out Image gradeImage))
        {
            gradeImage.color = GetGradeColor(order.Recipe.Data.MenuGrade);
            TMP_Text gradeText = _grade.GetComponentInChildren<TMP_Text>();
            gradeText.text = order.Recipe.Data.MenuGrade.ToString();
        }
        gameObject.SetActive(true);
        ResetPlates();
        BuildSequence();
        RefreshTimer();
        _memorizeCoroutine = StartCoroutine(MemorizeCoroutine());
    }
    private void ResetPlates()
    {
        for (int i = 0; i < _plates.Count; i++)
        {
            bool isActive = i < _activePlateCount;
            _plates[i].gameObject.SetActive(isActive);
            if (isActive)
            {
                _plates[i].Bind(i, _order.Recipe.Data.Icon, OnPlateClicked);
            }
        }
    }
    private void BuildSequence()
    {
        _sequence.Clear();
        for (int i = 0; i < _activePlateCount; i++)
        {
            _sequence.Add(i);
        }
        for (int i = _sequence.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            int temp = _sequence[i];
            _sequence[i] = _sequence[randomIndex];
            _sequence[randomIndex] = temp;
        }
    }
    private IEnumerator MemorizeCoroutine()
    {
        for(int i = 3; i > 0; i--)
        {
            _guideText.text = i.ToString();
            yield return new WaitForSecondsRealtime(0.3f);
        }
        if (_guideText != null)
        {
            _guideText.text = "순서를 기억하세요";
        }
        for (int i=0;i< _sequence.Count;i++)
        {
            UI_SequencePlate plate = _plates[_sequence[i]];
            plate.SetOrderText(i + 1);
            yield return new WaitForSecondsRealtime(_currentLevel.OrderDisplayDuration);
            plate.ResetOrder();
        }
        SetPlateInteractable(true);
        _memorizeCoroutine = null;
        _state = ESequenceState.Playing;
        _remainingTime = _timeLimit;
        if (_guideText != null)
        {
            _guideText.text = "순서대로 접시를 선택하세요";
        }
        RefreshTimer();
    }
    private void OnPlateClicked(int plateIndex)
    {
        if(_state != ESequenceState.Playing)
        {
            return;
        }
        if(plateIndex != _sequence[_inputIndex])
        {
            CompleteGame(EQuality.Fail);
            return;
        }
        AudioManager.Instance?.PlaySFX(EAudioType.Game_Drop);
        _inputIndex++;
        if(_inputIndex >= _sequence.Count)
        {
            float timeRatio = Mathf.Clamp01(_remainingTime / _timeLimit);
            float totalScore = Mathf.Clamp01( timeRatio + GetStaffQualityBonus());
            EQuality quality = CalculateQuality(totalScore);
            CompleteGame(quality, totalScore);
        }
    }
    private void CompleteGame(EQuality quality, float totalScore=0)
    {
        if (_state == ESequenceState.Completed)
        {
            return;
        }
        _state = ESequenceState.Completed;
        SetPlateInteractable(false);
        _completeCoroutine = StartCoroutine(CompleteCoroutine(quality, totalScore));
    }
    private IEnumerator CompleteCoroutine(EQuality quality, float totalScore)
    {
        if (_guideText != null)
        {
            _guideText.text = string.Empty;
        }
        if (_resultUI != null)
        {
            _resultUI.ApplyResult(quality, totalScore, GetStaffQualityBonus());
            _resultUI.gameObject.SetActive(true);
        }
        yield return new WaitForSecondsRealtime(_completeDuration);
        if (_resultUI != null)
        {
            _resultUI.gameObject.SetActive(false);
        }
        _completeCoroutine = null;
        Action<EQuality> callback = _onCompleted;
        _onCompleted = null;
        _order = null;
        callback?.Invoke(quality);
        gameObject.SetActive(false);
    }
    private void SetPlateInteractable(bool interactable)
    {
        foreach(UI_SequencePlate plate in _plates)
        {
            plate.SetInteractable(interactable);
        }
    }
    private void RefreshTimer()
    {
        if (_timerText != null)
        {
            _timerText.text = Mathf.CeilToInt(_remainingTime).ToString();
        }
    }
    private EQuality CalculateQuality(float totalScore)
    {
        if (totalScore >= _greatTimeRatio)
        {
            return EQuality.Great;
        }
        if (totalScore >= _goodTimeRatio)
        {
            return EQuality.Good;
        }
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
    private SequenceLevel GetLevel(EMenuGrade grade)
    {
        foreach(SequenceLevel level in _levelSettings)
        {
            if(level.MenuGrade == grade)
            {
                return level;
            }
        }
        return null;
    }
}
[Serializable]
public class SequenceLevel
{
    [SerializeField]
    private EMenuGrade _menuGrade;
    [SerializeField]
    private int _plateCount;
    [SerializeField]
    private float _orderDisplayDuration;

    public EMenuGrade MenuGrade => _menuGrade;
    public int PlateCount => _plateCount;
    public float OrderDisplayDuration => _orderDisplayDuration;
}
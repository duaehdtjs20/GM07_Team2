using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_MainGame : MonoBehaviour
{
    [Header("Heirarchy")]
    [SerializeField]
    private GameFlowManager _gameFlowManager;
    [SerializeField]
    private TMP_Text _dayText;
    [SerializeField]
    private TMP_Text _remainingTimeText;
    [SerializeField]
    private TMP_Text _gameStateText;
    [SerializeField]
    private TMP_Text _moneyText;
    [SerializeField]
    private Button _openButton;
    [SerializeField]
    private Button _nextdayButton;
    [SerializeField]
    private GameObject _closePanel;
    [SerializeField]
    private Image _clockImage;

    [Header("Preparing Panel")]
    [SerializeField]
    private GameObject _preparingPanel;

    private const int OpenHour = 10;
    private const int CloseHour = 21;
    private const int PreparingHour = 9;
    private const float MorningEndProgress = 0.33f;
    private const float LunchEndProgress = 0.66f;
    private const float ClockMaxFillAmount = (CloseHour - OpenHour) / 12f;

    private void Start()
    {
        if(_gameFlowManager == null)
        {
            return;
        }
        if(_openButton != null)
        {
            _openButton.onClick.AddListener(() => _gameFlowManager.OnClickOpen());
        }
        if(_nextdayButton != null)
        {
            _nextdayButton.onClick.AddListener(() => _gameFlowManager.OnClickNextDay());
        }
        _gameFlowManager.OnGameStateChanged += RefreshGameState;
        _gameFlowManager.OnRemainingTimeChanged += RefreshRemainingTime;
        _gameFlowManager.OnDayChanged += RefreshDay;
        CurrencyManager.Instance.OnMoneyChanged += RefreshMoney;

        RefreshPreparingPanel(_gameFlowManager.GameState);
    }
    private void OnDisable()
    {
        if(_gameFlowManager == null)
        {
            return;
        }
        if (_openButton != null)
        {
            _openButton.onClick.RemoveListener(() => _gameFlowManager.OnClickOpen());
        }
        if (_nextdayButton != null)
        {
            _nextdayButton.onClick.RemoveListener(() => _gameFlowManager.OnClickNextDay());
        }
        _gameFlowManager.OnGameStateChanged -= RefreshGameState;
        _gameFlowManager.OnRemainingTimeChanged -= RefreshRemainingTime;
        _gameFlowManager.OnDayChanged -= RefreshDay;
        CurrencyManager.Instance.OnMoneyChanged -= RefreshMoney;
    }

    private void RefreshGameState(EGameState gameState)
    {
        RefreshButton();
        RefreshRemainingTime(_gameFlowManager.RemainingTime);
        RefreshPreparingPanel(gameState);

        if (_gameStateText != null)
        {
            _gameStateText.text = GetGameStateText(gameState, out Color color);
            _gameStateText.color = color;
        }
    }
    private void RefreshPreparingPanel(EGameState gameState)
    {
        if (_preparingPanel != null)
        {
            _preparingPanel.SetActive(gameState == EGameState.Preparing);
        }
    }
    private void RefreshRemainingTime(float remainingTime)
    {
        float openDuration = _gameFlowManager.OpenDuration;
        float progress = openDuration > 0f
            ? 1f - Mathf.Clamp01(remainingTime / openDuration)
            : 0f;
        bool isPreparing = _gameFlowManager.GameState == EGameState.Preparing;

        if(_remainingTimeText != null)
        {
            int totalMinutes = isPreparing
                ? PreparingHour * 60
                : Mathf.FloorToInt(Mathf.Lerp(OpenHour * 60f, CloseHour * 60f, progress));
            int hour = (totalMinutes / 60) % 12;
            int minute = totalMinutes % 60;

            if (hour == 0)
            {
                hour = 12;
            }

            _remainingTimeText.text = $"{hour:00} : {minute:00}";
        }

        if(_clockImage != null)
        {
            _clockImage.fillAmount = isPreparing
                ? 0f
                : progress * ClockMaxFillAmount;
        }

        RefreshDayPeriod(progress);
    }
    private void RefreshDay(int day)
    {
        RefreshDayPeriod(GetOpenProgress());
    }

    private void RefreshDayPeriod(float progress)
    {
        if(_dayText == null)
        {
            return;
        }

        string period = progress <= MorningEndProgress
            ? "아침"
            : progress <= LunchEndProgress
            ? "점심"
            : "저녁";

        _dayText.text = $"{_gameFlowManager.CurrentDay}일차 - {period}";
    }

    private float GetOpenProgress()
    {
        if(_gameFlowManager.OpenDuration <= 0f)
        {
            return 0f;
        }

        return 1f - Mathf.Clamp01(_gameFlowManager.RemainingTime / _gameFlowManager.OpenDuration);
    }
    private void RefreshMoney(int money)
    {
        if(_moneyText == null)
        {
            return;
        }
        _moneyText.text = $"{money:N0}";
    }
    private void RefreshButton()
    {
        bool isPreparing = _gameFlowManager.GameState == EGameState.Preparing;
        bool isClosed = _gameFlowManager.GameState == EGameState.Close;

        if(_openButton != null)
        {
            _openButton.gameObject.SetActive(isPreparing);
        }
        if(_closePanel != null)
        {
            _closePanel.SetActive(isClosed);
        }
    }
    private string GetGameStateText(EGameState gameState, out Color color)
    {
        switch(gameState)
        {
            case EGameState.Preparing:
                color = Color.black;
                return "영업준비중";
            case EGameState.Open:
                color = Color.darkGreen;
                return "영업중";
            case EGameState.ClosingWait:
                color = Color.red;
                return "영업종료대기";
            case EGameState.Close:
                color = Color.red;
                return "영업종료";
            default:
                color = Color.black;
                return null;
        }
    }
}

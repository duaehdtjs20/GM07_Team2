using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_MainGame : MonoBehaviour
{
    [Header("GameFlowManager")]
    [SerializeField]
    private GameFlowManager _gameFlowManager;
    [Header("GameState")]
    [SerializeField]
    private TMP_Text _dayText;
    [SerializeField]
    private TMP_Text _remainingTimeText;
    [SerializeField]
    private TMP_Text _gameStateText;
    [Header("Money")]
    [SerializeField]
    private TMP_Text _moneyText;
    [SerializeField]
    private Transform _moneyChangeRoot;
    [SerializeField]
    private TMP_Text _moneyChangeTextPrefab;
    [SerializeField]
    private float _moneyChangeDuration;
    [Header("Buttons")]
    [SerializeField]
    private Button _openButton;
    [SerializeField]
    private Button _homeButton;
    [SerializeField]
    private Button _nextdayButton;
    [Header("ClosePanel")]
    [SerializeField]
    private GameObject _closePanel;
    [SerializeField]
    private Image _clockImage;

    [Header("Preparing Panel")]
    [SerializeField]
    private GameObject _preparingPanel;

    private List<TMP_Text> _moneyChangeList = new();

    private const int OpenHour = 10;
    private const int CloseHour = 21;
    private const int PreparingHour = 9;
    private const float MorningEndProgress = 0.33f;
    private const float LunchEndProgress = 0.66f;
    private const float ClockMaxFillAmount = (CloseHour - OpenHour) / 12f;

    #region Init
    private void Start()
    {
        if(_gameFlowManager == null)
        {
            return;
        }
        _openButton?.onClick.AddListener(OnClickOpen);
        _homeButton?.onClick.AddListener(OnclickHome);
        _nextdayButton?.onClick.AddListener(OnClickNextDay);
        _gameFlowManager.OnGameStateChanged += RefreshGameState;
        _gameFlowManager.OnRemainingTimeChanged += RefreshRemainingTime;
        _gameFlowManager.OnDayChanged += RefreshDay;
        CurrencyManager.Instance.OnMoneyChanged += RefreshMoney;
        CurrencyManager.Instance.OnMoneyTransaction += ShowMoneyChange;
        RefreshMoney(CurrencyManager.Instance.Money);
        RefreshGameState(_gameFlowManager.GameState);
        RefreshDay(_gameFlowManager.CurrentDay);
        RefreshPreparingPanel(_gameFlowManager.GameState);
    }
    private void OnDisable()
    {
        if (_gameFlowManager == null)
        {
            return;
        }
        _openButton?.onClick.RemoveListener(OnClickOpen);
        _homeButton?.onClick.RemoveListener(OnclickHome);
        _nextdayButton?.onClick.RemoveListener(OnClickNextDay);
        _gameFlowManager.OnGameStateChanged -= RefreshGameState;
        _gameFlowManager.OnRemainingTimeChanged -= RefreshRemainingTime;
        _gameFlowManager.OnDayChanged -= RefreshDay;
        CurrencyManager.Instance.OnMoneyChanged -= RefreshMoney;
        CurrencyManager.Instance.OnMoneyTransaction -= ShowMoneyChange;
    }
    #endregion

    #region GameState
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
    private string GetGameStateText(EGameState gameState, out Color color)
    {
        switch (gameState)
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
    #endregion

    #region Money
    private void RefreshMoney(int money)
    {
        if (_moneyText == null)
        {
            return;
        }
        _moneyText.text = $"{money:N0}";
        _moneyText.color = money < 0 ? Color.red : Color.black;
    }
    private void ShowMoneyChange(int amout, ECurrencyTransactionType transactionType)
    {
        if(_moneyChangeRoot == null || _moneyChangeTextPrefab == null ||
            transactionType == ECurrencyTransactionType.None ||
            transactionType ==ECurrencyTransactionType.RentExpense ||
            transactionType==ECurrencyTransactionType.WageExpense)
        {
            return;
        }

        bool isExpense = transactionType == ECurrencyTransactionType.OtherExpense ? true : false;

        TMP_Text changeText = Instantiate(_moneyChangeTextPrefab, _moneyChangeRoot);
        changeText.text = isExpense ? $"-{amout}" : $"+{amout}";
        changeText.color = isExpense ? Color.red : Color.black;

        _moneyChangeList.Add(changeText);
        StartCoroutine(ShowMoneyChangeCo(changeText));
    }
    IEnumerator ShowMoneyChangeCo(TMP_Text changeText)
    {
        yield return new WaitForSeconds(_moneyChangeDuration);

        _moneyChangeList.Remove(changeText);
        Destroy(changeText.gameObject);
    }
    #endregion

    #region Button
    private void RefreshButton()
    {
        bool isPreparing = _gameFlowManager.GameState == EGameState.Preparing;
        bool isClosed = _gameFlowManager.GameState == EGameState.Close;

        if (_openButton != null)
        {
            _openButton.gameObject.SetActive(isPreparing);
        }
        if (_homeButton != null)
        {
            _homeButton.gameObject.SetActive(isPreparing);
        }
        if (_closePanel != null)
        {
            _closePanel.SetActive(isClosed);
        }
    }
    private void OnclickHome()
    {
        SceneManager.LoadScene(ESceneName.Title.ToString());
    }
    private void OnClickOpen()
    {
        _gameFlowManager.OnClickOpen();
    }
    private void OnClickNextDay()
    {
        _gameFlowManager.OnClickNextDay();
    }
    #endregion
}

using System;
using UnityEngine;

public class DailySettlementManager : MonoBehaviour
{
    [SerializeField]
    private GameFlowManager _gameFlowManager;
    [SerializeField]
    private Restaurant _restaurant;

    private DailySettlementData _dailySettlementData = new DailySettlementData();
    private bool _hasStarted;
    private bool _currencySubscribed;
    private bool _gameFlowSubscribed;

    public DailySettlementData DailySettlementData => _dailySettlementData;

    public Action<DailySettlementData> OnSettlementCompleted;
    private void Start()
    {
        _hasStarted = true;
        SubscribeEvents();

        if (_gameFlowManager != null && _dailySettlementData.Day <= 0)
        {
            _dailySettlementData.Day = Mathf.Max(1, _gameFlowManager.CurrentDay);
        }
    }
    private void OnEnable()
    {
        if (_hasStarted)
        {
            SubscribeEvents();
        }
    }
    private void OnDisable()
    {
        UnsubscribeEvents();
    }
    private void SubscribeEvents()
    {
        if (!_currencySubscribed && CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnMoneyTransaction += OnMoneyChanged;
            _currencySubscribed = true;
        }

        if (!_gameFlowSubscribed && _gameFlowManager != null)
        {
            _gameFlowManager.OnDayChanged += OnDayChanged;
            _gameFlowManager.OnGameStateChanged += OnGameStateChanged;
            _gameFlowSubscribed = true;
        }
    }
    private void UnsubscribeEvents()
    {
        if (_currencySubscribed && CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnMoneyTransaction -= OnMoneyChanged;
        }

        if (_gameFlowSubscribed && _gameFlowManager != null)
        {
            _gameFlowManager.OnDayChanged -= OnDayChanged;
            _gameFlowManager.OnGameStateChanged -= OnGameStateChanged;
        }

        _currencySubscribed = false;
        _gameFlowSubscribed = false;
    }
    public void RecordCustomerVisit()
    {
        _dailySettlementData.CustomerCount++;
    }
    private void OnMoneyChanged(int amount, ECurrencyTransactionType type)
    {
        if (_dailySettlementData == null)
        {
            return;
        }

        switch (type)
        {
            case ECurrencyTransactionType.None:
                break;
            case ECurrencyTransactionType.Sale:
                RecordCustomerVisit();
                _dailySettlementData.SalesRevenue += amount;
                break;
            case ECurrencyTransactionType.Tip:
                _dailySettlementData.TipRevenue += amount;
                break;
            case ECurrencyTransactionType.RentExpense:
                _dailySettlementData.RentExpense += amount;
                break;
            case ECurrencyTransactionType.WageExpense:
                _dailySettlementData.WageExpense += amount;
                break;
            case ECurrencyTransactionType.OtherExpense:
                _dailySettlementData.OtherExpense += amount;
                break;
        }
    }
    private void OnGameStateChanged(EGameState gameState)
    {
        switch (gameState)
        {
            case EGameState.Close:
                ChargeRent();
                ChargeWages();
                OnSettlementCompleted?.Invoke(_dailySettlementData);
                return;
        }
    }
    private void OnDayChanged(int day)
    {
        _dailySettlementData = new DailySettlementData { Day = day };
    }

    private void ChargeRent()
    {
        if (_restaurant == null || CurrencyManager.Instance == null)
        {
            return;
        }

        int rent = _restaurant.Rent;
        if (!CurrencyManager.Instance.TrySpendMoney(rent,ECurrencyTransactionType.RentExpense))
        {
            Debug.LogWarning("임대료 부족");
        }
    }

    private void ChargeWages()
    {
        if(_restaurant == null || CurrencyManager.Instance == null)
        {
            return;
        }
        if (!CurrencyManager.Instance.TrySpendMoney(_restaurant.TotalWage, ECurrencyTransactionType.WageExpense))
        {
            Debug.LogWarning("돈 부족");
        }
    }
}


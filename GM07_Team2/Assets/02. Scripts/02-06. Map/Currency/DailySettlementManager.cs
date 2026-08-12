using System;
using UnityEngine;

public class DailySettlementManager : MonoBehaviour
{
    [SerializeField]
    private GameFlowManager _gameFlowManager;
    [SerializeField]
    private Restaurant _restaurant;

    private DailySettlementData _dailySettlementData = new DailySettlementData();
    public DailySettlementData DailySettlementData => _dailySettlementData;

    public Action<DailySettlementData> OnSettlementCompleted;

    private void OnEnable()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnMoneyTransaction += OnMoneyChanged;
        }
        if(_gameFlowManager != null)
        {
            _gameFlowManager.OnDayChanged += OnDayChanged;
            _gameFlowManager.OnGameStateChanged += OnGameStateChanged;
        }
    }

    private void OnDisable()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnMoneyTransaction -= OnMoneyChanged;
        }
        if (_gameFlowManager != null)
        {
            _gameFlowManager.OnDayChanged -= OnDayChanged;
            _gameFlowManager.OnGameStateChanged -= OnGameStateChanged;
        }
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


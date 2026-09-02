using System;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class CurrencyManager : MonoBehaviourSingleton<CurrencyManager>
{
    [SerializeField, Min(0)]
    private int _initialMoney;

    public int Money { get; private set; }

    public Action<int, ECurrencyTransactionType> OnMoneyTransaction;
    public Action<int> OnMoneyChanged;

    public void InitNewGame()
    {
        InitMoney(_initialMoney);
    }
    public void InitMoney(int money)
    {
        Money = money;
        OnMoneyChanged?.Invoke(Money);
    }
    public void AddMoney(int amount, ECurrencyTransactionType transactionType)
    {
        if(amount <= 0)
        {
            return;
        }
        Money += amount;
        OnMoneyChanged?.Invoke(Money);
        OnMoneyTransaction?.Invoke(amount, transactionType);
    }

    public bool TrySpendMoney(int amount, ECurrencyTransactionType transactionType)
    {
        if (amount < 0)
        {
            return false;
        }

        bool isDailySettlement = transactionType == ECurrencyTransactionType.RentExpense ||
                                 transactionType == ECurrencyTransactionType.WageExpense;
        if(Money<amount && !isDailySettlement)
        {
            if(UI_ToastMessage.Instance != null)
            {
                UI_ToastMessage.Instance.Show("���� �����մϴ�");
                AudioManager.Instance?.PlaySFX(EAudioType.ButtonFail);
            }
            return false;
        }
        Money -= amount;
        OnMoneyChanged?.Invoke(Money);
        OnMoneyTransaction?.Invoke(amount, transactionType);
        if(transactionType == ECurrencyTransactionType.OtherExpense)
        {
            AudioManager.Instance?.PlaySFX(EAudioType.Upgrade);
        }
        else if(isDailySettlement)
        {
            return true;
        }
        else
        {
            AudioManager.Instance?.PlaySFX(EAudioType.Coin);
        }
        return true;
    }
}

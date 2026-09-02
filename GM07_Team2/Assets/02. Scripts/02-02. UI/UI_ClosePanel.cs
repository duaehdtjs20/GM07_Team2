using TMPro;
using UnityEngine;

public class UI_ClosePanel : MonoBehaviour
{
    [SerializeField]
    private DailySettlementManager _settlementManager;
    [SerializeField]
    private TMP_Text _dayText;
    [SerializeField]
    private TMP_Text _settlementText;

    private void OnEnable()
    {
        if(_settlementManager == null)
        {
            return;
        }
        _settlementManager.OnSettlementCompleted += RefreshSettlement;
        if (_settlementManager.DailySettlementData == null)
        {
            return;
        }
        RefreshSettlement(_settlementManager.DailySettlementData);
        AudioManager.Instance?.PlaySFX(EAudioType.Result);
    }

    private void OnDisable()
    {
        if (_settlementManager != null)
        {
            _settlementManager.OnSettlementCompleted -= RefreshSettlement;
        }
    }

    private void RefreshSettlement(DailySettlementData data)
    {
        if(_dayText == null || _settlementText == null)
        {
            return;
        }

        _dayText.text = $"{data.Day}일차 정산\n";
        _settlementText.text =
            $"{data.CustomerCount}\n\n" +
            $"{data.SalesRevenue:N0}\n" +
            $"{data.TipRevenue:N0}\n\n" +
            $"{data.TotalRevenue:N0}\n\n\n" +
            $"{data.RentExpense:N0}\n" +
            $"{data.WageExpense:N0}\n" +
            $"{data.OtherExpense:N0}\n\n" +
            $"{data.TotalExpense:N0}\n\n" +
            $"{data.NetProfit:N0}\n" +
            $"{CurrencyManager.Instance.Money:N0}";
    }
}

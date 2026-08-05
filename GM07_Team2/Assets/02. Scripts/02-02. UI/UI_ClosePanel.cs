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
            $"{data.CustomerCount}명의 손님 방문\n" +
            $"판매 금액 : {data.SalesRevenue:N0}\n" +
            $"팁 : {data.TipRevenue:N0}\n" +
            $"총 매출 : {data.TotalRevenue:N0}\n" +
            $"가게 임대료 : {data.RentExpense:N0}\n" +
            $"직원 임금 : {data.WageExpense:N0}\n" +
            $"기타 지출 : {data.OtherExpense:N0}\n" +
            $"순이익 : {data.NetProfit:N0}\n" +
            $"현재 자금 : {CurrencyManager.Instance.Money:N0}";
    }
}

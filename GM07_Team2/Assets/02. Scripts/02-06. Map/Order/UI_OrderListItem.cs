using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GM07.Order
{
    public class UI_OrderListItem : MonoBehaviour
    {
        
        [SerializeField]
        private TMP_Text _seatInfoText;

        
        [SerializeField]
        private Button _startCookButton;

        private OrderData _order;
        private Action<OrderData> _onClickStartCook;

        private void OnEnable()
        {
            _startCookButton.onClick.AddListener(OnClickStartCook);
        }

        private void OnDisable()
        {
            _startCookButton.onClick.RemoveListener(OnClickStartCook);
        }

        public void InitItem(OrderData order, Action<OrderData> onClickStartCook)
        {
            _order = order;
            _onClickStartCook = onClickStartCook;
            RefreshItem();
        }

        private void RefreshItem()
        {
            // TODO: 메뉴 데이터 연결 후 메뉴 이름/가격/이미지 표시 추가
            _seatInfoText.text = $"{_order.Seat.SeatId}번 좌석";
        }

        private void OnClickStartCook()
        {
            _onClickStartCook?.Invoke(_order);
        }
    }
}
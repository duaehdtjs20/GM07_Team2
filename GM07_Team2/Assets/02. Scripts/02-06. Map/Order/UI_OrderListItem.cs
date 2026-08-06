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
        private Button _actionButton;

        [SerializeField]
        private TMP_Text _actionButtonText;

        [SerializeField]
        private GameObject _timerObject;

        [SerializeField]
        private TMP_Text _timerText;

        private OrderData _order;
        private Action<OrderData> _onClickAction;

        private void OnEnable()
        {
            _actionButton.onClick.AddListener(OnClickActionButton);
        }

        private void OnDisable()
        {
            _actionButton.onClick.RemoveListener(OnClickActionButton);
        }

        private void Update()
        {
            if (_order == null || _order.State != EOrderState.Cooking)
            {
                return;
            }

            RefreshTimerText();
        }

        // isTableCooking: 이 테이블에서 (다른 주문이) 조리중인지 여부
        public void InitItem(OrderData order, bool isTableCooking, Action<OrderData> onClickAction)
        {
            _order = order;
            _onClickAction = onClickAction;
            RefreshItem(isTableCooking);
        }

        private void RefreshItem(bool isTableCooking)
        {
            // TODO: 메뉴 데이터 연결 후 메뉴 이름/가격/이미지 표시 추가
            _seatInfoText.text = $"{_order.Seat.SeatId}번 좌석";

            bool isCooking = _order.State == EOrderState.Cooking;
            _actionButton.gameObject.SetActive(!isCooking);
            _timerObject.SetActive(isCooking);

            if (isCooking)
            {
                RefreshTimerText();
                return;
            }

            bool isReady = _order.State == EOrderState.Ready;
            _actionButtonText.text = isReady ? "서빙" : "요리시작";
            _actionButton.interactable = isReady || !isTableCooking;
        }

        private void RefreshTimerText()
        {
            float cookingTime = _order.Recipe.Data.CookingTime;
            float remainingTime = Mathf.Max(cookingTime - (Time.time - _order.CookStartTime), 0.0f);
            _timerText.text = $"{remainingTime:F1}초";
        }

        private void OnClickActionButton()
        {
            _onClickAction?.Invoke(_order);
        }
    }
}
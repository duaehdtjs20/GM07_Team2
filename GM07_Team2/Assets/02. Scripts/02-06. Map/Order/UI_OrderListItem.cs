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
        private Image _recipeIconImage;

        [SerializeField]
        private TMP_Text _recipeNameText;

        [SerializeField]
        private TMP_Text _priceText;

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

            //RefreshTimerText();
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
            _seatInfoText.text = $"{_order.Seat.SeatId+1}번 좌석";
            RefreshRecipeInfo();

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

        private void RefreshRecipeInfo()
        {
            RecipeData data = _order.Recipe.Data;
            _recipeIconImage.sprite = data.Icon;
            _recipeNameText.text = data.Name;
            _priceText.text = $"{data.Price}원";
        }

        private void RefreshTimerText()
        {
            float cookingTime = _order.Recipe.Data.CookingTime;
            if (_order.Staff != null)
            {
                cookingTime /= _order.Staff.CookSpeed;
            }
            float remainingTime = Mathf.Max(cookingTime - (Time.time - _order.CookStartTime), 0.0f);
            _timerText.text = $"{remainingTime:F1}초";
        }

        private void OnClickActionButton()
        {
            _onClickAction?.Invoke(_order);
        }
    }
}

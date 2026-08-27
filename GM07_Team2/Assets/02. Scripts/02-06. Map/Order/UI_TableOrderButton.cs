using UnityEngine;
using UnityEngine.EventSystems;

namespace GM07.Order
{
    public class UI_TableOrderButton : MonoBehaviour, IPointerDownHandler
    {
        
        [SerializeField]
        private TableOrderController _table;

        [SerializeField]
        private GameObject _exclamationMarkObject;

        private Vector2 _screenPosition;

        private void OnEnable()
        {
            _table.OnOrderListChanged += HandleOrderListChanged;
        }

        private void OnDisable()
        {
            _table.OnOrderListChanged -= HandleOrderListChanged;
        }

        private void HandleOrderListChanged()
        {
            bool hasOrder = _table.Orders.Count > 0;
            RefreshExclamationMark(hasOrder);
        }

        private void RefreshExclamationMark(bool isVisible)
        {
            _exclamationMarkObject.SetActive(isVisible);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _screenPosition = eventData.position;
        }

        public void OnClickButton()
        {
            UI_OrderRequestWindowManager.Instance.OpenWindow(_table, _screenPosition);
        }
    }
}
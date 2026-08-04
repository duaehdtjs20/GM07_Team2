using UnityEngine;

namespace GM07.Order
{
    public class UI_TableOrderButton : MonoBehaviour
    {
        
        [SerializeField]
        private TableOrderController _table;

        
        [SerializeField]
        private GameObject _exclamationMarkObject;

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

        public void OnClickButton()
        {
            UI_OrderRequestWindowManager.Instance.OpenWindow(_table);
        }
    }
}
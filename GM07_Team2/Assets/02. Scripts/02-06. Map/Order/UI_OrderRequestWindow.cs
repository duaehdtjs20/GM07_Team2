using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GM07.Order
{
    public class UI_OrderRequestWindow : MonoBehaviour
    {
        [SerializeField]
        private Button _closeButton;

        [SerializeField]
        private Transform _listContentParent;

        [SerializeField]
        private UI_OrderListItem _orderListItemPrefab;

        private TableOrderController _table;
        private readonly List<UI_OrderListItem> _spawnedItems = new();

        private void OnEnable()
        {
            _closeButton.onClick.AddListener(OnClickClose);
        }

        private void OnDisable()
        {
            _closeButton.onClick.RemoveListener(OnClickClose);
            UnsubscribeTable();
        }

        public void InitWindow(TableOrderController table)
        {
            UnsubscribeTable();
            _table = table;
            _table.OnOrderListChanged += RefreshList;
            RefreshList();
        }

        private void UnsubscribeTable()
        {
            if (_table != null)
            {
                _table.OnOrderListChanged -= RefreshList;
            }
        }

        private void RefreshList()
        {
            ClearItems();

            bool isTableCooking = _table.IsCooking;

            foreach (OrderData order in _table.Orders)
            {
                UI_OrderListItem item = Instantiate(_orderListItemPrefab, _listContentParent);
                item.InitItem(order, isTableCooking, OnClickAction);
                _spawnedItems.Add(item);
            }
        }

        private void ClearItems()
        {
            foreach (UI_OrderListItem item in _spawnedItems)
            {
                Destroy(item.gameObject);
            }
            _spawnedItems.Clear();
        }

        // 아이템의 상태에 따라 요리시작 / 서빙으로 분기
        private void OnClickAction(OrderData order)
        {
            if (order.State == EOrderState.Ready)
            {
                _table.ServeOrder(order);
            }
            else
            {
                _table.StartCooking(order);
            }
        }

        private void OnClickClose()
        {
            UI_OrderRequestWindowManager.Instance.CloseWindow();
        }
    }
}
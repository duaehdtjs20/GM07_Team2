using UnityEngine;

namespace GM07.Order
{
    public class UI_OrderRequestWindowManager : MonoBehaviourSingleton<UI_OrderRequestWindowManager>
    {
        
        [SerializeField]
        private UI_OrderRequestWindow _orderRequestWindow;

        public void OpenWindow(TableOrderController table)
        {
            _orderRequestWindow.gameObject.SetActive(true);
            _orderRequestWindow.InitWindow(table);
        }

        public void CloseWindow()
        {
            _orderRequestWindow.gameObject.SetActive(false);
        }
    }
}
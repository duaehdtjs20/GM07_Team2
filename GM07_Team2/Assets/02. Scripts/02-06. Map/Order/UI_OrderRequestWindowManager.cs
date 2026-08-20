using UnityEngine;

namespace GM07.Order
{
    public class UI_OrderRequestWindowManager : MonoBehaviourSingleton<UI_OrderRequestWindowManager>
    {
        
        [SerializeField]
        private UI_OrderRequestWindow _orderRequestWindow;

        private bool _isBlock = false;
        public void OpenWindow(TableOrderController table)
        {
            if (_isBlock)
            {
                return;
            }

            _orderRequestWindow.gameObject.SetActive(true);
            _orderRequestWindow.InitWindow(table);
        }

        public void CloseWindow()
        {
            _orderRequestWindow.gameObject.SetActive(false);
        }

        public void SwitchBlockWindow(bool isBlock) 
        {
            _isBlock = isBlock;
        }

    }
}
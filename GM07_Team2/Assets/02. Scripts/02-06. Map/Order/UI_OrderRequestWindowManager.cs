using UnityEngine;

namespace GM07.Order
{
    public class UI_OrderRequestWindowManager : MonoBehaviourSingleton<UI_OrderRequestWindowManager>
    {
        
        [SerializeField]
        private UI_OrderRequestWindow _orderRequestWindow;

        private bool _isBlock = false;
        private OrderWindowOpenEffect _effect;

        private void Start()
        {
            _effect = GetComponent<OrderWindowOpenEffect>();
        }
        public void OpenWindow(TableOrderController table, Vector2 screenPosition)
        {
            if (_isBlock)
            {
                return;
            }

            _orderRequestWindow.gameObject.SetActive(true);
            _orderRequestWindow.InitWindow(table);
            _effect?.PlayFrom(screenPosition);
        }

        public void CloseWindow()
        {
            _effect?.Kill();
            _orderRequestWindow.gameObject.SetActive(false);
        }

        public void SwitchBlockWindow(bool isBlock) 
        {
            _isBlock = isBlock;
        }

    }
}
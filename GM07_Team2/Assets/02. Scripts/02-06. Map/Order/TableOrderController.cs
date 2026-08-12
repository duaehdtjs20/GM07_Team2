using System;
using System.Collections.Generic;
using GM07.Map;
using UnityEngine;

namespace GM07.Order
{
    public class TableOrderController : MonoBehaviour
    {
        public Action OnOrderListChanged;

        [SerializeField]
        private Table _table;
        [SerializeField]
        private Restaurant _restaurant;

        private readonly List<OrderData> _orders = new();

        public IReadOnlyList<OrderData> Orders => _orders;

        // 테이블에 요리사가 한 명 고정이므로, 조리중인 주문이 있으면 true
        public bool IsCooking => _orders.Exists(order => order.State == EOrderState.Cooking);

        private void Awake()
        {
            if(_table == null)
            {
                _table = GetComponent<Table>();
            }
            if(_restaurant == null)
            {
                _restaurant = FindFirstObjectByType<Restaurant>();
            }
        }

        private void Update()
        {
            UpdateCookingOrders();
        }

        // 동선님 파트(손님 착석 완료 시점)에서 호출
        // seat: 손님이 착석한 좌석 정보
        // customer: 주문한 손님
        // recipe: 주문한 메뉴
        public void RequestOrder(Seat seat, Customer customer, Recipe recipe)
        {
            _restaurant.TryGetStaffIndex(_table.TableId - 1, out Staff staff);
            OrderData order = new OrderData
            {
                Seat = seat,
                OrderRequestTime = Time.time,
                Customer = customer,
                Recipe = recipe,
                Staff = staff
            };
            _orders.Add(order);
            OnOrderListChanged?.Invoke();
        }

        // 주문 확인 창에서 "요리시작" 눌렀을 때 호출
        // 이미 조리중인 주문이 있으면 무시 (테이블당 요리사 1명)
        public void StartCooking(OrderData order)
        {
            if (IsCooking || order.State != EOrderState.Waiting)
            {
                return;
            }

            order.State = EOrderState.Cooking;
            order.CookStartTime = Time.time;
            OnOrderListChanged?.Invoke();
        }

        // 주문 확인 창에서 "서빙" 눌렀을 때 호출
        public void ServeOrder(OrderData order)
        {
            if (order.State != EOrderState.Ready)
            {
                return;
            }

            _orders.Remove(order);
            order.Customer.Receive();
            OnOrderListChanged?.Invoke();
        }

        // 조리중인 주문의 완성 여부를 매 프레임 확인, 완성되면 Ready로 전환
        private void UpdateCookingOrders()
        {
            bool isChanged = false;

            foreach (OrderData order in _orders)
            {
                if (order.State != EOrderState.Cooking)
                {
                    continue;
                }

                float cookingTime = order.Recipe.Data.CookingTime;
                if(order.Staff != null)
                {
                    cookingTime /= order.Staff.CookSpeed;
                }
                if (Time.time - order.CookStartTime >= cookingTime)
                {
                    order.State = EOrderState.Ready;
                    isChanged = true;
                }
            }

            if (isChanged)
            {
                OnOrderListChanged?.Invoke();
            }
        }
    }
}

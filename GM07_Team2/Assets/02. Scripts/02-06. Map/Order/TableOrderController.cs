using System;
using System.Collections.Generic;
using GM07.Map;
using UnityEngine;

namespace GM07.Order
{
    public class TableOrderController : MonoBehaviour
    {
        
        public Action OnOrderListChanged;

        private readonly List<OrderData> _orders = new();

        public IReadOnlyList<OrderData> Orders => _orders;

        // 동선님 파트(손님 착석 완료 시점)에서 호출
        // seat: 손님이 착석한 좌석 정보
        public void RequestOrder(Seat seat)
        {
            OrderData order = new OrderData
            {
                Seat = seat,
                OrderRequestTime = Time.time
            };
            _orders.Add(order);
            OnOrderListChanged?.Invoke();
        }

        // 주문 확인 창에서 "요리시작" 눌렀을 때 호출
        public void StartCooking(OrderData order)
        {
            _orders.Remove(order);
            OnOrderListChanged?.Invoke();
            // TODO: 요리 파트 진행 시 실제 조리 로직 연결
        }
    }
}
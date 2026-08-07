using UnityEngine;
// 빈 자리에 도착하고 나서 자리에 앉고 주문 요청하는 클래스
public class CustomerSeat : CustomerStateBase
{
    public CustomerSeat(Customer customer, Animator animator) : base(customer, animator, Animator.StringToHash("Sit")) { }
    public override void Enter()
    {
        base.Enter();

        // 위치와 회전 맞추기
        _customer.SetOffsetSeat();

        // 주문 신청 알림
        _customer.OrderMenu();
    }
    public override void Update()
    {
        // 음식을 받으면 식사 상태로 변경
        if (_customer.IsReceived)
        {
            _customer.StateMachine.TransitionTo(_customer.StateMachine.EatState);
        }
    }
}

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
        // 음식을 받은 경우
        if (_customer.IsReceived)
        {
            // 음식 실패 시 접시 치우고 화냄
            if (_customer.Quality == GM07.Order.EQuality.Fail)
            {
                _customer.ClearDish();
                _customer.StateMachine.TransitionTo(_customer.StateMachine.AngryState);
            }
            // 나머지 경우 먹기
            else
            {
                _customer.StateMachine.TransitionTo(_customer.StateMachine.EatState);
            }
            return;
        }
        // WaitTime 만큼 기다린 경우
        if (_customer.IsWaited)
        {
            // 주문 취소 후 화내기
            _customer.CancelOrder();
            _customer.ShowQualityIcon(GM07.Order.EQuality.Fail);
            _customer.StateMachine.TransitionTo(_customer.StateMachine.AngryState);
            return;
        }
        // 기다리기
        _customer.Waiting();
    }
}

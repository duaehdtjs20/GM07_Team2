using UnityEngine;

// 스폰 직후 빈자리 찾아서 이동하는 상태
public class CustomerEnter : CustomerStateBase
{
    public CustomerEnter(Customer customer) : base(customer) { }
    public override void Enter()
    {
        // 빈 자리를 목표로 지정
        _customer.SetDestination(_customer.Seat.Anchor);

        // 상태 변경 임시 확인
        _customer.SetColor(Color.red);
    }
    public override void Update()
    {
        // 일정 거리 이상 일 때는 무시
        if(_customer.CalculateSqrMagnitude() > 1f)
        {
            return;
        }
        // 상태 변경
        _customer.StateMachine.TransitionTo(_customer.StateMachine.SeatState);
    }
    public override void Exit()
    {
    }
}

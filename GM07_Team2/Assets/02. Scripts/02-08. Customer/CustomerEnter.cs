using UnityEngine;

// 스폰 직후 빈자리 찾아서 이동하는 상태
public class CustomerEnter : CustomerStateBase
{
    public CustomerEnter(Customer customer, Animator animator) : base(customer, animator, Animator.StringToHash("Walk")) { }
    public override void Enter()
    {
        base.Enter();

        // 빈 자리를 목표로 지정
        _customer.SetDestination(_customer.Seat.Anchor);
    }
    public override void Update()
    {
        // 일정 거리 이상 일 때는 무시
        if(_customer.CalculateSqrMagnitude() > 2f)
        {
            return;
        }
        // 상태 변경
        _customer.StopAgent();
        _customer.StateMachine.TransitionTo(_customer.StateMachine.SeatState);
    }
}

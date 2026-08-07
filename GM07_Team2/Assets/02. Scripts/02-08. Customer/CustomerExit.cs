using UnityEngine;

// 
public class CustomerExit : CustomerStateBase
{
    public CustomerExit(Customer customer, Animator animator) : base(customer, animator, Animator.StringToHash("Walk")) { }
    public override void Enter()
    {
        base.Enter();

        _customer.SetOffsetStandUp();
        // 목적지를 출구(시작 위치)로 지정
        _customer.SetDestination(_customer.StartPos);
    }
    public override void Update()
    {
        // 일정 거리 이상 일 때는 무시
        if(_customer.CalculateSqrMagnitude() > 1f)
        {
            return;
        }
        // 출구로 나가면 결제
        _customer.PayMoney();

        // 반환
        _customer.Release();
    }
}

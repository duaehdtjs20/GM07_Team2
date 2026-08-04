using UnityEngine;

// 빈 자리에 도착하고 나서 자리에 앉고 주문 요청하는 클래스
public class CustomerSeat : CustomerStateBase
{
    public CustomerSeat(Customer customer) : base(customer) { }
    public override void Enter()
    {
        // 자리에 앉기

        // 주문 신청 알림
    }
    public override void Update()
    {
        // 대기 타이머 초과되면 퇴장(확장 영역)

        // 주문 접수 완료되면 (타이머 멈추고) 음식 대기
        
    }
    public override void Exit()
    {

    }
}

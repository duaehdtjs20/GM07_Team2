using GM07.Map;

using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Customer : MonoBehaviour
{
    [Header("손님 데이터")]
    [SerializeField]
    private CustomerData _data;
    [Header("네비게이션")]
    [SerializeField]
    private NavMeshAgent _agent;

    public Table Table { get; private set; }
    public Seat Seat { get; private set; }

    public CustomerStateMachine StateMachine { get; private set; }

    private void Update()
    {
        StateMachine.UpdateState();
    }
    public void Init(Table table, Seat seat)
    {
        Table = table;
        Seat = seat;

        if(_agent == null)
        {
            TryGetComponent(out _agent);
        }
        StateMachine = new CustomerStateMachine(this);
        StateMachine.Initialize(StateMachine.EnterState);
    }

    // parameter로 주어진 목적지 까지의 경로를 생성하는 메서드
    public void SetDestination(Transform target)
    {
        // 방어 코드
        if(target == null)
        {
            Debug.LogWarning("NULL target 접근");
            return;
        }

        if(!_agent.SetDestination(target.position))
        {
            Debug.LogWarning(transform.name + " 경로 찾기 실패");
        }
    }

    // 메뉴 선택 후 반환하는 메서드 (반환형 수정 예정)
    public void OrderMenu()
    {
        // 메뉴 선택 후 반환 로직
    }
    // 현재 설정된 목적지 까지의 거리를 반환하는 메서드
    public float CalculateDistance()
    {
        return Vector3.Distance(transform.position, _agent.destination);
    }
}

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

    private float _eatTimer = 0.0f;
    private TableManager _tableManager;

    public CustomerStateMachine StateMachine { get; private set; }
    public Table Table { get; private set; }
    public Seat Seat { get; private set; }
    public Vector3 StartPos { get; private set; }
    public bool IsAte => _eatTimer >= _data.EatTime;

    #region Test Fields
    private float _orderTimer = 0.0f; // test
    private float _orderTime = 2.0f; // test
    private float _receiveTimer = 0.0f; // test
    private float _receiveTime = 2.0f; // test
    public bool IsOrder => _orderTimer >= _orderTime; // test
    public bool IsReceiveFood => _receiveTimer >= _receiveTime; // test
    #endregion

    private void Update()
    {
        StateMachine.UpdateState();
    }

    // 스폰 시 호출되는 초기화 메서드
    public void Init(TableManager tableManager, Table table, Seat seat)
    {
        _tableManager = tableManager;
        Table = table;
        Seat = seat;

        if(_agent == null)
        {
            TryGetComponent(out _agent);
        }
        StartPos = transform.position;

        if (StateMachine == null)
        {
            StateMachine = new CustomerStateMachine(this);
        }
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
    // 오버로딩
    public void SetDestination(Vector3 target)
    {
        if(!_agent.SetDestination(target))
        {
            Debug.LogWarning(transform.name + " 경로 찾기 실패");
        }
    }

    // 현재 설정된 목적지 까지의 거리를 반환하는 메서드
    public float CalculateSqrMagnitude()
    {
        return Vector3.SqrMagnitude(transform.position -_agent.destination);
    }

    // 메뉴 주문하는 메서드
    public void OrderMenu()
    {
        // 메뉴 선택 후 반환 로직
        //Table.GetComponent<TableOrderController>().RequestOrder(Seat);

        _orderTimer = 0.0f; // test
        _receiveTimer = 0.0f; // test
    }
    public void Ordering() // test
    {
        _orderTimer += Time.deltaTime;
    }
    public void Watting() // test
    {
        _receiveTimer += Time.deltaTime;
    }
    public void Received()
    {
        // 요리 받기 로직
        
    }

    public void ResetTimer()
    {
        _eatTimer = 0.0f;
    }

    public void Eating()
    {
        _eatTimer += Time.deltaTime;
    }

    public void Release()
    {
        // 결제 로직 추가 예정
        
        // 자리 반환
        if (_tableManager != null && Table != null && Seat != null)
        {
            _tableManager.ReleaseSeat(Table, Seat);
        }
        _tableManager = null;
        Table = null;
        Seat = null;
        
        // 임시로 파괴 로직으로 구현(풀링 예정)
        Destroy(gameObject);
    }
    // 애니메이션 적용 전, 상태 변경 시각화를 위한 색상 변경 메서드
    public void SetColor(Color color)
    {
        GetComponent<Renderer>().material.color = color;
    }
}

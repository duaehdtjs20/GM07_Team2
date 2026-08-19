using System.Collections.Generic;
using System.Linq;

using GM07.Map;
using GM07.Order;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
public class Customer : MonoBehaviour
{
    [Header("손님 데이터")]
    [SerializeField]
    private CustomerData _data;
    [SerializeField]
    private CustomerModelDatas _modelDatas;
    [SerializeField]
    private UI_CustomerQualityIcon _qualityIconUI;
    [Header("네비게이션")]
    [SerializeField]
    private NavMeshAgent _agent;
    // 기능 구현 전용 필드
    private float _waitTimer = 0.0f;
    private float _eatTimer = 0.0f;
    private TableManager _tableManager;
    private Recipe _recipe;
    private OrderData _orderData;
    private Transform _model;
    private GM07.Order.EQuality _quality = GM07.Order.EQuality.Normal;
    private Stack<GameObject> _pool;
    private List<GameObject> _models = new List<GameObject>();
    private List<CustomerStateMachine> _stateMachines = new List<CustomerStateMachine>();
    public CustomerStateMachine StateMachine { get; private set; }
    public Table Table { get; private set; }
    public Seat Seat { get; private set; }
    public Vector3 StartPos { get; private set; }
    public bool IsWaited => _waitTimer >= _data.WaitTime;
    public bool IsAte => _eatTimer >= _data.EatTime;
    public bool IsReceived { get; private set; }
    public bool IsOrdered { get; private set; }
    private void Update()
    {
        StateMachine.UpdateState();
    }
    // 스폰 시 호출되는 초기화 메서드
    public void Init(TableManager tableManager, Table table, Seat seat, Stack<GameObject> pool)
    {
        _tableManager = tableManager;
        Table = table;
        Seat = seat;
        _pool = pool;
        _eatTimer = 0.0f;
        _waitTimer = 0.0f;
        IsReceived = false;
        IsOrdered = false;
        _recipe = null;
        _orderData = null;
        if (_agent == null)
        {
            TryGetComponent(out _agent);
        }
        // 최초 모델 생성
        if (_models.Count <= 0 && _modelDatas != null && _modelDatas.Count > 0)
        {
            foreach (var model in _modelDatas.Models)
            {
                // 모델 생성
                GameObject modelObj = Instantiate(model, transform);
                // 모델 초기화
                modelObj.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                modelObj.transform.localPosition = new Vector3(0.0f, -1.0f, 0.0f);
                modelObj.SetActive(false);
                // 애니메이터 캐싱
                Animator animator = modelObj.GetComponent<Animator>();
                // 리스트에 넣어서 모델 관리
                _models.Add(modelObj);
                // 캐싱한 애니메이터로 상태머신 생성 후 리스트에 보관
                _stateMachines.Add(new CustomerStateMachine(this, animator));
            }
        }
        // 랜덤 모델과 그에 맞는 상태 머신 불러오기
        int randomIndex = Random.Range(0, _models.Count);
        _model = _models[randomIndex].transform;
        StateMachine = _stateMachines[randomIndex];
        // 활성화 후 위치, 회전, 상태 초기화
        _model.gameObject.SetActive(true);
        StartPos = transform.position;
        transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
        StateMachine.Initialize(StateMachine.EnterState);
    }
    // parameter로 주어진 목적지 까지의 경로를 생성하는 메서드
    public void SetDestination(Transform target)
    {
        // 방어 코드
        if (target == null)
        {
            Debug.LogWarning("NULL target 접근");
            return;
        }
        if (!_agent.SetDestination(target.position))
        {
            Debug.LogWarning(transform.name + " 경로 찾기 실패");
        }
    }
    // 오버로딩
    public void SetDestination(Vector3 target)
    {
        if (!_agent.SetDestination(target))
        {
            Debug.LogWarning(transform.name + " 경로 찾기 실패");
        }
    }
    // 의자에 앉기 전 이동을 멈추는 메서드
    public void StopAgent()
    {
        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;
    }
    // 현재 설정된 목적지 까지의 거리를 반환하는 메서드
    public float CalculateSqrMagnitude()
    {
        return Vector3.SqrMagnitude(transform.position - _agent.destination);
    }
    // 의자에 맞춰 위치와 회전을 초기화 하는 메서드
    public void SetOffsetSeat()
    {
        if (Seat == null)
        {
            return;
        }
        int id = Seat.SeatId;
        id /= 2;
        float rotate = 90.0f * (id + 2);
        transform.rotation = Quaternion.Euler(0.0f, rotate, 0.0f);
        _model.position = Seat.Anchor.position;
        _model.localPosition += new Vector3(0.0f, 0.7f, 0.3f);
    }
    // 의자에서 일어날 때 위치와 회전을 초기화 하는 메서드
    public void SetOffsetStandUp()
    {
        if (Seat == null)
        {
            return;
        }
        int id = Seat.SeatId;
        id /= 2;
        float rotate = 90.0f * id;
        transform.rotation = Quaternion.Euler(0.0f, rotate, 0.0f);
        _model.localPosition = new Vector3(0.0f, -1.0f, 0.0f);
        _agent.isStopped = false;
    }
    // 메뉴 주문하는 메서드
    public void OrderMenu()
    {
        if (RecipeManager.Instance != null && Table.TryGetComponent(out TableOrderController order))
        {
            List<Recipe> recipes = new List<Recipe>();
            for (int i = 0; i < RecipeManager.Instance.Count; i++)
            {
                if (RecipeManager.Instance.TryGetRecipeIndex(i, out Recipe recipe) && recipe.Unlocked)
                {
                    recipes.Add(recipe);
                }
            }
            // 해금된 레시피 랜덤으로 선택
            Recipe selectRecipe = recipes[Random.Range(0, recipes.Count)];
            _recipe = selectRecipe;
            // 주문 요청 (좌석/손님/레시피 정보 전달)
            order.RequestOrder(Seat, this, selectRecipe);
            _orderData = order.Orders.FirstOrDefault(c => c.Customer == this);
        }
    }
    // 주문 취소 메서드
    public void CancelOrder()
    {
        _recipe = null;
        if (Table.TryGetComponent(out TableOrderController order))
        {
            // 주문 취소
            order.CancelOrder(this);
        }
    }
    public void Waiting()
    {
        if(_orderData.State == EOrderState.Waiting)
        {
            _waitTimer += Time.deltaTime;
        }
    }
    public void Eating()
    {
        _eatTimer += Time.deltaTime;
    }
    public void ClearDish()
    {
        if (Table.TryGetComponent(out TableOrderController order))
        {
            order.SetDishActive(_orderData.Seat.SeatId, false);
        }
    }
    public void Receive(GM07.Order.EQuality quality)
    {
        IsReceived = true;
        _quality = quality;
    }
    // 조리 완료(완성도 결정) 시점에 TableOrderController에서 호출 — 손님 머리 위에 완성도 이모티콘 표시
    public void ShowQualityIcon(GM07.Order.EQuality quality)
    {
        if (_qualityIconUI != null)
        {
            _qualityIconUI.ShowQuality(quality);
        }
    }
    public void PayMoney()
    {
        if (CurrencyManager.Instance != null && _recipe != null)
        {
            float multiplier = 1.0f;
            if (QualityManager.Instance != null)
            {
                multiplier = QualityManager.Instance.GetPriceMultiplier(_quality);
            }
            int finalPrice = Mathf.RoundToInt(_recipe.Data.Price * multiplier);
            CurrencyManager.Instance.AddMoney(finalPrice, ECurrencyTransactionType.Sale);
        }
    }
    public void Release()
    {
        // 자리 반환
        if (_tableManager != null && Table != null && Seat != null)
        {
            _tableManager.ReleaseSeat(Table, Seat);
        }
        _tableManager = null;
        Table = null;
        Seat = null;
        _pool.Push(gameObject);
        _model.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
    private void OnDrawGizmos()
    {
        if (_agent.path == null)
        {
            return;
        }
        Gizmos.color = Color.green;
        for (int i = 0; i < _agent.path.corners.Length - 1; i++)
        {
            Gizmos.DrawLine(_agent.path.corners[i], _agent.path.corners[i + 1]);
        }
    }
}

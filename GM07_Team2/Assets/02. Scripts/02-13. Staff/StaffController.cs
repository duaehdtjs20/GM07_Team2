using System.Collections;

using GM07.Map;
using GM07.Order;

using UnityEngine;
using UnityEngine.AI;

public class StaffController : MonoBehaviour
{
    [Header("Component Fields")]
    [SerializeField]
    private GameFlowManager _flowManager;
    [SerializeField]
    private TableOrderController _orderController;
    [SerializeField]
    private Table _table;
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private NavMeshAgent _agent;
    [Header("Object Fields")]
    [SerializeField]
    private GameObject[] _handKnife;
    [SerializeField]
    private GameObject _tableKnife;
    [SerializeField]
    private GameObject _fish;
    [SerializeField]
    private Transform _cookPos;
    [SerializeField]
    private Transform _washPos;

    private Staff _staff;
    private int _upgrade = 1;
    private bool _switching = false;

    void Awake()
    {
        if (_flowManager == null)
        {
            _flowManager = FindFirstObjectByType<GameFlowManager>();
        }
        if (_agent == null)
        {
            _agent = GetComponent<NavMeshAgent>();
        }
    }
    private void Start()
    {
        if (_table != null && _orderController != null)
        {
            if (_orderController.Restaurant != null
                && _orderController.Restaurant.TryGetStaffIndex(_table.TableId - 1, out Staff staff))
            {
                _staff = staff;
                _upgrade = _staff.Upgrade;
                _staff.OnUpgraded += RefreshModel;
            }
        }
        _animator = transform.GetChild(_upgrade - 1).GetComponent<Animator>();
        StartCoroutine(StateStreamCo());
    }
    private void OnDestroy()
    {
        if (_staff != null)
        {
            _staff.OnUpgraded -= RefreshModel;
        }
    }
    IEnumerator StateStreamCo()
    {
        if (_flowManager == null)
        {
            yield break;
        }
        while (true)
        {
            if (_flowManager.GameState == EGameState.Open)
            {
                if (_switching)
                {
                    yield return MoveCo(_cookPos);
                    ObjectActivate(true);
                    yield return StateCo("Cook", Random.Range(6.0f, 10.0f));
                    ObjectActivate(false);
                }
                else
                {
                    yield return MoveCo(_washPos);
                    yield return StateCo("Wash", Random.Range(1.0f, 3.0f));
                }
                _switching = !_switching;
            }
            else
            {
                yield return null;
            }
        }
    }
    IEnumerator StateCo(string name, float time)
    {
        _animator.SetBool(name, true);
        yield return new WaitForSeconds(time);
        _animator.SetBool(name, false);
    }
    IEnumerator MoveCo(Transform target)
    {
        _agent.SetDestination(target.position); 
        transform.LookAt(target.position);
        _animator.SetBool("Move", true);
        while(_agent.remainingDistance > _agent.stoppingDistance)
        {
            yield return null;
        }
        _animator.SetBool("Move", false);
        transform.rotation = target.rotation;
    }
    private void ObjectActivate(bool flag)
    {
        if (_handKnife == null || _tableKnife == null || _fish == null)
        {
            return;
        }
        if (_upgrade - 1 < 0 || _upgrade - 1 >= _handKnife.Length)
        {
            return;
        }
        _handKnife[_upgrade - 1].SetActive(flag);
        _tableKnife.SetActive(!flag);
        _fish.SetActive(flag);
    }
    private void RefreshModel()
    {
        if (_staff.Upgrade < 0 || _staff.Upgrade - 1 >= _handKnife.Length || _staff.Upgrade - 1 >= transform.childCount)
        {
            return;
        }
        _upgrade = _staff.Upgrade;
        _animator = transform.GetChild(_upgrade - 1).GetComponent<Animator>();
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        transform.GetChild(_upgrade - 1).gameObject.SetActive(true);
    }
}

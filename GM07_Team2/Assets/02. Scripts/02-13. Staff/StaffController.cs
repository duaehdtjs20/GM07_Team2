using System.Collections;

using GM07.Order;

using UnityEngine;
using UnityEngine.AI;

public class StaffController : MonoBehaviour
{
    [SerializeField]
    private GameFlowManager _flowManager;
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private NavMeshAgent _agent;
    [SerializeField]
    private GameObject _knife;
    [SerializeField]
    private Transform _cookPos;
    [SerializeField]
    private Transform _washPos;
    [SerializeField]
    private bool _switching = false;

    void Awake()
    {
        if (_flowManager == null)
        {
            _flowManager = FindFirstObjectByType<GameFlowManager>();
        }
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
        if (_agent == null)
        {
            _agent = GetComponent<NavMeshAgent>();
        }
    }
    private void Start()
    {
        StartCoroutine(StateStreamCo());
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
                    _knife.SetActive(true);
                    yield return StateCo("Cook", 8.0f);
                    _knife.SetActive(false);
                }
                else
                {
                    yield return MoveCo(_washPos);
                    yield return StateCo("Wash", 2.0f);
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
}

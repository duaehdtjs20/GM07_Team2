using UnityEngine;

public class CustomerAngry : CustomerStateBase
{
    private float _waitSecond = 3.0f;
    private float _timer = 0.0f;
    public CustomerAngry(Customer customer, Animator animator) : base(customer, animator, Animator.StringToHash("Angry")) { }

    public override void Enter()
    {
        base.Enter();

        _timer = 0.0f;
    }
    public override void Update()
    {
        if (_timer >= _waitSecond)
        {
            _customer.StateMachine.TransitionTo(_customer.StateMachine.ExitState);
            return;
        }
        _timer += Time.deltaTime;
    }
}

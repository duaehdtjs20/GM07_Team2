using UnityEngine;

public abstract class CustomerStateBase : ICharacterState
{
    protected Customer _customer;
    protected Animator _animator;
    protected int _hash;
    public CustomerStateBase(Customer customer, Animator animator, int hash)
    {
        _customer = customer;
        _animator = animator;
        _hash = hash;
    }
    public virtual void Enter()
    {
        _animator.SetBool(_hash, true);
    }
    public virtual void Exit()
    {
        _animator.SetBool(_hash, false);
    }
    public abstract void Update();
}

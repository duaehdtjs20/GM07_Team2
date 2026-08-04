using UnityEngine;

public abstract class CustomerStateBase : ICharacterState
{
    protected Customer _customer;
    public CustomerStateBase(Customer customer)
    {
        _customer = customer;
    }
    public virtual void Enter()
    {
    }

    public virtual void Exit()
    {
    }

    public virtual void Update()
    {
    }
}

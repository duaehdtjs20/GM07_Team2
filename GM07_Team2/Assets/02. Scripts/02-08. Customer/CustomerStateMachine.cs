using UnityEngine;

public class CustomerStateMachine : StateMachineBase
{
    public CustomerEnter EnterState { get; private set; }
    public CustomerSeat SeatState { get; private set; }
    public CustomerEat EatState { get; private set; }
    public CustomerExit ExitState { get; private set; }
    public CustomerAngry AngryState { get; private set; }

    public CustomerStateMachine(Customer customer, Animator animator)
    {
        EnterState = new CustomerEnter(customer, animator);
        SeatState = new CustomerSeat(customer, animator);
        EatState = new CustomerEat(customer, animator);
        ExitState = new CustomerExit(customer, animator);
        AngryState = new CustomerAngry(customer, animator);
    }
}

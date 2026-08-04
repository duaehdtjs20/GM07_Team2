public class CustomerStateMachine : StateMachineBase
{
    public CustomerEnter EnterState { get; private set; }
    public CustomerSeat SeatState { get; private set; }
    public CustomerEat EatState { get; private set; }
    public CustomerExit ExitState { get; private set; }

    public CustomerStateMachine(Customer customer)
    {
        EnterState = new CustomerEnter(customer);
        SeatState = new CustomerSeat(customer);
        EatState = new CustomerEat(customer);
        ExitState = new CustomerExit(customer);
    }
}

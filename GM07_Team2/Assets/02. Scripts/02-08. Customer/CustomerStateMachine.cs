public class CustomerStateMachine : StateMachineBase
{
    public CustomerEnter EnterState { get; private set; }
    public CustomerSeat SeatState { get; private set; }

    public CustomerStateMachine(Customer customer)
    {
        EnterState = new CustomerEnter(customer);
        SeatState = new CustomerSeat(customer);
    }
}

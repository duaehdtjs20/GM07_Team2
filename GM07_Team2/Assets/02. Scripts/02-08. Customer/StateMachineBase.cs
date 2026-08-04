public abstract class StateMachineBase
{
    private ICharacterState _currentState;
    public void Initialize(ICharacterState startState)
    {
        _currentState = startState;
        _currentState.Enter();
    }
    public void UpdateState()
    {
        _currentState.Update();
    }
    public void TransitionTo(ICharacterState nextState)
    {
        if(_currentState == nextState)
        {
            return;
        }
        _currentState.Exit();
        _currentState = nextState;
        _currentState.Enter();
    }
}

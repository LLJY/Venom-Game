
public abstract class State<T>
    where T: StatefulMonoBehaviour<T>
{
    protected T _behaviour;
    
    public State(T behaviour)
    {
        _behaviour = behaviour;
    }
    public abstract void Prepare();
    public abstract void CleanUp();
    public abstract void Update();
    public abstract void LateUpdate();
    public abstract void FixedUpdate();

    protected virtual void SetState(State<T> state)
    {
        _behaviour.CurrentState = state;
    }
}

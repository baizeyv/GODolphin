using System;

namespace GODolphin.StateMachine;

public interface IState
{
    bool Condition();
    void Enter();
    void Process();
    void PhysicsProcess();
    void Exit();
}

public class SimpleState : IState
{
    private Func<bool> _onCondition;
    private System.Action _onEnter;
    private System.Action _onProcess;
    private System.Action _onPhysicsProcess;
    private System.Action _onDraw;
    private System.Action _onExit;

    public SimpleState OnCondition(Func<bool> condition)
    {
        _onCondition = condition;
        return this;
    }

    public SimpleState OnEnter(System.Action onEnter)
    {
        _onEnter = onEnter;
        return this;
    }

    public SimpleState OnProcess(System.Action onProcess)
    {
        _onProcess = onProcess;
        return this;
    }

    public SimpleState OnPhysicsProcess(System.Action onPhysicsProcess)
    {
        _onPhysicsProcess = onPhysicsProcess;
        return this;
    }

    public SimpleState OnExit(System.Action onExit)
    {
        _onExit = onExit;
        return this;
    }

    public bool Condition()
    {
        var result = _onCondition?.Invoke();
        return result == null || result.Value;
    }

    public void Enter()
    {
        _onEnter?.Invoke();
    }

    public void Process()
    {
        _onProcess?.Invoke();
    }

    public void PhysicsProcess()
    {
        _onPhysicsProcess?.Invoke();
    }

    public void Exit()
    {
        _onExit?.Invoke();
    }
}

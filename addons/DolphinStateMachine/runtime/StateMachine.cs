using System;
using System.Collections.Generic;
using Godot;

namespace GODolphin.StateMachine;

public partial class StateMachine<T> : GodotObject where T : Enum
{
    private Dictionary<T, IState> _states = new();

    private IState _currentState;

    private T _currentStateId;

    public IState CurrentState => _currentState;

    public T CurrentStateId => _currentStateId;

    public T PreviousStateId { get; private set; }

    public long FrameCountOfCurrentState = 1;

    public float SecondsOfCurrentState = 0f;

    private event Action<T, T> _onStateChanged = (_, __) => { };

    public string MachineName { get; private set; }

    internal StateMachine(string machineName)
    {
        MachineName = machineName;
        StateMachineSharedBuffer.Instance.SendStateMachine(this, true, false);
    }

    /// <summary>
    /// * 方式1
    /// </summary>
    /// <param name="id"></param>
    /// <param name="state"></param>
    public void AddState(T id, IState state)
    {
        _states.Add(id, state);
    }

    /// <summary>
    /// * 方式2
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public SimpleState State(T t)
    {
        if (_states.ContainsKey(t))
        {
            return _states[t] as SimpleState;
        }

        var state = new SimpleState();
        _states.Add(t, state);
        return state;
    }

    public void SwitchState(T t)
    {
        if (t.Equals(CurrentStateId))
            return;

        if (_states.TryGetValue(t, out var state))
        {
            if (CurrentState != null && state.Condition())
            {
                _currentState.Exit();
                PreviousStateId = _currentStateId;
                _currentState = state;
                _currentStateId = t;
                _onStateChanged?.Invoke(PreviousStateId, CurrentStateId);
                FrameCountOfCurrentState = 1;
                SecondsOfCurrentState = 0f;
                _currentState.Enter();
                StateMachineSharedBuffer.Instance.SendStateMachine(this, false, false);
            }
        }
    }

    public void OnStateChanged(Action<T, T> onStateChanged)
    {
        _onStateChanged += onStateChanged;
    }

    public void StartState(T t)
    {
        if (_states.TryGetValue(t, out var state))
        {
            PreviousStateId = t;
            _currentState = state;
            _currentStateId = t;
            FrameCountOfCurrentState = 0;
            SecondsOfCurrentState = 0f;
            state.Enter();
            StateMachineSharedBuffer.Instance.SendStateMachine(this, false, false);
        }
    }

    public void _PhysicsProcess(double delta)
    {
        _currentState?.PhysicsProcess();
    }

    public void _Process(double delta)
    {
        _currentState?.Process();
        FrameCountOfCurrentState++;
        SecondsOfCurrentState += (float)delta;
    }

    public void Clear()
    {
        _currentState = null;
        _currentStateId = default;
        _states.Clear();
    }
}

public abstract class AbstractState<TStateId, TTarget> : IState where TStateId : Enum
{
    protected StateMachine<TStateId> machine;

    protected TTarget target;

    public AbstractState(StateMachine<TStateId> machine, TTarget target)
    {
        this.machine = machine;
        this.target = target;
    }

    public bool Condition()
    {
        return OnCondition();
    }

    public void Enter()
    {
        OnEnter();
    }

    public void Process()
    {
        OnProcess();
    }

    public void PhysicsProcess()
    {
        OnPhysicsProcess();
    }

    public void Exit()
    {
        OnExit();
    }

    protected virtual bool OnCondition() => true;

    protected virtual void OnEnter()
    {
    }

    protected virtual void OnProcess()
    {
    }

    protected virtual void OnPhysicsProcess()
    {
    }

    protected virtual void OnExit()
    {
    }
}
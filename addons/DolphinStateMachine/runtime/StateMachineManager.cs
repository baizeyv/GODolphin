using System;
using System.Collections.Generic;
using GODolphin.Singleton;

namespace GODolphin.StateMachine;

[NodePath("[StateMachine-Manager]")]
public partial class StateMachineManager : NodeSingleton<StateMachineManager>
{
    private HashSet<string> _stateMachineSet = new();

    private StateMachineManager(){}

    public StateMachine<T> CreateMachine<T>(string machineName) where T : Enum
    {
        if (_stateMachineSet.Contains(machineName))
        {
            throw new ArgumentException($"Machine '{machineName}' already exists.");
        }
        _stateMachineSet.Add(machineName);
        return new StateMachine<T>(machineName);
    }

    public void Free<T>(StateMachine<T> machine) where T : Enum
    {
        StateMachineSharedBuffer.Instance.SendStateMachine(machine, false, true);
        Free(machine.MachineName);
    }

    private void Free(string machineName)
    {
        if (_stateMachineSet.Contains(machineName))
        {
            _stateMachineSet.Remove(machineName);
        }
    }
}
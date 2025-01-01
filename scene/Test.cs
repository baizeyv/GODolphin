using Godot;
using GODolphin.Log;
using GODolphin.StateMachine;

public partial class Test : Control
{
    [Export]
    public Tree tree;

    public override void _Ready()
    {
        // var a = 222;
        // Log.Debug().Var("a value", a).Sep().Msg("hello").Tag("MYMANAGER").Do();
        // Log.Warn().Msg("test content hello world").Cr().Msg("????").Do();
        // Log.Error().Msg("test content hello world").Cr().Msg("????").Do();
        // Log.Info().Msg("test content hello world").Cr().Msg("????").Do();
        // Log.Debug().Msg("GOGOGO").Do();
        // for (var i = 0; i < 10; i++)
        // {
        //     Log.Error().Var("i value", i).Do();
        // }
        //////////////////////////////////////
        // var root = tree.CreateItem();
        // root.SetText(0, "ROOT");
        // var child1 = tree.CreateItem(root);
        // child1.SetText(0, "child1");
        // var child2 = tree.CreateItem(root);
        // child2.SetText(0, "child2");
        // var child3 = tree.CreateItem(root);
        // child3.SetText(0, "child3");
        // var child4 = tree.CreateItem(root);
        // child4.SetText(0, "child4");
        // var child5 = tree.CreateItem(root);
        // child5.SetText(0, "child5");

        var machine = StateMachineManager.Instance.CreateMachine<LogType>("LogTypeMachine");
        machine.State(LogType.Debug).OnEnter(() =>
        {
            Log.Debug().Msg("ENTER").Do();
        });
        machine.State(LogType.Error).OnEnter(() =>
        {

        });
        machine.StartState(LogType.Debug);
        machine.SwitchState(LogType.Error);
        var machine2 = StateMachineManager.Instance.CreateMachine<LogType>("LogTypeMachine2");
        machine2.State(LogType.Debug).OnEnter(() =>
        {
            Log.Debug().Msg("ENTER").Do();
        });
        machine2.State(LogType.Error).OnEnter(() =>
        {

        });
        machine2.StartState(LogType.Error);
        machine2.SwitchState(LogType.Debug);
    }
}
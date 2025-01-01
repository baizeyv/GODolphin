using Godot;
using GODolphin.Log;

public partial class Test : Control
{
    public override void _Ready()
    {
        var a = 222;
        Log.Debug().Var("a value", a).Sep().Msg("hello").Tag("MYMANAGER").Do();
        Log.Warn().Msg("test content hello world").Cr().Msg("????").Do();
        Log.Error().Msg("test content hello world").Cr().Msg("????").Do();
        Log.Info().Msg("test content hello world").Cr().Msg("????").Do();
        Log.Debug().Msg("GOGOGO").Do();
        for (var i = 0; i < 10; i++)
        {
            Log.Error().Var("i value", i).Do();
        }
    }
}
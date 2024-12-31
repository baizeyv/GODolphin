using Godot;

namespace GODolphin.AOP;

public class LogInvocationHandler : AbstractInvocationHandler
{
    public static LogInvocationHandler Instance = new LogInvocationHandler();

    private LogInvocationHandler() { }

    public override void Preprocess()
    {
        GD.Print("PRE");
    }

    public override void PostProcess()
    {
        GD.Print("POST");
    }
}

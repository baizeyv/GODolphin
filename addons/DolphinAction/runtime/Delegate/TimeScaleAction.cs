using GODolphin.Pool;
using Godot;

namespace GODolphin.Action
{
    public class TimeScaleAction : DelegateAction
    {
        private double _scale;

        protected override bool Delegate(double delta)
        {
            if (ActionExe == null)
            {
                return true;
            }

            return ActionExe.Act(delta * _scale);
        }

        public void SetScale(double scale)
        {
            _scale = scale;
        }
    }

    public static class TimeScaleActionExtensions
    {
        public static TimeScaleAction TimeScale(this Node _, double scale, ActionExecutor action)
        {
            var myAction = SafeObjectPool<TimeScaleAction>.Instance.Obtain();
            myAction.SetScale(scale);
            myAction.ActionExe = action;
            return myAction;
        }
    }
}

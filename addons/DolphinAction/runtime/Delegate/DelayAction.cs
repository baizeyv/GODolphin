using GODolphin.Pool;
using Godot;

namespace GODolphin.Action
{
    public class DelayAction : DelegateAction
    {
        private double _duration,
            _time;

        protected override bool Delegate(double delta)
        {
            if (_time < _duration)
            {
                _time += delta;
                if (_time < _duration)
                {
                    return false;
                }

                delta = _time - _duration;
            }

            if (ActionExe == null)
            {
                return true;
            }

            return ActionExe.Act(delta);
        }

        public void Finish()
        {
            _time = _duration;
        }

        public override void Restart()
        {
            base.Restart();
            _time = 0;
        }

        public void SetDuration(double duration)
        {
            _duration = duration;
        }
    }

    public static class DelayActionExtensions
    {
        public static DelayAction Delay(this Node self, double duration, ActionExecutor action)
        {
            var myAction = SafeObjectPool<DelayAction>.Instance.Obtain();
            myAction.SetDuration(duration);
            myAction.ActionExe = action;
            return myAction;
        }
    }
}

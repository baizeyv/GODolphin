using GODolphin.Pool;
using Godot;

namespace GODolphin.Action
{
    public class DelayFrameAction : DelegateAction
    {
        private int _duration,
            _time;

        protected override bool Delegate(double delta)
        {
            if (_time < _duration)
            {
                _time++;
                if (_time < _duration)
                    return false;
            }

            if (ActionExe == null)
                return true;

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

        public void SetDuration(int duration)
        {
            _duration = duration;
        }
    }

    public static class DelayFrameActionExtensions
    {
        public static DelayFrameAction DelayFrame(
            this Node self,
            int duration,
            ActionExecutor action
        )
        {
            var myAction = SafeObjectPool<DelayFrameAction>.Instance.Obtain();
            myAction.SetDuration(duration);
            myAction.ActionExe = action;
            return myAction;
        }
    }
}

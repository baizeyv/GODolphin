using GODolphin.Pool;
using Godot;

namespace GODolphin.Action
{
    public class RepeatAction : DelegateAction
    {
        public static readonly int FOREVER = -1;

        private int _repeatCount,
            _executedCount;

        private bool _finished;

        protected override bool Delegate(double delta)
        {
            if (_executedCount == _repeatCount)
                return true;
            if (ActionExe.Act(delta))
            {
                if (_finished)
                    return true;
                if (_repeatCount > 0)
                    _executedCount++;
                if (_executedCount == _repeatCount)
                    return true;
                ActionExe?.Restart();
            }

            return false;
        }

        /// <summary>
        /// Causes the action to not repeat again
        /// </summary>
        public void Finish()
        {
            _finished = true;
        }

        public override void Restart()
        {
            base.Restart();
            _executedCount = 0;
            _finished = false;
        }

        public void SetCount(int count)
        {
            _repeatCount = count;
        }
    }

    public static class RepeatActionExtensions
    {
        public static RepeatAction Repeat(this Node _, int count, ActionExecutor action)
        {
            var myAction = SafeObjectPool<RepeatAction>.Instance.Obtain();
            myAction.SetCount(count);
            myAction.ActionExe = action;
            return myAction;
        }
    }
}

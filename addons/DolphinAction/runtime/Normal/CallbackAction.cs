using System;
using GODolphin.Pool;
using Godot;

namespace GODolphin.Action
{
    public class CallbackAction : ActionExecutor
    {
        private System.Action _action;

        private bool _ran;

        public override bool Act(double delta)
        {
            if (!_ran)
            {
                _ran = true;
                Run();
            }

            return true;
        }

        public void Run()
        {
            var pool = GetPool();
            SetPool(null);
            try
            {
                _action?.Invoke();
            }
            finally
            {
                SetPool(pool);
            }
        }

        public override void Restart()
        {
            _ran = false;
        }

        public override void Reset()
        {
            base.Reset();
            _action = null;
        }

        public void SetAction(System.Action action)
        {
            _action = action;
        }
    }

    public static class CallbackActionExtensions
    {
        public static CallbackAction Callback(this Node self, System.Action action)
        {
            var myAction = SafeObjectPool<CallbackAction>.Instance.Obtain();
            myAction.SetAction(action);
            return myAction;
        }
    }
}

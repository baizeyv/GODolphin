using System.Collections.Generic;
using GODolphin.Pool;
using Godot;

namespace GODolphin.Action
{
    public class ParallelAction : ActionExecutor
    {
        protected List<ActionExecutor> _actions = new();

        private bool _complete;

        public override bool Act(double delta)
        {
            if (_complete)
                return true;
            _complete = true;
            var pool = GetPool();
            SetPool(null);
            try
            {
                var actions = _actions;
                for (int i = 0, n = actions.Count; i < n; i++)
                {
                    var currentAction = actions[i];
                    if (!currentAction.Act(delta))
                        _complete = false;
                }

                return _complete;
            }
            finally
            {
                SetPool(pool);
            }
        }

        public override void Restart()
        {
            _complete = false;
            var actions = _actions;
            for (int i = 0, n = actions.Count; i < n; i++)
                actions[i].Restart();
        }

        public override void Reset()
        {
            base.Reset();
            _actions.Clear();
        }

        public void AddAction(ActionExecutor action)
        {
            _actions.Add(action);
        }
    }

    public static class ParallelActionExtensions
    {
        public static ParallelAction Parallel(this Node self, params ActionExecutor[] actions)
        {
            var action = SafeObjectPool<ParallelAction>.Instance.Obtain();
            foreach (var t in actions)
            {
                action.AddAction(t);
            }

            return action;
        }
    }
}

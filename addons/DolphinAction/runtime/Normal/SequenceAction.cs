using GODolphin.Pool;
using Godot;

namespace GODolphin.Action
{
    public class SequenceAction : ParallelAction
    {
        private int _index;

        public override bool Act(double delta)
        {
            if (_index >= _actions.Count)
            {
                return true;
            }

            var pool = GetPool();
            SetPool(null);
            try
            {
                if (_actions[_index].Act(delta))
                {
                    _index++;
                    if (_index >= _actions.Count)
                        return true;
                }

                return false;
            }
            finally
            {
                SetPool(pool);
            }
        }

        public override void Restart()
        {
            base.Restart();
            _index = 0;
        }
    }

    public static class SequenceActionExtensions
    {
        public static SequenceAction Sequence(this Node self, params ActionExecutor[] actions)
        {
            var action = SafeObjectPool<SequenceAction>.Instance.Obtain();
            foreach (var t in actions)
            {
                action.AddAction(t);
            }
            return action;
        }
    }
}

using Godot;

namespace GODolphin.Action
{
    public abstract class DelegateAction : ActionExecutor
    {
        public ActionExecutor ActionExe { get; set; }

        protected abstract bool Delegate(double delta);

        public sealed override bool Act(double delta)
        {
            var pool = GetPool();
            SetPool(null);
            try
            {
                return Delegate(delta);
            }
            finally
            {
                SetPool(pool);
            }
        }

        public override void Restart()
        {
            ActionExe?.Restart();
        }

        public override void Reset()
        {
            base.Reset();
            ActionExe = null;
        }
    }
}

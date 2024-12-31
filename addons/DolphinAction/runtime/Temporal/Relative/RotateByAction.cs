using GODolphin.Pool;
using Godot;

namespace GODolphin.Action
{
    public class RotateByAction : RelativeTemporalAction
    {
        private float _amount;

        private Node2D _node;

        private Control _control;

        private bool _isLocal;

        protected override void UpdateRelative(double percentDelta)
        {
            if (_control != null)
            {
                _control.RotateBy(_amount * (float)percentDelta);
            }
            else
            {
                if (_isLocal)
                {
                    _node.RotateBy(_amount * (float)percentDelta);
                }
                else
                {
                    _node.RotateByGlobal(_amount * (float)percentDelta);
                }
            }
        }

        public RotateByAction SetLocal()
        {
            _isLocal = true;
            return this;
        }

        public override void Reset()
        {
            base.Reset();
            _node = null;
            _control = null;
        }

        public void SetAmount(Node2D node, float amount)
        {
            _node = node;
            _amount = amount;
        }

        public void SetAmount(Control control, float amount)
        {
            _control = control;
            _amount = amount;
        }
    }

    public static class RotateByActionExtensions
    {
        public static RotateByAction RotateBy(this Node2D node, float amount, double duration)
        {
            var action = SafeObjectPool<RotateByAction>.Instance.Obtain();
            action.SetDuration(duration);
            action.SetAmount(node, amount);
            return action;
        }

        public static RotateByAction RotateBy(this Control node, float amount, double duration)
        {
            var action = SafeObjectPool<RotateByAction>.Instance.Obtain();
            action.SetDuration(duration);
            action.SetAmount(node, amount);
            return action;
        }
    }
}

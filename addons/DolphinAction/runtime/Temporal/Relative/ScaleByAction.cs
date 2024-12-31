using GODolphin.Pool;
using Godot;

namespace GODolphin.Action
{
    public class ScaleByAction : RelativeTemporalAction
    {
        private float _amountX,
            _amountY;

        private Node2D _node;

        private Control _control;

        private bool _isLocal;

        protected override void UpdateRelative(double percentDelta)
        {
            if (_control != null)
            {
                _control.ScaleBy(_amountX * (float)percentDelta, _amountY * (float)percentDelta);
            }
            else
            {
                if (_isLocal)
                {
                    _node.ScaleBy(_amountX * (float)percentDelta, _amountY * (float)percentDelta);
                }
                else
                {
                    _node.ScaleByGlobal(
                        _amountX * (float)percentDelta,
                        _amountY * (float)percentDelta
                    );
                }
            }
        }

        public ScaleByAction SetLocal()
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

        public void SetAmount(Node2D node, float amountX, float amountY)
        {
            _amountX = amountX;
            _amountY = amountY;
            _node = node;
        }

        public void SetAmount(Control control, float amountX, float amountY)
        {
            _amountX = amountX;
            _amountY = amountY;
            _control = control;
        }
    }

    public static class ScaleByActionExtensions
    {
        public static ScaleByAction ScaleBy(
            this Node2D node,
            float amountX,
            float amountY,
            double duration
        )
        {
            var action = SafeObjectPool<ScaleByAction>.Instance.Obtain();
            action.SetDuration(duration);
            action.SetAmount(node, amountX, amountY);
            return action;
        }

        public static ScaleByAction ScaleBy(
            this Control node,
            float amountX,
            float amountY,
            double duration
        )
        {
            var action = SafeObjectPool<ScaleByAction>.Instance.Obtain();
            action.SetDuration(duration);
            action.SetAmount(node, amountX, amountY);
            return action;
        }
    }
}

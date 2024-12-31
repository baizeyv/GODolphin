using GODolphin.Pool;
using Godot;

namespace GODolphin.Action
{
    public class MoveByAction : RelativeTemporalAction
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
                if (_isLocal)
                {
                    _control.MoveBy(_amountX * (float)percentDelta, _amountY * (float)percentDelta);
                }
                else
                {
                    _control.MoveByGlobal(
                        _amountX * (float)percentDelta,
                        _amountY * (float)percentDelta
                    );
                }
            }
            else
            {
                if (_isLocal)
                {
                    _node.MoveBy(_amountX * (float)percentDelta, _amountY * (float)percentDelta);
                }
                else
                {
                    _node.MoveByGlobal(
                        _amountX * (float)percentDelta,
                        _amountY * (float)percentDelta
                    );
                }
            }
        }

        public void SetAmount(Node2D node, float x, float y)
        {
            _amountX = x;
            _amountY = y;
            _node = node;
        }

        public void SetAmount(Control control, float x, float y)
        {
            _amountX = x;
            _amountY = y;
            _control = control;
        }

        public override void Reset()
        {
            base.Reset();
            _node = null;
            _control = null;
        }

        public MoveByAction SetLocal()
        {
            _isLocal = true;
            return this;
        }
    }

    public static class MoveByActionExtensions
    {
        public static MoveByAction MoveBy(this Node2D self, float x, float y, double duration)
        {
            var action = SafeObjectPool<MoveByAction>.Instance.Obtain();
            action.SetDuration(duration);
            action.SetAmount(self, x, y);
            return action;
        }

        public static MoveByAction MoveBy(this Control self, float x, float y, double duration)
        {
            var action = SafeObjectPool<MoveByAction>.Instance.Obtain();
            action.SetDuration(duration);
            action.SetAmount(self, x, y);
            return action;
        }
    }
}

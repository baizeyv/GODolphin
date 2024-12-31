using GODolphin.Pool;
using Godot;

namespace GODolphin.Action
{
    public class RotateToAction : TemporalAction
    {
        private float _start,
            _end;

        private bool _useShortestDirection;

        private bool _isLocal;

        private Node2D _node;

        private Control _control;

        protected override void Begin()
        {
            if (_control != null)
            {
                _start = _control.RotationDegrees;
            }
            else
            {
                _start = _isLocal ? _node.RotationDegrees : _node.GlobalRotationDegrees;
            }
        }

        protected override void Update(double percent)
        {
            if (_useShortestDirection)
            {
                var angle = Mathf.LerpAngle(
                    Mathf.DegToRad(_start),
                    Mathf.DegToRad(_end),
                    (float)percent
                );
                angle = Mathf.RadToDeg(angle);
                if (_control != null)
                {
                    _control.SetRotationDegrees(angle);
                }
                else
                {
                    if (_isLocal)
                    {
                        _node.SetRotationDegrees(angle);
                    }
                    else
                    {
                        _node.SetGlobalRotationDegrees(angle);
                    }
                }
            }
            else
            {
                var angle = _start + (_end - _start) * (float)percent;
                if (_control != null)
                {
                    _control.SetRotationDegrees(angle);
                }
                else
                {
                    if (_isLocal)
                    {
                        _node.SetRotationDegrees(angle);
                    }
                    else
                    {
                        _node.SetGlobalRotationDegrees(angle);
                    }
                }
            }
        }

        public RotateToAction SetUseShortestDirection()
        {
            _useShortestDirection = true;
            return this;
        }

        public RotateToAction SetLocal()
        {
            _isLocal = true;
            return this;
        }

        public void SetAngle(Node2D node, float angle)
        {
            _end = angle;
            _node = node;
        }

        public void SetAngle(Control control, float angle)
        {
            _end = angle;
            _control = control;
        }

        public override void Reset()
        {
            base.Reset();
            _node = null;
            _control = null;
        }
    }

    public static class RotateToActionExtensions
    {
        public static RotateToAction RotateTo(this Node2D node, float degrees, double duration)
        {
            var action = SafeObjectPool<RotateToAction>.Instance.Obtain();
            action.SetDuration(duration);
            action.SetAngle(node, degrees);
            return action;
        }

        public static RotateToAction RotateTo(this Control node, float degrees, double duration)
        {
            var action = SafeObjectPool<RotateToAction>.Instance.Obtain();
            action.SetDuration(duration);
            action.SetAngle(node, degrees);
            return action;
        }
    }
}

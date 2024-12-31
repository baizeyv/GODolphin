using GODolphin.Pool;
using Godot;

namespace GODolphin.Action
{
    public class MoveToAction : TemporalAction
    {
        private float _startX,
            _startY;

        private float _endX,
            _endY;

        private bool _isLocal;

        private Node2D _node;

        private Control _control;

        protected override void Begin()
        {
            base.Begin();
            if (_control != null)
            {
                if (_isLocal)
                {
                    _startX = _control.Position.X;
                    _startY = _control.Position.Y;
                    _node.SetPosition(_startX, _startY);
                }
                else
                {
                    _startX = _control.GlobalPosition.X;
                    _startY = _control.GlobalPosition.Y;
                    _node.SetGlobalPosition(_startX, _startY);
                }
            }
            else
            {
                if (_isLocal)
                {
                    _startX = _node.Position.X;
                    _startY = _node.Position.Y;
                    _node.SetPosition(_startX, _startY);
                }
                else
                {
                    _startX = _node.GlobalPosition.X;
                    _startY = _node.GlobalPosition.Y;
                    _node.SetGlobalPosition(_startX, _startY);
                }
            }
        }

        protected override void Update(double percent)
        {
            if (_control != null)
            {
                if (_isLocal)
                {
                    _control.SetPosition(
                        _startX + (_endX - _startX) * (float)percent,
                        _startY + (_endY - _startY) * (float)percent
                    );
                }
                else
                {
                    _control.SetGlobalPosition(
                        _startX + (_endX - _startX) * (float)percent,
                        _startY + (_endY - _startY) * (float)percent
                    );
                }
            }
            else
            {
                if (_isLocal)
                {
                    _node.SetPosition(
                        _startX + (_endX - _startX) * (float)percent,
                        _startY + (_endY - _startY) * (float)percent
                    );
                }
                else
                {
                    _node.SetGlobalPosition(
                        _startX + (_endX - _startX) * (float)percent,
                        _startY + (_endY - _startY) * (float)percent
                    );
                }
            }
        }

        public override void Reset()
        {
            base.Reset();
            _node = null;
            _control = null;
        }

        public void SetPosition(Node2D node, float x, float y)
        {
            _node = node;
            _endX = x;
            _endY = y;
        }

        public void SetPosition(Control control, float x, float y)
        {
            _control = control;
            _endX = x;
            _endY = y;
        }

        public MoveToAction SetLocal<T>()
        {
            _isLocal = true;
            return this;
        }
    }

    public static class MoveToActionExtensions
    {
        public static MoveToAction MoveTo(this Node2D self, float x, float y, double duration)
        {
            var action = SafeObjectPool<MoveToAction>.Instance.Obtain();
            action.SetDuration(duration);
            action.SetPosition(self, x, y);
            return action;
        }

        public static MoveToAction MoveTo(this Control self, float x, float y, double duration)
        {
            var action = SafeObjectPool<MoveToAction>.Instance.Obtain();
            action.SetDuration(duration);
            action.SetPosition(self, x, y);
            return action;
        }
    }
}

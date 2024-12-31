using GODolphin.Pool;
using Godot;

namespace GODolphin.Action
{
    public class ScaleToAction : TemporalAction
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
            if (_control != null)
            {
                _startX = _control.Scale.X;
                _startY = _control.Scale.Y;
            }
            else
            {
                if (_isLocal)
                {
                    _startX = _node.Scale.X;
                    _startY = _node.Scale.Y;
                }
                else
                {
                    _startX = _node.GlobalScale.X;
                    _startY = _node.GlobalScale.Y;
                }
            }
        }

        protected override void Update(double percent)
        {
            if (_control != null)
            {
                _control.SetScale(
                    _startX + (_endX - _startX) * (float)percent,
                    _startY + (_endY - _startY) * (float)percent
                );
            }
            else
            {
                if (_isLocal)
                {
                    _node.SetScale(
                        _startX + (_endX - _startX) * (float)percent,
                        _startY + (_endY - _startY) * (float)percent
                    );
                }
                else
                {
                    _node.SetGlobalScale(
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

        public ScaleToAction SetLocal()
        {
            _isLocal = true;
            return this;
        }

        public void SetScale(Node2D node, float x, float y)
        {
            _node = node;
            _endX = x;
            _endY = y;
        }

        public void SetScale(Control control, float x, float y)
        {
            _control = control;
            _endX = x;
            _endY = y;
        }
    }

    public static class ScaleToActionExtensions
    {
        public static ScaleToAction ScaleTo(this Node2D self, float x, float y, double duration)
        {
            var action = SafeObjectPool<ScaleToAction>.Instance.Obtain();
            action.SetDuration(duration);
            action.SetScale(self, x, y);
            return action;
        }

        public static ScaleToAction ScaleTo(this Control self, float x, float y, double duration)
        {
            var action = SafeObjectPool<ScaleToAction>.Instance.Obtain();
            action.SetDuration(duration);
            action.SetScale(self, x, y);
            return action;
        }
    }
}

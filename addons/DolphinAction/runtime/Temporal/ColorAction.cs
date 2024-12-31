using GODolphin.Pool;
using Godot;

namespace GODolphin.Action
{
    public class ColorAction : TemporalAction
    {
        private float _startR,
            _startG,
            _startB,
            _startA;

        private CanvasItem _canvasItem;

        private Color _end;

        private bool _isSelf;

        protected override void Begin()
        {
            Color clr;
            clr = _isSelf ? _canvasItem.SelfModulate : _canvasItem.Modulate;
            _startR = clr.R;
            _startG = clr.G;
            _startB = clr.B;
            _startA = clr.A;
        }

        protected override void Update(double percent)
        {
            var r = _startR + (_end.R - _startR) * (float)percent;
            var g = _startG + (_end.G - _startG) * (float)percent;
            var b = _startB + (_end.B - _startB) * (float)percent;
            var a = _startA + (_end.A - _startA) * (float)percent;
            if (_isSelf)
            {
                _canvasItem.SelfModulate = new Color(r, g, b, a);
            }
            else
            {
                _canvasItem.Modulate = new Color(r, g, b, a);
            }
        }

        public void SetColor(CanvasItem canvasItem, Color color)
        {
            _canvasItem = canvasItem;
            _end = color;
        }

        public ColorAction SetSelf()
        {
            _isSelf = true;
            return this;
        }

        public override void Reset()
        {
            base.Reset();
            _canvasItem = null;
        }
    }

    public static class ColorActionExtensions
    {
        public static ColorAction Color(this CanvasItem canvasItem, Color color, double duration)
        {
            var action = SafeObjectPool<ColorAction>.Instance.Obtain();
            action.SetDuration(duration);
            action.SetColor(canvasItem, color);
            return action;
        }
    }
}

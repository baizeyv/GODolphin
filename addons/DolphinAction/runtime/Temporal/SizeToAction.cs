using GODolphin.Pool;
using Godot;

namespace GODolphin.Action
{
    public class SizeToAction : TemporalAction
    {
        private float _startWidth,
            _startHeight;

        private float _endWidth,
            _endHeight;

        private Control _control;

        protected override void Begin()
        {
            _startWidth = _control.Size.X;
            _startHeight = _control.Size.Y;
        }

        protected override void Update(double percent)
        {
            _control.SetSize(
                _startWidth + (_endWidth - _startWidth) * (float)percent,
                _startHeight + (_endHeight - _startHeight) * (float)percent
            );
        }

        public void SetSize(Control control, float width, float height)
        {
            _control = control;
            _endWidth = width;
            _endHeight = height;
        }

        public override void Reset()
        {
            base.Reset();
            _control = null;
        }
    }

    public static class SizeToActionExtensions
    {
        public static SizeToAction SizeTo(
            this Control control,
            float width,
            float height,
            double duration
        )
        {
            var action = SafeObjectPool<SizeToAction>.Instance.Obtain();
            action.SetDuration(duration);
            action.SetSize(control, width, height);
            return action;
        }
    }
}

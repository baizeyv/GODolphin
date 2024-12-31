using GODolphin.Pool;
using Godot;

namespace GODolphin.Action
{
    public class SizeByAction : RelativeTemporalAction
    {
        private float _amountWidth,
            _amountHeight;

        private Control _control;

        protected override void UpdateRelative(double percentDelta)
        {
            _control.SizeBy(
                _amountWidth * (float)percentDelta,
                _amountHeight * (float)percentDelta
            );
        }

        public void SetAmount(Control control, float amountWidth, float amountHeight)
        {
            _control = control;
            _amountWidth = amountWidth;
            _amountHeight = amountHeight;
        }

        public override void Reset()
        {
            base.Reset();
            _control = null;
        }
    }

    public static class SizeByActionExtensions
    {
        public static SizeByAction SizeBy(
            this Control control,
            float amountWidth,
            float amountHeight,
            double duration
        )
        {
            var action = SafeObjectPool<SizeByAction>.Instance.Obtain();
            action.SetDuration(duration);
            action.SetAmount(control, amountWidth, amountHeight);
            return action;
        }
    }
}

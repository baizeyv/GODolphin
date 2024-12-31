using GODolphin.Pool;
using Godot;

namespace GODolphin.Action
{
    public class AlphaAction : TemporalAction
    {
        private float _start,
            _end;

        private CanvasItem _canvasItem;

        private bool _isSelf;

        protected override void Begin()
        {
            _start = _isSelf ? _canvasItem.SelfModulate.A : _canvasItem.Modulate.A;
        }

        protected override void Update(double percent)
        {
            var value = _start + (_end - _start) * (float)percent;
            if (_isSelf)
            {
                _canvasItem.SetSelfAlpha(value);
            }
            else
            {
                _canvasItem.SetGroupAlpha(value);
            }
        }

        public override void Reset()
        {
            base.Reset();
            _canvasItem = null;
            _isSelf = false;
        }

        public void SetAlpha(CanvasItem canvasItem, float alpha)
        {
            _canvasItem = canvasItem;
            _end = alpha;
        }

        public AlphaAction SetSelf()
        {
            _isSelf = true;
            return this;
        }
    }

    public static class AlphaActionExtensions
    {
        public static AlphaAction Alpha(this CanvasItem self, float alpha, double duration)
        {
            var action = SafeObjectPool<AlphaAction>.Instance.Obtain();
            action.SetDuration(duration);
            action.SetAlpha(self, alpha);
            return action;
        }
    }
}

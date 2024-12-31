using System;
using GODolphin.Pool;
using Godot;

namespace GODolphin.Action
{
    public class FloatAction : TemporalAction
    {
        private float _start,
            _end;

        private float _value;

        private Action<float> _onCompleted;

        private Action<float> _onProgress;

        protected override void Begin()
        {
            _value = _start;
            _onProgress?.Invoke(_value);
        }

        protected override void Update(double percent)
        {
            _value = _start + (_end - _start) * (float)percent;
            _onProgress?.Invoke(_value);
        }

        protected override void End()
        {
            _onCompleted?.Invoke(_value);
        }

        public void SetValue(float start, float end)
        {
            _start = start;
            _end = end;
        }

        public FloatAction OnCompleted(Action<float> action)
        {
            _onCompleted = action;
            return this;
        }

        public FloatAction OnProgress(Action<float> action)
        {
            _onProgress = action;
            return this;
        }

        public override void Reset()
        {
            base.Reset();
            _onCompleted = null;
            _onProgress = null;
        }
    }

    public static class FloatActionExtensions
    {
        public static FloatAction Float(this Node _, float start, float end, double duration)
        {
            var action = SafeObjectPool<FloatAction>.Instance.Obtain();
            action.SetDuration(duration);
            action.SetValue(start, end);
            return action;
        }
    }
}

using System;
using GODolphin.Pool;
using Godot;

namespace GODolphin.Action
{
    public class IntAction : TemporalAction
    {
        private int _start,
            _end;

        private int _value;

        private Action<int> _onCompleted;

        private Action<int> _onProgress;

        protected override void Begin()
        {
            _value = _start;
            _onProgress?.Invoke(_value);
        }

        protected override void Update(double percent)
        {
            _value = (int)(_start + (_end - _start) * percent);
            _onProgress?.Invoke(_value);
        }

        protected override void End()
        {
            _onProgress?.Invoke(_value);
        }

        public void SetValue(int start, int end)
        {
            _start = start;
            _end = end;
        }

        public IntAction OnCompleted(Action<int> action)
        {
            _onCompleted = action;
            return this;
        }

        public IntAction OnProgress(Action<int> action)
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

    public static class IntActionExtensions
    {
        public static IntAction Int(this Node _, int start, int end, double duration)
        {
            var action = SafeObjectPool<IntAction>.Instance.Obtain();
            action.SetDuration(duration);
            action.SetValue(start, end);
            return action;
        }
    }
}

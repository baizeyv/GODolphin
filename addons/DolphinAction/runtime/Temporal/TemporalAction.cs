using Godot;

namespace GODolphin.Action
{
    public abstract class TemporalAction : ActionExecutor
    {
        private double _duration,
            _time;

        private Curve _interpolation;

        private bool _reverse,
            _began,
            _complete;

        public override bool Act(double delta)
        {
            if (_complete)
            {
                return true;
            }

            var pool = GetPool();
            SetPool(null);
            try
            {
                if (!_began)
                {
                    Begin();
                    _began = true;
                }

                _time += delta;
                _complete = _time >= _duration;
                double percent;
                if (_complete)
                    percent = 1;
                else
                {
                    percent = _time / _duration;
                    if (_interpolation != null)
                        percent = _interpolation.Sample((float)percent);
                }
                Update(_reverse ? 1 - percent : percent);
                if (_complete)
                    End();
                return _complete;
            }
            finally
            {
                SetPool(pool);
            }
        }

        protected virtual void Begin() { }

        protected virtual void End() { }

        protected abstract void Update(double percent);

        public void Finish()
        {
            _time = _duration;
        }

        public override void Restart()
        {
            _time = 0;
            _began = false;
            _complete = false;
        }

        public override void Reset()
        {
            base.Reset();
            _reverse = false;
            _interpolation = null;
        }

        public T SetInterpolation<T>(Curve interpolation)
            where T : TemporalAction
        {
            _interpolation = interpolation;
            return this as T;
        }

        public T SetReverse<T>()
            where T : TemporalAction
        {
            _reverse = true;
            return this as T;
        }

        public void SetDuration(double duration)
        {
            _duration = duration;
        }
    }
}

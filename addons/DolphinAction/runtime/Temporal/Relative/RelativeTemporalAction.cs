namespace GODolphin.Action
{
    public abstract class RelativeTemporalAction : TemporalAction
    {
        private double _lastPercent;

        protected override void Begin()
        {
            _lastPercent = 0;
        }

        protected override void Update(double percent)
        {
            UpdateRelative(percent - _lastPercent);
            _lastPercent = percent;
        }

        protected abstract void UpdateRelative(double percentDelta);
    }
}

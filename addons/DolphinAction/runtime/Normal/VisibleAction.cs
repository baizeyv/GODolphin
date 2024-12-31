using GODolphin.Pool;
using Godot;

namespace GODolphin.Action
{
    public class VisibleAction : ActionExecutor
    {
        private CanvasItem _canvasItem;

        private bool _isVisible;

        public override bool Act(double delta)
        {
            _canvasItem.SetVisible(_isVisible);
            return true;
        }

        public void SetVisible(CanvasItem canvasItem, bool visible)
        {
            _canvasItem = canvasItem;
            _isVisible = visible;
        }
    }

    public static class VisibleActionExtensions
    {
        public static VisibleAction Visible(this CanvasItem self, bool visible)
        {
            var action = SafeObjectPool<VisibleAction>.Instance.Obtain();
            action.SetVisible(self, visible);
            return action;
        }
    }
}

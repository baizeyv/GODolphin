using GODolphin.Pool;
using Godot;

namespace GODolphin.Action
{
    public class RemoveNodeAction : ActionExecutor
    {
        private bool _removed;

        private Node _removeNode;

        public override bool Act(double delta)
        {
            if (!_removed)
            {
                _removed = true;
                _removeNode.QueueFree();
            }

            return true;
        }

        public override void Restart()
        {
            base.Restart();
            _removed = false;
        }

        public void SetRemoveNode(Node removeNode)
        {
            _removeNode = removeNode;
        }
    }

    public static class RemoveNodeActionExtensions
    {
        public static RemoveNodeAction RemoveNode(this Node self)
        {
            var action = SafeObjectPool<RemoveNodeAction>.Instance.Obtain();
            action.SetRemoveNode(self);
            return action;
        }
    }
}

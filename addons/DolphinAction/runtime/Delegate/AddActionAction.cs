using GODolphin.Pool;
using Godot;

namespace GODolphin.Action
{
    public class AddActionAction : DelegateAction
    {
        private Node _node;

        protected override bool Delegate(double delta)
        {
            _node?.AppendAction(ActionExe);
            return true;
        }

        public void SetNode(Node node)
        {
            _node = node;
        }
    }

    public static class AddActionActionExtensions
    {
        public static AddActionAction AddAction(this Node self, ActionExecutor action)
        {
            var myAction = SafeObjectPool<AddActionAction>.Instance.Obtain();
            myAction.SetNode(self);
            myAction.ActionExe = action;
            return myAction;
        }
    }
}

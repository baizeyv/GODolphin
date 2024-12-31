using System.Collections.Generic;
using GODolphin.Pool;
using Godot;

namespace GODolphin.Action
{
    public class AfterAction : DelegateAction
    {
        private List<ActionExecutor> _waitForActions = new();

        private ActionNode _actionNode;

        protected override bool Delegate(double delta)
        {
            var currentActions = _actionNode.Actions;
            if (currentActions.Count == 1)
                _waitForActions.Clear();
            for (var i = _waitForActions.Count - 1; i >= 0; i--)
            {
                var action = _waitForActions[i];
                var index = currentActions.IndexOf(action);
                if (index == -1)
                    _waitForActions.RemoveAt(i);
            }

            if (_waitForActions.Count > 0)
                return false;
            return ActionExe.Act(delta);
        }

        public override void Restart()
        {
            base.Restart();
            _waitForActions.Clear();
        }

        public void SetActionNode(ActionNode actionNode)
        {
            _actionNode = actionNode;
            _waitForActions.AddRange(actionNode.Actions);
        }
    }

    public static class AfterActionExtensions
    {
        public static AfterAction After(this Node self, ActionExecutor action)
        {
            var myAction = SafeObjectPool<AfterAction>.Instance.Obtain();
            myAction.SetActionNode(self.ActionNode());
            myAction.ActionExe = action;
            return myAction;
        }
    }
}

using System;
using System.Collections.Generic;
using Godot;

namespace GODolphin.Action
{
    public partial class ActionNode : Node
    {
        public readonly List<ActionExecutor> Actions = new();

        public override void _Process(double delta)
        {
            var actions = Actions;
            for (var i = 0; i < actions.Count; i++)
            {
                var action = actions[i];
                if (action.Act(delta) && i < actions.Count)
                {
                    var current = actions[i];
                    var actionIndex = current == action ? i : actions.IndexOf(action);
                    if (actionIndex != -1)
                    {
                        actions.RemoveAt(actionIndex);
                        action.Recycle();
                        i--;
                    }
                }
            }
        }

        public void AddAction(ActionExecutor action)
        {
            Actions.Add(action);
        }

        public void RemoveAction(ActionExecutor action)
        {
            Actions.Remove(action);
            action.Recycle();
        }

        public void ClearActions()
        {
            foreach (var t in Actions)
            {
                t.Recycle();
            }

            Actions.Clear();
        }
    }

    public static class ActionExecutorExtensions
    {
        private const string ActionNodeName = "MUA-ActionNode-";

        public static void AppendAction(this Node node, ActionExecutor action)
        {
            if (action == null)
                return;
            var actionNode = node.ActionNode();
            actionNode.AddAction(action);
        }

        public static void RemoveAction(this Node node, ActionExecutor action)
        {
            if (action == null)
                return;
            var actionNode = node.ActionNode();
            actionNode?.RemoveAction(action);
        }

        public static void ClearActions(this Node node)
        {
            var actionNode = node.ActionNode();
            actionNode?.ClearActions();
        }

        public static ActionNode ActionNode(this Node self)
        {
            var actionNode = self.GetNodeOrNull<ActionNode>($"{ActionNodeName}{self.Name}");
            if (actionNode == null)
            {
                actionNode = new ActionNode();
                actionNode.Name = $"{ActionNodeName}{self.Name}";
                self.AddChild(actionNode);
            }

            return actionNode;
        }
    }
}

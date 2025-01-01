using System;
using System.Collections.Generic;
using Godot;
using System.Linq;
using System.Reflection;
using GODolphin;
using GODolphin.Singleton;
using Godot.Collections;

[Tool]
public partial class SingletonWindow : Control
{
    [Export] public Button RefreshButton;

    [Export] public BoxContainer NodeContainer;

    private Array<SingletonNode> _singletonNodes = new();

    public override void _EnterTree()
    {
        RefreshButton.Connect("pressed", new Callable(this, nameof(OnPressed)));
        RefreshSingletonResource();
    }

    private void OnPressed()
    {
        RefreshSingletonResource();
    }

    private void Clear()
    {
        foreach (var node in _singletonNodes)
        {
            node.QueueFree();
        }

        _singletonNodes.Clear();
    }

    private void UpdatePanel(IEnumerable<Type> nodeSingletonTypeList, IEnumerable<Type> normalSingletonTypeList)
    {
        PackedScene prefab = GD.Load("res://addons/DolphinSingleton/prefab/SingletonNode.tscn") as PackedScene;
        foreach (var type in nodeSingletonTypeList)
        {
            var typeName = type.Name;
            var fullName = type.FullName;
            var attributes = type.GetCustomAttributes(true);
            bool forceContinue = false;
            foreach (var atr in attributes)
            {
                var defineAtr = atr as NodePathAttribute;
                if (defineAtr == null)
                    continue;
                var nodeName = defineAtr.NodeName;
                var node = prefab?.Instantiate() as SingletonNode;
                _singletonNodes.Add(node);
                node.Setup(typeName, nodeName, fullName);
                NodeContainer.AddChild(node);
                forceContinue = true;
                break;
            }

            if (forceContinue) continue;
            {
                var node = prefab?.Instantiate() as SingletonNode;
                _singletonNodes.Add(node);
                node.Setup(typeName, typeName, fullName);
                NodeContainer.AddChild(node);
            }
        }

        foreach (var type in normalSingletonTypeList)
        {
            var typeName = type.Name;
            var fullName = type.FullName;
            var node = prefab?.Instantiate() as SingletonNode;
            _singletonNodes.Add(node);
            node.Setup(typeName, "----", fullName);
            NodeContainer.AddChild(node);
        }
    }

    private void RefreshSingletonResource()
    {
        Clear();
        var assembly = Assembly.GetExecutingAssembly();
        var derivedTypes = assembly.GetTypes().Where(type =>
        {
            var baseType = type.BaseType;
            if (baseType is { IsGenericType: true } &&
                baseType.GetGenericTypeDefinition() == typeof(NodeSingleton<>))
            {
                var genericArguments = baseType.GetGenericArguments();
                return genericArguments.Length == 1 && genericArguments[0] == type;
            }

            return false;
        });
        string[] managerTypes = new string[derivedTypes.Count()];
        for (int i = 0; i < derivedTypes.Count(); i++)
        {
            managerTypes[i] = derivedTypes.ElementAt(i).FullName;
        }

        var resource = new SingletonResource()
        {
            ManagerTypeArray = managerTypes
        };

        var path = GODolphinConst.ManagerTypeResource;

        var error = ResourceSaver.Save(resource, path);
        if (error == Error.Ok)
        {
            GD.PrintRich("[color=green]Save ManagerType Successful ![/color]");
        }
        else
        {
            GD.PrintRich($"[color=red]Save ManagerType FAILD ! {error}[/color]");
        }

        var normalDerivedTypes = assembly.GetTypes().Where(type =>
        {
            var baseType = type.BaseType;
            if (baseType is { IsGenericType: true } && baseType.GetGenericTypeDefinition() == typeof(Singleton<>))
            {
                var genericArguments = baseType.GetGenericArguments();
                return genericArguments.Length == 1 && genericArguments[0] == type;
            }

            return false;
        });
        UpdatePanel(derivedTypes, normalDerivedTypes);
    }

    public override void _ExitTree()
    {
        RefreshButton.Disconnect("pressed", new Callable(this, nameof(OnPressed)));
    }
}
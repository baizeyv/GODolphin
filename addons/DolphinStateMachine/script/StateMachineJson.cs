using Godot.Collections;

namespace GODolphin.StateMachine;

public class StateMachineJson
{
    public string EnumTypeFullName;

    public string StateMachineName;

    public string State; // State为空代表是在初始化当前状态机

    public bool NeedRemove;

    public string Command;

    public Dictionary Dictionarify()
    {
        Dictionary dic = new()
        {
            {"EnumTypeFullName", EnumTypeFullName},
            {"StateMachineName", StateMachineName},
            {"State", State},
            {"NeedRemove", NeedRemove},
            {"Command", Command}
        };
        return dic;
    }

    public static StateMachineJson ParseDictionary(Dictionary dic)
    {
        return new StateMachineJson()
        {
            EnumTypeFullName = dic["EnumTypeFullName"].ToString(),
            StateMachineName = dic["StateMachineName"].ToString(),
            State = dic["State"].ToString(),
            NeedRemove = (bool)dic["NeedRemove"],
            Command = dic["Command"].ToString(),
        };
    }
}
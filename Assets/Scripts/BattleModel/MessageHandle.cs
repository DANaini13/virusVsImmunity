using System;
using System.Collections.Generic;
using BattleMath;
using UnityEngine;

class ModelMessageWrapper
{

    private BattleModel battleModel;

    private object parseArg(PlayerArg arg) {
        switch (arg.type) {
            case "int": return int.Parse(arg.value);
            case "string": return arg.value;
            case "vector2": return vector2.fromString(arg.value);
            case "decimal": return decimal.Parse(arg.value);
        }
        return 0;
    }

    public ModelMessageWrapper(BattleModel model)
    {
        this.battleModel = model;
    }

    public void doTest() 
    {
        // Debug.Log("do Test");
        // PlayerArg playerArg0 = new PlayerArg("int", "100");
        // PlayerArg playerArg1 = new PlayerArg("vector2", (new vector2(0.1m, 5.0m)).ToString());
        // List<PlayerArg> argList = new List<PlayerArg>();
        // argList.Add(playerArg0);
        // argList.Add(playerArg1);
        // PlayerOperation playerOperation = new PlayerOperation(1, "move", argList);
        // string json = JsonUtility.ToJson(playerOperation);
        // Debug.Log(json);
        // implementOperation(json);
    }

    public void implementOperation(string operation)
    {
        PlayerOperation playerOperation = JsonUtility.FromJson<PlayerOperation>(operation);
        Type type = typeof(BattleModel);
        System.Reflection.MethodInfo methodInfo = type.GetMethod(playerOperation.operationName);
        
        object[] argList = new object[playerOperation.argList.Count];
        int i=0;
        foreach(var arg in playerOperation.argList)
        {
            argList[i] = parseArg(arg);
            ++i;
        }
        methodInfo.Invoke(battleModel, argList); 
    }
}

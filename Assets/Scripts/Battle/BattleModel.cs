using System;
using System.Collections.Generic;
using BattleMath;
using UnityEngine;

[Serializable]
class PlayerArg{
    public PlayerArg(string type, string value)
    {
        this.type = type;
        this.value = value;
    }
    public string type;
    public string value;
}

[Serializable]
class PlayerOperation 
{
    public PlayerOperation(int playerId, string operationName, List<PlayerArg> argList)
    {
        this.playerId = playerId;
        this.operationName = operationName;
        this.argList = argList;
    }
    [SerializeField]
    public int playerId;
    [SerializeField]
    public string operationName;
    [SerializeField]
    public List<PlayerArg> argList;
}

class ModelMessageWrapper
{
    public ModelMessageWrapper(BattleModel model)
    {
        this.battleModel = model;
    }

    private BattleModel battleModel;

    public void doTest() 
    {
        PlayerArg playerArg0 = new PlayerArg("int", "100");
        PlayerArg playerArg1 = new PlayerArg("vector2", (new vector2(0.1m, 5.0m)).ToString());
        List<PlayerArg> argList = new List<PlayerArg>();
        argList.Add(playerArg0);
        argList.Add(playerArg1);
        PlayerOperation playerOperation = new PlayerOperation(1, "move", argList);
        string json = JsonUtility.ToJson(playerOperation);
        Debug.Log(json);
        implementOperation(json);
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
    private object parseArg(PlayerArg arg) {
        switch (arg.type) {
            case "int": return int.Parse(arg.value);
            case "string": return arg.value;
            case "vector2": return vector2.fromString(arg.value);
        }
        return 0;
    }
}

class BattleModel
{
    private Dictionary<int, Player> player_map;

    public BattleModel()
    {
        player_map = new Dictionary<int, Player>();
    }

    public void addPlayer(int id, int type)
    {
        player_map.Add(id, new Macrophages());
    }

    public void move(int playerId, vector2 direction)
    {
        Debug.Log("playerId = " + playerId);
        Debug.Log("Direction = " + direction.x + "-" + direction.y);
    }

}

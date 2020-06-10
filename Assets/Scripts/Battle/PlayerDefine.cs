using System;
using System.Collections.Generic;
using BattleMath;
using UnityEngine;

abstract class Player 
{
    protected vector2 location;
    protected decimal speed;
    public abstract void move(vector2 direction);
    public abstract void implementTask(int key);
} 
class Macrophages: Player
{
    public override void implementTask(int key)
    {
    }

    public override void move(vector2 direction)
    {
        location = vector2.plus(location, vector2.mutiply(direction, speed));
    }
}

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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using BattleMath;
using UnityEngine;

class BattleModel
{
    private Dictionary<int, Player> player_map;

    public BattleModel()
    {
        player_map = new Dictionary<int, Player>();
    }

    public void addPlayer(int id, int type)
    {
        player_map.Add(id, new Macrophages(new vector2(0, 0)));
    }

    public PlayerOperation move(int playerId, vector2 direction)
    {
        if(player_map.ContainsKey(playerId)) {
            var player = player_map[playerId];
            player.setLocation(vector2.plus(player.getLocation(), direction));
        }
        string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        PlayerArg playerArg0 = new PlayerArg("int", playerId.ToString());
        PlayerArg playerArg1 = new PlayerArg("vector2", direction.ToString());
        List<PlayerArg> argList = new List<PlayerArg>();
        argList.Add(playerArg0);
        argList.Add(playerArg1);
        PlayerOperation playerOperation = new PlayerOperation(1, methodName, argList);
        return playerOperation;
    }

    public vector2 getPlayerPosition(int playerId)
    {
        if(!player_map.ContainsKey(playerId)) return new vector2(0, 0);
        return player_map[playerId].getLocation();
    }

}

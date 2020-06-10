using System.Collections.Generic;
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
        player_map.Add(id, new Macrophages());
    }

    public void move(int playerId, vector2 direction)
    {
        Debug.Log("playerId = " + playerId);
        Debug.Log("Direction = " + direction.x + "-" + direction.y);
    }

}

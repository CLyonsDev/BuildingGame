using UnityEngine;
using System.Collections.Generic;

public class PlayerList : MonoBehaviour {

    private const string PLAYER_ID_PREFIX = "Player ";

    private static Dictionary<string, PlayerHealth> players = new Dictionary<string, PlayerHealth>();

    public static void RegisterPlayer(string _netID, PlayerHealth _player)
    {
        string _playerID = PLAYER_ID_PREFIX + _netID;
        players.Add(_playerID, _player);
        _player.transform.name = _playerID;
    }

    public static PlayerHealth GetPlayer(string _playerID)
    {
        return players[_playerID];
    }

    public static void UnRegisterPlayer(string _playerID)
    {
        players.Remove(_playerID);
    }
}

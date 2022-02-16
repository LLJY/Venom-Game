using System;
using Player;
using UnityEngine;
using World;

public class GameCache: MonoBehaviour
{
    public GameObject Player;
    public GameObject worldManager;

    public static GameObject _worldManagerStatic;
    private static WorldManager worldManagerScriptStatic;

    public static WorldManager worldManagerScript
    {
        get
        {
            if (worldManagerScriptStatic == null)
            {
                worldManagerScriptStatic = _worldManagerStatic.GetComponent<WorldManager>();
            }

            return worldManagerScriptStatic;
        }
    }

    public static GameObject playerStatic;

    private static Character _playerScript;
    public static Character playerScript
    {
        get
        {
            if (_playerScript == null)
            {
                _playerScript = playerStatic.GetComponent<Character>();
            }
            return _playerScript;
        }
    }

    private void Awake()
    {
        playerStatic = Player;
        _worldManagerStatic = worldManager;
        enabled = false;
    }
}

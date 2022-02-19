using System;
using Game;
using Obstacles.Turret;
using Player;
using UnityEngine;
using World;

public class GameCache: MonoBehaviour
{
    public GameObject Player;
    public GameObject worldManager;
    public GameObject bulletManager;

    public static GameData GameData;

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
    
    public static GameObject bulletManagerStatic;
    private static BulletObjectPool _bulletObjectPool;
    public static BulletObjectPool bulletObjectPool
    {
        get
        {
            if (_bulletObjectPool == null)
            {
                _bulletObjectPool = bulletManagerStatic.GetComponent<BulletObjectPool>();
            }
            return _bulletObjectPool;
        }
    }

    private void Awake()
    {
        bulletManagerStatic = bulletManager;
        playerStatic = Player;
        _worldManagerStatic = worldManager;
        enabled = false;
    }
}

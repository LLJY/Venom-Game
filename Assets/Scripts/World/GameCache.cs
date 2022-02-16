using System;
using Player;
using UnityEngine;

public class GameCache: MonoBehaviour
{
    public GameObject Player;
    
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
        enabled = false;
    }
}

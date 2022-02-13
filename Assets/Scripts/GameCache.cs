using System;
using Player;
using UnityEngine;

public class GameCache: MonoBehaviour
{
    public GameObject Player;

    public Character PlayerScript;

    public static GameObject playerStatic;
    public static Character playerScriptStatic;

    private void Awake()
    {
        playerStatic = Player;
        playerScriptStatic = PlayerScript;
        enabled = false;
    }
}

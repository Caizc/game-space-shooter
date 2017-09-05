using UnityEngine;
using System.Collections;

/// <summary>
/// 游戏管理
/// </summary>
public class GameMgr : MonoBehaviour
{
    public static GameMgr instance;

    public string id = "Tank";

    void Awake()
    {
        instance = this;
    }
}
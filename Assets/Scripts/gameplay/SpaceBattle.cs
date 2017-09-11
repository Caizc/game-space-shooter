using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 太空战斗场景管理
/// </summary>
public class SpaceBattle
{
    private static SpaceBattle _instance;

    // 获取 SpaceBattle 单例
    public static SpaceBattle Instance
    {
        get
        {
            if (null == _instance)
            {
                _instance = new SpaceBattle();
            }
            return _instance;
        }
    }

    // 场景中的所有飞船
    public Dictionary<string, Ship> ShipDict = new Dictionary<string, Ship>();

    // 战斗是否开始
    public bool isBattleStart = false;

    /// <summary>
    /// 私有构造方法，防止单例模式下产生多个类的实例
    /// </summary>
    private SpaceBattle()
    {
        // nothing to do her.
    }

    /// <summary>
    /// 开始战斗
    /// </summary>
    /// <param name="proto"></param>
    public void StartBattle(ProtocolBytes proto)
    {
        // 解析协议
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        if (protoName != "Fight")
        {
            return;
        }

        // 飞船总数
        int count = proto.GetInt(start, ref start);

        //TODO: 清理场景

        // 处理每一架飞船
        for (int i = 0; i < count; i++)
        {
            string id = proto.GetString(start, ref start);
            int team = proto.GetInt(start, ref start);
            int spid = proto.GetInt(start, ref start);

            // 产生飞船
            SpawnShip(id, team, spid);
        }

        // 向消息分发管理器注册相应事件的回调方法
        NetMgr.srvConn.msgDist.AddListener("UpdateShipInfo", RecvUpdateShipInfo);
//        NetMgr.srvConn.msgDist.AddListener("Shooting", RecvShooting);
//        NetMgr.srvConn.msgDist.AddListener("Hit", RecvHit);
//        NetMgr.srvConn.msgDist.AddListener("Result", RecvResult);

        isBattleStart = true;
    }

    /// <summary>
    /// 在场景中生成飞船
    /// </summary>
    /// <param name="id"></param>
    /// <param name="team"></param>
    /// <param name="spid"></param>
    private void SpawnShip(string id, int team, int spid)
    {
        // 获取预设的出生点
        Transform spawnPoints = GameObject.FindWithTag("SpwanPoints").transform;
        // 出生点坐标
        Transform spawnTrans;

        if (team == 1)
        {
            spawnTrans = spawnPoints.GetChild(0).GetChild(spid - 1);
        }
        else
        {
            spawnTrans = spawnPoints.GetChild(1).GetChild(spid - 1);
        }

        if (spawnTrans == null)
        {
            Debug.LogError("SpawnShip 出生点错误！");
            return;
        }

        // 获取飞船 prefab
        GameObject battleControllerObject = GameObject.FindWithTag("BattleController");
        if (battleControllerObject == null)
        {
            Debug.LogError("场景中缺失 BattleController 对象！");
            return;
        }

        BattleController battleController = battleControllerObject.GetComponent<BattleController>();
        GameObject[] shipPrefabs = battleController.characterPrefabs;

        if (shipPrefabs.Length < 2)
        {
            Debug.LogError("飞船预设数量不够！");
            return;
        }

        // 产生飞船
        GameObject shipObj = (GameObject) Object.Instantiate(shipPrefabs[team - 1]);
        shipObj.name = id;
        shipObj.transform.position = spawnTrans.position;
        shipObj.transform.rotation = spawnTrans.rotation;

        // 构造 Ship 对象
        Ship ship = new Ship();
        ship.playerController = shipObj.GetComponent<PlayerController>();
        ship.team = team;

        // 保存到场景的 Ship 列表中
        ShipDict.Add(id, ship);

        // 角色操控处理
        if (id == GameMgr.instance.id)
        {
            ship.playerController.ControlMode = PlayerController.CharacterControlMode.Player;

            // 设置摇杆操作控制的角色
            GameObject inputManagerObject = GameObject.FindWithTag("InputManager");
            if (inputManagerObject == null)
            {
                Debug.LogError("场景中缺失 InputManager 对象！");
                return;
            }

            InputManager inputManager = battleControllerObject.GetComponent<InputManager>();
            inputManager.Player = ship.playerController;
        }
        else
        {
            ship.playerController.ControlMode = PlayerController.CharacterControlMode.Net;
        }
    }

    /// <summary>
    /// 处理接收到的单位同步信息
    /// </summary>
    /// <param name="protocol"></param>
    private void RecvUpdateShipInfo(ProtocolBase protocol)
    {
        // 解析协议
        int start = 0;
        ProtocolBytes proto = (ProtocolBytes) protocol;
        string protoName = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);

        // 如果该位置同步消息包是自己发出的，则忽略，不进行位置和转向同步，避免被拉回到之前的位置上
        if (id == GameMgr.instance.id)
        {
            return;
        }

        Vector2 mov;
        Vector2 rot;
        // 移动向量
        mov.x = proto.GetFloat(start, ref start);
        mov.y = proto.GetFloat(start, ref start);
        // 转向向量
        rot.x = proto.GetFloat(start, ref start);
        rot.y = proto.GetFloat(start, ref start);

        // 处理
        Debug.Log("RecvUpdateShipInfo - " + id);
        if (!ShipDict.ContainsKey(id))
        {
            Debug.Log("RecvUpdateShipInfo ship == null ");
            return;
        }

        // 设置飞船的移动和转向
        Ship ship = ShipDict[id];
        ship.playerController.deltaMovement = mov;
        ship.playerController.deltaRotation = rot;
    }
}
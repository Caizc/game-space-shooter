using UnityEngine;

/// <summary>
/// 挂载到场景中的程序入口脚本
/// </summary>
public class Root : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(this);
        Application.runInBackground = true;

        // 打开登录面板
        PanelMgr.instance.OpenPanel<LoginPanel>("");

        Debug.Log("=== Application has been started up! ===");
    }

    void FixedUpdate()
    {
        // 固定时间间隔更新，处理消息队列中的消息
        NetMgr.Update();
    }
}
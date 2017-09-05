using UnityEngine;

/// <summary>
/// 挂载到场景中的程序入口脚本
/// </summary>
public class Root : MonoBehaviour
{
    void Start()
    {
        Application.runInBackground = true;
        PanelMgr.instance.OpenPanel<LoginPanel>("");
    }

    void Update()
    {
        NetMgr.Update();
    }
}
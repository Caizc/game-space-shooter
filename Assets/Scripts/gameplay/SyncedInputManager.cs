using TrueSync;
using UnityEngine;

public class SyncedInputManager
{
    // 移动增量
    private Vector2 _deltaMovement;

    // 转向增量
    private Vector2 _deltaRotation;

    public Vector2 DeltaMovement
    {
        get { return _deltaMovement; }

        set { _deltaMovement = value; }
    }

    public Vector2 DeltaRotation
    {
        get { return _deltaRotation; }

        set { _deltaRotation = value; }
    }

    /// <summary>
    /// 处理左摇杆移动事件
    /// </summary>
    public void LeftJoystickMovement(Vector2 movement)
    {
        if (SpaceBattle.Instance.isBattleStart)
        {
            _deltaMovement = movement;
        }
    }

    /// <summary>
    /// 处理右摇杆移动事件
    /// </summary>
    public void RightJoystickMovement(Vector2 movement)
    {
        if (SpaceBattle.Instance.isBattleStart)
        {
            _deltaRotation = movement;
        }
    }
}
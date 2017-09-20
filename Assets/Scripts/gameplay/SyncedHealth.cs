using System;
using TrueSync;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 生命值/耐久度
/// </summary>
public class SyncedHealth : TrueSyncBehaviour
{
    [SerializeField] private GameObject explosion;
    [SerializeField] private GameObject playerExplosion;

    /// <summary>
    /// 角色生命值/物品耐久度
    /// </summary>
    [AddTracking] public int Health = 100;

    public override void OnSyncedStart()
    {
    }

    /// <summary>
    /// 受到伤害扣生命值/物品耐久度降低
    /// </summary>
    /// <param name="damage">伤害值</param>
    public void TakeDamage(int damage)
    {
        Health -= damage;

        PlayFX();

        if (Health <= 0)
        {
            Health = 0;
            Death();
        }
    }

    /// <summary>
    /// 播放视觉特效和声音特效
    /// </summary>
    private void PlayFX()
    {
        if (null != explosion)
        {
            Instantiate(explosion, transform.position, transform.rotation);
        }
    }

    /// <summary>
    /// 死亡/损毁
    /// </summary>
    private void Death()
    {
        TrueSyncManager.SyncedDestroy(this.gameObject);

        if (null != playerExplosion)
        {
            Instantiate(playerExplosion, transform.position, transform.rotation);
        }
    }
}
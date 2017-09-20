/// <summary>
/// 网络状态
/// </summary>
public class NetStatus
{
    /// <summary>
    /// 获取 NetStatus 单例
    /// </summary>
    public static NetStatus Instance
    {
        get
        {
            if (null == _instance)
            {
                _instance = new NetStatus();
            }
            return _instance;
        }
    }

    /// <summary>
    /// 平均延迟
    /// </summary>
    public float RoundTripTime
    {
        get { return _lastRtt; }
    }

    private static NetStatus _instance;

    /// <summary>
    /// 总延迟 = 延迟1 + 延迟2 + 延迟3 + ...
    /// </summary>
    private float _rttSum = 0.0f;

    /// <summary>
    /// 延迟计数（每次往 RTTSum 中加入一个延迟，该值加一）
    /// </summary>
    private int _rttCount = 0;

    /// <summary>
    /// 最后一个平均延迟 = 总延迟/延迟计数
    /// </summary>
    private float _lastRtt = 0.0f;

    /// <summary>
    /// 私有构造方法，防止单例模式下产生多个类的实例
    /// </summary>
    private NetStatus()
    {
        // nothing to do here.
    }

    public void AddRtt(float rtt)
    {
        _rttSum += rtt;
        _rttCount++;

        // 每累积 4 个延迟数据则计算一次平均延迟
        if (_rttCount > 3)
        {
            _lastRtt = _rttSum / _rttCount;
            ResetRtt();
        }
    }

    /// <summary>
    /// 重置延迟相关计数器
    /// </summary>
    private void ResetRtt()
    {
        _rttSum = 0.0f;
        _rttCount = 0;
    }
}
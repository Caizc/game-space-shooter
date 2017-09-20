using TrueSync;
using UnityEngine;

[System.SerializableAttribute]
public class BoundaryFP
{
    public FP xMin, xMax, zMin, zMax;
}

public class SyncedPlayerController : TrueSyncBehaviour
{
    // 移动速度
    [SerializeField] private FP speed;

    // 转向速度
    [SerializeField] private FP rotateSpeed;

    [SerializeField] private BoundaryFP boundary;

    // 机身倾斜系数
    [SerializeField] private FP tilt;

    [SerializeField] private GameObject shot;
    [SerializeField] private Transform shotSpawn;
    [SerializeField] private FP fireDelta = 0.25;

    [SerializeField] private ParticleSystem particleSystem1;
    [SerializeField] private ParticleSystem particleSystem2;

    private SyncedInputManager _syncedInputManager;
    private AudioSource _audioSource;

    private FP _myTime = 0;
    private FP _nextFire = 0.25;

    private FP _movX;
    private FP _movY;
    private FP _rotX;
    private FP _rotY;

    void Start()
    {
        GameObject inputManagerObject = GameObject.FindWithTag("InputManager");
        if (inputManagerObject == null)
        {
            Debug.LogError("场景中缺失 SyncedInputManager 对象！");
            return;
        }

        _syncedInputManager = inputManagerObject.GetComponent<SyncedInputManager>();
        if (_syncedInputManager == null)
        {
            Debug.LogError("场景中缺失 SyncedInputManager 组件！");
        }

        _audioSource = GetComponent<AudioSource>();
    }

    public override void OnSyncedStart()
    {
        tsTransform.position = new TSVector(TSRandom.Range(-5, 5), 0, TSRandom.Range(-5, 5));
    }

    public override void OnSyncedInput()
    {
        if (SpaceBattle.Instance.isBattleStart)
        {
            // 将飞船的位置和转向信息保存到 SyncedData 中以发送往 Server 同步到所有 Client
            TrueSyncInput.SetFP(0, _syncedInputManager.DeltaMovement.x);
            TrueSyncInput.SetFP(1, _syncedInputManager.DeltaMovement.y);
            TrueSyncInput.SetFP(2, _syncedInputManager.DeltaRotation.x);
            TrueSyncInput.SetFP(3, _syncedInputManager.DeltaRotation.y);
        }
    }

    public override void OnSyncedUpdate()
    {
        // TODO: 收到更新的操控指令

        _movX = TrueSyncInput.GetFP(0);
        _movY = TrueSyncInput.GetFP(1);
        _rotX = TrueSyncInput.GetFP(2);
        _rotY = TrueSyncInput.GetFP(3);

        // 移动
        Move();

        // 开火
        Fire();
    }

    /// <summary>
    /// 移动
    /// </summary>
    private void Move()
    {
        if (!(_movX == 0 && _movY == 0))
        {
            bool gotIt = true;
        }

        tsRigidBody.velocity = new TSVector(_movX * speed, 0, _movY * speed);

        // 限制角色移动范围
        tsRigidBody.position = new TSVector(
            TSMath.Clamp(tsRigidBody.position.x, boundary.xMin, boundary.xMax),
            0,
            TSMath.Clamp(tsRigidBody.position.z, boundary.zMin, boundary.zMax)
        );

        // 左右平移时稍微倾斜一下机身（绕 z 轴）
        tsRigidBody.rotation = TSQuaternion.Euler(0, 0, tsRigidBody.velocity.x * -tilt);

        // 旋转方向
        if (_rotX == 0 && _rotY == 0)
        {

            tsTransform.rotation = TSQuaternion.Lerp(tsTransform.rotation, TSQuaternion.identity,
                TrueSyncManager.DeltaTime * rotateSpeed);
        }
        else
        {
            TSVector joystickKnobPos = new TSVector(_rotX, 0, _rotY);
            TSQuaternion targetRot = TSQuaternion.LookRotation(joystickKnobPos);
            tsTransform.rotation =
                TSQuaternion.Lerp(tsTransform.rotation, targetRot, TrueSyncManager.DeltaTime * rotateSpeed);
        }

        // 旋转粒子系统
        float r = (float) tsTransform.rotation.eulerAngles.y * Mathf.Deg2Rad;
        if (null != particleSystem1)
        {
            particleSystem1.startRotation = r;
        }
        if (null != particleSystem2)
        {
            particleSystem2.startRotation = r;
        }
    }

    /// <summary>
    /// 开火
    /// </summary>
    private void Fire()
    {
        _myTime += TrueSyncManager.DeltaTime;

        bool canFire = (_rotX > 0.8 || _rotX < -0.8 || _rotY > 0.8 ||
                        _rotY < -0.8) && _myTime > _nextFire;

        if (canFire)
        {
            _nextFire = _myTime + fireDelta;

            Instantiate(shot, shotSpawn.position, shotSpawn.rotation);

            _nextFire = _nextFire - _myTime;
            _myTime = 0;

            _audioSource.Play();
        }
    }
}
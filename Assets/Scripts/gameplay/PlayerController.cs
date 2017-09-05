using UnityEngine;

[System.SerializableAttribute]
public class Boundary
{
    public float xMin, xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private Boundary boundary;
    [SerializeField] private float tilt;

    [SerializeField] private GameObject shot;
    [SerializeField] private Transform shotSpawn;
    [SerializeField] private float fireDelta = 0.25f;

   [SerializeField] private ParticleSystem particleSystem1;
    [SerializeField] private ParticleSystem particleSystem2;

    private Rigidbody _rigidbody;
    private AudioSource _audioSource;

    private float _myTime = 0.0f;
    private float _nextFire = 0.25f;

    [HideInInspector] public Vector2 deltaMovement;
    [HideInInspector] public Vector2 deltaRotation;

    public enum InputForcedMode
    {
        None,
        Mobile,
        Desktop
    }

    public InputForcedMode ForcedMode;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        // 开火
        Fire();

        float moveHorizontal = 0f;
        float moveVertical = 0f;

        // 使用摇杆控制
        if (ForcedMode == InputForcedMode.Mobile)
        {
            moveHorizontal = deltaMovement.x;
            moveVertical = deltaMovement.y;
        }

        // 使用键盘控制
        if (ForcedMode == InputForcedMode.Desktop)
        {
            moveHorizontal = Input.GetAxis("Horizontal");
            moveVertical = Input.GetAxis("Vertical");
        }

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        _rigidbody.velocity = movement * speed;

        _rigidbody.position = new Vector3(
            Mathf.Clamp(_rigidbody.position.x, boundary.xMin, boundary.xMax),
            0.0f,
            Mathf.Clamp(_rigidbody.position.z, boundary.zMin, boundary.zMax)
        );

        _rigidbody.rotation = Quaternion.Euler(0.0f, 0.0f, _rigidbody.velocity.x * -tilt);

        // 旋转方向
        if (deltaRotation.Equals(Vector2.zero))
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * rotateSpeed);
        }
        else
        {
            Vector3 joystickKnobPos = new Vector3(deltaRotation.x, 0.0f, deltaRotation.y);
            Quaternion targetRot = Quaternion.LookRotation(joystickKnobPos);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * rotateSpeed);
        }

        // 旋转粒子系统
        float r = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
        particleSystem1.startRotation = r;
        particleSystem2.startRotation = r;
    }

    private void Fire()
    {
        _myTime += Time.deltaTime;

        bool canFire = false;

        switch (ForcedMode)
        {
            case InputForcedMode.Desktop:
                canFire = Input.GetButton("Fire1") && _myTime > _nextFire;
                break;
            case InputForcedMode.Mobile:
                canFire = (deltaRotation.x > 0.8f || deltaRotation.x < -0.8f || deltaRotation.y > 0.8f ||
                           deltaRotation.y < -0.8f) && _myTime > _nextFire;
                break;
            default:
                break;
        }

        if (canFire)
        {
            _nextFire = _myTime + fireDelta;

            Instantiate(shot, shotSpawn.position, shotSpawn.rotation);

            _nextFire = _nextFire - _myTime;
            _myTime = 0.0f;

            _audioSource.Play();
        }
    }
}
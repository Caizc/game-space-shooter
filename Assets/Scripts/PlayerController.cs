using UnityEngine;

[System.SerializableAttribute]
public class Boundary
{
    public float xMin, xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private float speed;
    [SerializeField]
    private Boundary boundary;
    [SerializeField]
    private float tilt;

    [SerializeField]
    private GameObject shot;
    [SerializeField]
    private Transform shotSpawn;
    [SerializeField]
    private float fireDelta = 0.25f;

    private Rigidbody _rigidbody;
    private AudioSource _audioSource;

    private float _myTime = 0.0f;
    private float _nextFire = 0.25f;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        _myTime += Time.deltaTime;

        if (Input.GetButton("Fire1") && _myTime > _nextFire)
        {
            _nextFire = _myTime + fireDelta;

            Instantiate(shot, shotSpawn.position, shotSpawn.rotation);

            _nextFire = _nextFire - _myTime;
            _myTime = 0.0f;

            _audioSource.Play();
        }
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        _rigidbody.velocity = movement * speed;

        _rigidbody.position = new Vector3(
            Mathf.Clamp(_rigidbody.position.x, boundary.xMin, boundary.xMax),
            0.0f,
            Mathf.Clamp(_rigidbody.position.z, boundary.zMin, boundary.zMax)
        );

        _rigidbody.rotation = Quaternion.Euler(0.0f, 0.0f, _rigidbody.velocity.x * -tilt);
    }

}

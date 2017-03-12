using UnityEngine;

public class DestoryByContact : MonoBehaviour
{
    [SerializeField]
    private GameObject explosion;
    [SerializeField]
    private GameObject playerExplosion;

    [SerializeField]
    private int scoreValue;

    private GameController _gameController;

    void Start()
    {
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");

        if (null != gameControllerObject)
        {
            _gameController = gameControllerObject.GetComponent<GameController>();

            if (null == _gameController)
            {
                Debug.Log("Cannot find 'GameController' script!");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boundary") || other.CompareTag("Enemy"))
        {
            return;
        }

        if (null != explosion)
        {
            Instantiate(explosion, transform.position, transform.rotation);
        }

        if (other.CompareTag("Player"))
        {
            Instantiate(playerExplosion, other.transform.position, other.transform.rotation);

            _gameController.GameOver();
        }

        _gameController.AddScore(scoreValue);

        Destroy(other.gameObject);
        Destroy(this.gameObject);
    }
}

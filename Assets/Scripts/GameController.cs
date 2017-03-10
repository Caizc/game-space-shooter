using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    [SerializeField]
    private GameObject hazard;
    [SerializeField]
    private Vector3 spawnValues;
    [SerializeField]
    private int hazardCount;
    [SerializeField]
    private float spawnWait;
    [SerializeField]
    private float waveWait;
    [SerializeField]
    private float startWait;

    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private Text gameOverText;
    [SerializeField]
    private Text restartText;

    private bool _isGameOver;
    private bool _isRestart;
    private int _score;

    void Start()
    {
        _isGameOver = false;
        _isRestart = false;
        gameOverText.enabled = false;
        restartText.enabled = false;

        _score = 0;

        UpdateScore();

        StartCoroutine(SpawnWaves());
    }

    void Update()
    {
        if (_isRestart)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene("Main");
            }
        }
    }

    private IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(startWait);

        while (true)
        {
            for (int i = 0; i < hazardCount; i++)
            {
                Vector3 spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
                Quaternion spawnRotation = Quaternion.identity;

                Instantiate(hazard, spawnPosition, spawnRotation);

                yield return new WaitForSeconds(spawnWait);
            }

            yield return new WaitForSeconds(waveWait);

            if (_isGameOver)
            {
                restartText.text = "Press 'R' for Restart";
                restartText.enabled = true;
                _isRestart = true;
                break;
            }
        }
    }

    public void AddScore(int score)
    {
        _score += score;
        UpdateScore();
    }

    public void GameOver()
    {
        gameOverText.text = "Game Over!";
        gameOverText.enabled = true;
        _isGameOver = true;
    }

    private void UpdateScore()
    {
        scoreText.text = "Score: " + _score;
    }
}

using System;
using TrueSync;
using UnityEngine;
using UnityEngine.UI;

public class SyncedHealth : TrueSyncBehaviour
{
    [SerializeField] private GameObject explosion;
    [SerializeField] private GameObject playerExplosion;

    // 生命值
    [AddTracking] private int _health;



    private Text _healthText;

    public int Health
    {
        get { return _health; }
        set { _health = value; }
    }



    public override void OnSyncedStart()
    {
        _health = 100;
        

        GameObject uiCameraObject = GameObject.Find("UICamera");
        if (uiCameraObject != null)
        {
            try
            {
                _healthText = uiCameraObject.transform.GetChild(2).transform.GetChild(0).GetComponent<Text>();
            }
            catch (Exception e)
            {
                Debug.LogError("Can not get the reference of 'HealthText'!");
            }
        }
        else
        {
            Debug.LogError("GameObject 'UICamera' is missing in the current scene!");
        }
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        UpdateHealth();
    }

    private void UpdateHealth()
    {
        if (_health <= 0)
        {
            _health = 0;
            _death++;
            Respawn();
        }

        _healthText.text = "Health: " + _health;
    }

    private void Respawn()
    {
        if (null != explosion)
        {
            Instantiate(explosion, transform.position, transform.rotation);
        }

        if (null != playerExplosion)
        {
            Instantiate(playerExplosion, transform.position, transform.rotation);
        }

        tsTransform.position = new TSVector(TSRandom.Range(-5, 5), 0, TSRandom.Range(-5, 5));
    }


}
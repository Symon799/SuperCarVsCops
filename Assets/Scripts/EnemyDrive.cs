using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDrive : MonoBehaviour
{
    public GameObject[] Smokes;
    public GameObject prefabExplosion;
    public float Normalspeed = 23f;
    private float speed;
    public float rotationSpeed = 50f;
    private int enemyLife = 3;
    private const float DepopDistance = 90;
    private GameManager gameManager;
    private Transform player;
    private Text comboText;
    private ComboZone comboZone;


    // Start is called before the first frame update
    void Start()
    {
        comboText = GameObject.FindGameObjectWithTag("Combo").GetComponent<Text>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        comboZone = player.GetComponentInChildren<ComboZone>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        speed = Normalspeed;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = (player.position - this.transform.position).magnitude;
        if (distance > DepopDistance)
        {
            Debug.Log("DETROYED");
            Destroy(this.gameObject);
        }

        transform.position += transform.forward * Time.deltaTime * speed;
        Vector3 _direction = (player.position - transform.position).normalized;
        Quaternion _lookRotation = Quaternion.LookRotation(_direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * rotationSpeed);
        GetComponent<Rigidbody>().velocity *= 0.99f;

        if (comboZone.GetMultiplierCombo() > 0 && comboZone.GetComboLevelAmount() > 0.2f)
        {
            comboText.text = "";
            comboText.text = "COMBO X" + comboZone.GetMultiplierCombo();
        }
        else
        {
            comboText.text = "";
        }
    }

    void DecreaseEnemyLife(int nb = 1)
    {
            enemyLife -= nb;
            UpdateSmoke();
            if (enemyLife <= 0)
            {
                GameObject expl = Instantiate(prefabExplosion, transform.position, Quaternion.identity);
                gameManager.PlayExplosion();
                Destroy(expl, 1f);
                comboZone.TryAddMultiplierCombo();
                gameManager.IncreaseScore(6 * (comboZone.GetMultiplierCombo() == 0 ? 1 : comboZone.GetMultiplierCombo()));
                gameManager.AddCopDestroyActualChallenge();
                Destroy(this.gameObject);
            }
    }
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("TruckEnemy"))
        {
            DecreaseEnemyLife();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            speed = Normalspeed - 3;
        
        if (other.gameObject.CompareTag("Obstacle"))
            DecreaseEnemyLife();
        
        if (other.gameObject.CompareTag("Bullet"))
            DecreaseEnemyLife(3);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            speed = Normalspeed;
    }

    void UpdateSmoke()
    {
        if (enemyLife == 1)
        {
            Smokes[0].SetActive(false);
            Smokes[1].SetActive(true);
        }
        else if (enemyLife == 2)
        {
            Smokes[0].SetActive(true);
            Smokes[1].SetActive(false);
        }
        else if (enemyLife == 3)
        {
            Smokes[0].SetActive(false);
            Smokes[1].SetActive(false);
        }
    }

}

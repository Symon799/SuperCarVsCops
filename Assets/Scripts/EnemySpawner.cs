using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemySpawner : MonoBehaviour
{
	public List<GameObject> enemyPrefabs;
	public List<GameObject> enemyList;
	private List<Vector3> enemySpawn;
    private GameObject player;
    private GameManager gameManager;
    private float nextTime = 0.0f;
    private float interval = 0.5f;
    private int nbTrucks = 0;
 
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gameManager = GetComponent<GameManager>();
        enemySpawn = new List<Vector3>();
    }

    void SetNewSpawnPos()
    {
        Vector3 playerPos = player.transform.position;
        enemySpawn.Add(new Vector3(playerPos.x + 50, playerPos.y, playerPos.z));
        enemySpawn.Add(new Vector3(playerPos.x - 50, playerPos.y, playerPos.z));
        enemySpawn.Add(new Vector3(playerPos.x, playerPos.y, playerPos.z + 70));
        enemySpawn.Add(new Vector3(playerPos.x, playerPos.y, playerPos.z - 60));
    }

    // Update is called once per frame
    void Update ()
    {
        if (Time.time >= nextTime)
        {
            if (!gameManager.IsDead() && !gameManager.GetStartMenu())
		    {
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                if (enemies.Length < 4)
                {
                    SetNewSpawnPos();
                    Instantiate(enemyPrefabs[0], enemySpawn[Random.Range(0, enemySpawn.Count)], Quaternion.identity);
                    enemySpawn.Clear();
                }
                else if (gameManager.GetScore() > 200)
                {
                    int nbTrucks = gameManager.GetScore() / 200;
                    nbTrucks = nbTrucks > 2 ? 2 : nbTrucks;
                    GameObject[] trucks = GameObject.FindGameObjectsWithTag("TruckEnemy");
                    if (trucks.Length < nbTrucks)
                    {
                        SetNewSpawnPos();
                        Instantiate(enemyPrefabs[1], enemySpawn[Random.Range(0, enemySpawn.Count)], Quaternion.identity);
                        enemySpawn.Clear();
                    }
                }
            }
            nextTime += interval;
        }
    }

    public void DestroyEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject en in enemies)
            Destroy(en);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public GameObject prefabBullet;
    private float nextTime = 0.0f;
    private float interval = 0.5f;

    private int shootLevel = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public int AddShootLevel()
    {
        if (shootLevel < 2)
            return ++shootLevel;
        return -1;
    }

    public int RemoveShootLevel()
    {
        if (shootLevel > 0)
            return --shootLevel;
        return -1;
    }

    public int GetShootLevel()
    {
        return shootLevel;
    }
    
    public void ResetShootLevel()
    {
        shootLevel = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextTime)
        {
            if (shootLevel > 0)
            {
                GameObject bullet0 = GameObject.Instantiate(prefabBullet, transform.position, transform.rotation);
                Destroy(bullet0, 1.5f);
            }
            if (shootLevel > 1)
            {
                GameObject bullet1 = GameObject.Instantiate(prefabBullet, transform.position + transform.right, transform.rotation);
                Destroy(bullet1, 1.5f);

                GameObject bullet2 = GameObject.Instantiate(prefabBullet, transform.position - transform.right, transform.rotation);
                Destroy(bullet2, 1.5f);
            }
            nextTime += interval;
        }
    }
}

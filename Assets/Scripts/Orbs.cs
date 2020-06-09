using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbs : MonoBehaviour
{
    public GameObject prefabOrbHit;
    private GameObject player;
    private List<GameObject> Childrens;

    private int orbLevel = 0;

    public int AddOrbLevel()
    {
        if (orbLevel < 2)
            return ++orbLevel;
        return -1;
    }

    public int RemoveOrbLevel()
    {
        if (orbLevel > 0)
            return --orbLevel;
        return -1;
    }

    public int GetOrbLevel()
    {
        return orbLevel;
    }

    public void ResetOrbLevel()
    {
        orbLevel = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Childrens = new List<GameObject>();
        foreach (Transform child in transform)
        {
            Childrens.Add(child.gameObject);
            child.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (orbLevel > 0)
            transform.Rotate(0, 200f * Time.deltaTime, 0);

        if (orbLevel == 0)
        {
            Childrens[0].SetActive(false);
            Childrens[1].SetActive(false);
        }
        else if (orbLevel == 1)
        {
            Childrens[0].SetActive(true);
            Childrens[1].SetActive(false);
        }
        else if (orbLevel == 2)
        {
            Childrens[0].SetActive(true);
            Childrens[1].SetActive(true);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            GameObject hit = GameObject.Instantiate(prefabOrbHit, other.gameObject.transform.position, Quaternion.identity);
            hit.transform.parent = player.transform;
            Destroy(hit, 1f);
        }
    }
}
